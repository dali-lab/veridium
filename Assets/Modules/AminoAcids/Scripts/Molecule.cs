using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

namespace Veridium.Modules.AminoAcids {

    [RequireComponent(typeof(XRGrabInteractable))]
    public class Molecule : MonoBehaviour {
        public const float bondLength = 0.2f;
        public HashSet<Atom> atoms;
        public bool Mergeable = true;
        public XRGrabInteractable grabInteractable;
        public UnityEvent<Molecule> OnMoleculeChanged = new UnityEvent<Molecule>();
        private float selectedSince = float.PositiveInfinity;

        public void Awake()
        {
            atoms = new HashSet<Atom>(GetComponentsInChildren<Atom>());
        }

        void Start() {
            grabInteractable = GetComponent<XRGrabInteractable>();
            grabInteractable.firstSelectEntered.AddListener(UpdateSelected);
            grabInteractable.lastSelectExited.AddListener(ResetSelected);
        }

        private void UpdateSelected(SelectEnterEventArgs args) {
            selectedSince = Time.time;
        }

        private void ResetSelected(SelectExitEventArgs args) {
            selectedSince = float.PositiveInfinity;
        }

        public void MergeWith(Molecule other, Atom bondingAtom1, Atom bondingAtom2, int electrons = 2, bool keepDistance = false) {
            if (other == this) { bondingAtom1.BondWith(bondingAtom2, electrons); return; }
            if (!Mergeable || !other.Mergeable) return;
            if (selectedSince > other.selectedSince) return;
            if (selectedSince == other.selectedSince && GetInstanceID() < other.GetInstanceID()) return;

            // Align size and position
            if (!keepDistance) ResizeAndAlign(other, bondingAtom1, bondingAtom2);

            // Reparent atoms
            foreach (Atom atom in other.atoms) {
                AddAtom(atom);
            }

            // Reparent bonds
            foreach (Bond bond in other.GetComponentsInChildren<Bond>()) {
                bond.transform.SetParent(transform);
            }

            // other molecule should be empty
            if (other.transform.childCount != 0) {
                Debug.LogWarning("Molecule should be empty after merging");
            }

            // Bond atoms
            bondingAtom1.BondWith(bondingAtom2, electrons);

            // Transfer colliders
            grabInteractable.colliders.AddRange(other.grabInteractable.colliders);

            // Destroy other molecule
            if (keepDistance) {
                DestroyImmediate(other.gameObject);
                return;
            }

            IXRSelectInteractor interactor1 = grabInteractable.GetOldestInteractorSelecting();
            IXRSelectInteractor interactor2 = other.grabInteractable.GetOldestInteractorSelecting();
            StartCoroutine(TriggerColliderUpdateAndReselect(interactor1, interactor2));

            Destroy(other.gameObject);
            MoleculeChanged();
        }

        public void Split(Atom atom1, Atom atom2) {
            List<Atom> atomsToTransfer = atom2.GetConnectedAtoms();
            List<Transform> TsToTransfer = atomsToTransfer.Select(a => a.transform).ToList();
            if (atomsToTransfer.Contains(atom1)) return;

            Molecule newMolecule = Instantiate(MoleculeManager.Instance.EmptyMoleculePrefab, transform.position, transform.rotation);
            newMolecule.transform.localScale = transform.localScale;

            foreach (Collider c in grabInteractable.colliders.Where(c => TsToTransfer.Contains(c.transform)).ToList()) {
                c.transform.SetParent(newMolecule.transform);
                grabInteractable.colliders.Remove(c);
                newMolecule.grabInteractable.colliders.Add(c);
            }

            foreach (Atom atom in atomsToTransfer) {
                RemoveAtom(atom);
                newMolecule.AddAtom(atom);

                foreach (Bond bond in atom.Bonds) {
                    bond.transform.SetParent(newMolecule.transform);
                }
            }

            StartCoroutine(TriggerColliderUpdate());
            newMolecule.StartCoroutine(newMolecule.TriggerColliderUpdate());
            StartCoroutine(MoveSplitMoleculesApart(newMolecule, atom1, atom2));

            MoleculeChanged();
        }

        private IEnumerator MoveSplitMoleculesApart(Molecule other, Atom atom1, Atom atom2, float duration = 0.3f, float distanceEach = 0.1f) {
            Vector3 direction = (atom2.transform.position - atom1.transform.position).normalized;
            Vector3 offset = direction * distanceEach;

            Vector3 startPosSelf = transform.position;
            Vector3 targetPosSelf = transform.position - offset;
            Vector3 startPosOther = other.transform.position;
            Vector3 targetPosOther = other.transform.position + offset;

            Mergeable = false;
            other.Mergeable = false;

            float elapsedTime = 0;
            while (elapsedTime <= duration) {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(startPosSelf, targetPosSelf, elapsedTime / duration);
                other.transform.position = Vector3.Lerp(startPosOther, targetPosOther, elapsedTime / duration);
                yield return null;
            }

            // transform.position = targetPosSelf;
            // other.transform.position = targetPosOther;

            Mergeable = true;
            other.Mergeable = true;
        }

        private void MoleculeChanged() {
            MoleculeManager.Instance.UpdateMolecule(this);
            OnMoleculeChanged.Invoke(this);
        }

        private IEnumerator TriggerColliderUpdateAndReselect(IXRSelectInteractor interactor1, IXRSelectInteractor interactor2) {
            grabInteractable.interactionManager.UnregisterInteractable(grabInteractable as IXRInteractable);
            yield return new WaitForEndOfFrame();
            grabInteractable.interactionManager.RegisterInteractable(grabInteractable as IXRInteractable);
            if (interactor1 != null) grabInteractable.interactionManager.SelectEnter(interactor1, grabInteractable);
            if (interactor2 != null) grabInteractable.interactionManager.SelectEnter(interactor2, grabInteractable);
        }

        private IEnumerator TriggerColliderUpdate() {
            yield return new WaitForSeconds(0.1f);
            grabInteractable.interactionManager.SelectExit(grabInteractable.GetOldestInteractorSelecting(), grabInteractable);
            grabInteractable.interactionManager.UnregisterInteractable(grabInteractable as IXRInteractable);
            yield return new WaitForEndOfFrame();
            grabInteractable.interactionManager.RegisterInteractable(grabInteractable as IXRInteractable);
        }

        public void ResizeAndAlign(Molecule other, Atom bondingAtom1, Atom bondingAtom2) {
            Vector3 bondDirection = bondLength * (bondingAtom2.transform.position - bondingAtom1.transform.position).normalized;
            other.transform.localScale = transform.localScale;
            Vector3 newDirection = bondingAtom2.transform.position - bondingAtom1.transform.position;
            other.transform.position -= newDirection - bondDirection;
        }

        public void AddAtom(Atom atom) {
            atoms.Add(atom);
            atom.Molecule = this;
            atom.transform.SetParent(transform);
        }

        public void RemoveAtom(Atom atom) {
            atoms.Remove(atom);
        }

        [ContextMenu("Center Molecule")]
        public void CenterMolecule() {
            // Get center point of transform respecting all children
            Vector3 center = Vector3.zero;
            foreach (Transform child in transform) {
                center += child.position;
            }
            center /= transform.childCount;

            Vector3 toCenter = center - transform.position;

            // Move all children by toCenter offset
            foreach (Transform child in transform) {
                child.position -= toCenter;
            }
        }
    }
}