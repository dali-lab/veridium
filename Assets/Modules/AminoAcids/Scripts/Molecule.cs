using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meta.WitAi.Utilities;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.XR.Interaction.Toolkit;

namespace Veridium.Modules.AminoAcids {

    [RequireComponent(typeof(XRGrabInteractable))]
    public class Molecule : MonoBehaviour {
        public HashSet<Atom> atoms;
        public XRGrabInteractable grabInteractable;
        private float selectedSince = float.PositiveInfinity;

        void Start() {
            grabInteractable = GetComponent<XRGrabInteractable>();
            atoms = new HashSet<Atom>(GetComponentsInChildren<Atom>());
            grabInteractable.firstSelectEntered.AddListener(UpdateSelected);
            grabInteractable.lastSelectExited.AddListener(ResetSelected);
        }

        private void UpdateSelected(SelectEnterEventArgs args) {
            selectedSince = Time.time;
        }

        private void ResetSelected(SelectExitEventArgs args) {
            selectedSince = float.PositiveInfinity;
        }

        public void MergeWith(Molecule other, Atom bondingAtom1, Atom bondingAtom2) {
            if (other == this) return;
            if (selectedSince > other.selectedSince) return;
            if (selectedSince == other.selectedSince && GetInstanceID() < other.GetInstanceID()) return;

            // Get interactors
            IXRSelectInteractor interactor1 = grabInteractable.GetOldestInteractorSelecting();
            IXRSelectInteractor interactor2 = other.grabInteractable.GetOldestInteractorSelecting();

            // Align size and position
            ResizeAndAlign(other, bondingAtom1, bondingAtom2);

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
            bondingAtom1.BondWith(bondingAtom2);

            // Transfer colliders
            grabInteractable.colliders.AddRange(other.grabInteractable.colliders);
            StartCoroutine(TriggerColliderUpdate(interactor1, interactor2));

            // Destroy other molecule
            Destroy(other.gameObject);
        }

        private IEnumerator TriggerColliderUpdate(IXRSelectInteractor interactor1, IXRSelectInteractor interactor2) {
            grabInteractable.interactionManager.UnregisterInteractable(grabInteractable as IXRInteractable);
            yield return new WaitForEndOfFrame();
            grabInteractable.interactionManager.RegisterInteractable(grabInteractable as IXRInteractable);
            if (interactor1 != null) grabInteractable.interactionManager.SelectEnter(interactor1, grabInteractable);
            if (interactor2 != null) grabInteractable.interactionManager.SelectEnter(interactor2, grabInteractable);
        }

        public void ResizeAndAlign(Molecule other, Atom bondingAtom1, Atom bondingAtom2) {
            Vector3 bondDirection = 2 * transform.localScale.x * (bondingAtom2.transform.position - bondingAtom1.transform.position).normalized;
            other.transform.localScale = transform.localScale;
            Vector3 newDirection = bondingAtom2.transform.position - bondingAtom1.transform.position;
            other.transform.position -= newDirection - bondDirection;
        }

        public void AddAtom(Atom atom) {
            atoms.Add(atom);
            atom.molecule = this;
            atom.transform.SetParent(transform);
        }

        public void RemoveAtom(Atom atom) {
            atoms.Remove(atom);
            atom.molecule = null;
            atom.transform.SetParent(null);

            foreach (Bond bond in atom.bonds.Where(bond => atoms.Contains(bond.Other(atom)))) {
                bond.Destroy();
            }
        }
    }
}