using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using System;
using Veridium.Animation;

namespace Veridium.Modules.AminoAcids {

    [RequireComponent(typeof(XRGrabInteractable))]
    public class Molecule : MonoBehaviour
    {
        public const float bondLength = 2.5f;
        public HashSet<Atom> atoms;
        public Bond[] Bonds => GetComponentsInChildren<Bond>();
        public bool Mergeable = true;
        public XRGrabInteractable grabInteractable;
        public UnityEvent<Molecule> OnMoleculeChanged = new UnityEvent<Molecule>();
        private float selectedSince = float.PositiveInfinity;
        public Atom AlphaCarbon;
        public Bond AlphaCToR;

        public Vector3 mainAxis;
        public Vector3 AlignmentAxis => transform.up;
        public Vector3 PerpendicularToMainAxis => Vector3.Cross(mainAxis, AlignmentAxis).normalized;
        private MoleculeMergeInfo mergeInfo;

        public void Awake()
        {
            atoms = new HashSet<Atom>(GetComponentsInChildren<Atom>());
        }

        void Start()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
            grabInteractable.firstSelectEntered.AddListener(UpdateSelected);
            grabInteractable.lastSelectExited.AddListener(ResetSelected);
            grabInteractable.activated.AddListener(Activated);

            foreach (Bond bond in GetComponentsInChildren<Bond>())
            {
                foreach (Collider collider in bond.GetComponentsInChildren<Collider>())
                {
                    if (grabInteractable.colliders.Contains(collider)) continue;
                    grabInteractable.colliders.Add(collider);
                }
            }
        }

        private void UpdateSelected(SelectEnterEventArgs args)
        {
            selectedSince = Time.time;
        }

        private void ResetSelected(SelectExitEventArgs args)
        {
            selectedSince = float.PositiveInfinity;
        }


        private void CompleteMerge(Molecule other, Atom bondingAtom1, Atom bondingAtom2, int electrons)
        {
            BasicMergeSteps(other, bondingAtom1, bondingAtom2, electrons);

            IXRSelectInteractor interactor1 = grabInteractable.GetOldestInteractorSelecting();
            IXRSelectInteractor interactor2 = other.grabInteractable.GetOldestInteractorSelecting();
            StartCoroutine(TriggerColliderUpdateAndReselect(interactor1, interactor2));

            Destroy(other.gameObject);
            RestoreAngles(bondingAtom1, bondingAtom2);
            // CenterMolecule();
            MoleculeChanged();
        }

        public void MergeWithAndKeepDistance(Molecule other, Atom bondingAtom1, Atom bondingAtom2, int electrons = 1)
        {
            if (!Mergeable || !other.Mergeable) return;
            if (selectedSince > other.selectedSince) return;
            if (other == this) { bondingAtom1.BondWith(bondingAtom2, electrons); return; }
            if (selectedSince == other.selectedSince && GetInstanceID() < other.GetInstanceID()) return;

            BasicMergeSteps(other, bondingAtom1, bondingAtom2, electrons, false);

            DestroyImmediate(other.gameObject);
        }

        public void BasicMergeSteps(Molecule other, Atom bondingAtom1, Atom bondingAtom2, int electrons, bool center = true)
        {
            // Reparent atoms
            foreach (Atom atom in other.atoms)
            {
                AddAtom(atom);
            }

            // Reparent bonds
            foreach (Bond bond in other.GetComponentsInChildren<Bond>())
            {
                bond.transform.parent = transform;
            }

            // other molecule should be empty
            if (other.transform.childCount != 0)
            {
                Debug.LogWarning("Molecule should be empty after merging");
            }

            // Bond atoms
            bondingAtom1.BondWith(bondingAtom2, electrons);

            foreach (Collider c in grabInteractable.colliders)
            {
                Debug.Log(c.name, c.gameObject);
            }

            foreach (Collider c in other.grabInteractable.colliders)
            {
                Debug.Log(c.name, c.gameObject);
            }

            // Transfer colliders
            // grabInteractable.colliders.AddRange(other.grabInteractable.colliders);
            foreach (Collider collider in other.grabInteractable.colliders)
            {
                grabInteractable.colliders.Add(collider);
            }
        }

        public void Split(Atom atom1, Atom atom2)
        {
            // print("Split");
            StartCoroutine(SplitCoroutine(atom1, atom2));
        }

        private IEnumerator SplitCoroutine(Atom atom1, Atom atom2)
        {
            // print("Split Coroutine");
            List<Atom> atomsToTransfer = atom2.GetConnectedAtoms();
            List<Transform> TsToTransfer = atomsToTransfer.Select(a => a.transform).ToList();
            if (atomsToTransfer.Contains(atom1)) yield break;
            // print("Doesn't contain atom1");

            XRInteractionManager manager = grabInteractable.interactionManager;
            Molecule newMolecule = Instantiate(MoleculeManager.Instance.EmptyMoleculePrefab, transform.position, transform.rotation);
            newMolecule.transform.localScale = transform.localScale;

            manager.UnregisterInteractable(grabInteractable as IXRInteractable);
            manager.UnregisterInteractable(newMolecule.grabInteractable as IXRInteractable);
            // print("Unregistered interactables");

            yield return new WaitForEndOfFrame();
            // print("Waited for 1st time");

            foreach (Collider c in grabInteractable.colliders.Where(c => TsToTransfer.Contains(c.transform)).ToList())
            {
                c.transform.SetParent(newMolecule.transform);
                grabInteractable.colliders.Remove(c);
                newMolecule.grabInteractable.colliders.Add(c);
            }

            foreach (Atom atom in atomsToTransfer)
            {
                RemoveAtom(atom);
                newMolecule.AddAtom(atom);

                foreach (Bond bond in atom.Bonds)
                {
                    bond.transform.SetParent(newMolecule.transform);
                    foreach (Collider collider in bond.GetComponentsInChildren<Collider>())
                    {
                        grabInteractable.colliders.Remove(collider);
                        newMolecule.grabInteractable.colliders.Add(collider);
                    }
                }
            }
            // print("Finished transferring atoms and bonds");

            yield return new WaitForEndOfFrame();

            manager.RegisterInteractable(grabInteractable as IXRInteractable);
            manager.RegisterInteractable(newMolecule.grabInteractable as IXRInteractable);
            // print("Registered interactables");

            // CenterMolecule();
            // newMolecule.CenterMolecule();

            // StartCoroutine(TriggerColliderUpdate());
            // StartCoroutine(newMolecule.TriggerColliderUpdate());
            // StartCoroutine(MoveSplitMoleculesApart(newMolecule, atom1, atom2));

            StartCoroutine(MoveSplitMoleculesApart(newMolecule, atom1, atom2));
            MoleculeChanged();
            // print("Finished splitting molecules");
        }

        private IEnumerator MoveSplitMoleculesApart(Molecule other, Atom atom1, Atom atom2, float duration = 0.3f, float distanceEach = 0.1f)
        {
            Vector3 direction = (atom2.transform.position - atom1.transform.position).normalized;
            Vector3 offset = direction * distanceEach;

            Vector3 startPosSelf = transform.position;
            Vector3 targetPosSelf = transform.position - offset;
            Vector3 startPosOther = other.transform.position;
            Vector3 targetPosOther = other.transform.position + offset;

            Mergeable = false;
            other.Mergeable = false;

            float elapsedTime = 0;
            while (elapsedTime <= duration)
            {
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

        private void MoleculeChanged()
        {
            UpdateMainAxis();
            MoleculeManager.Instance.UpdateMolecule(this);
            OnMoleculeChanged.Invoke(this);
        }

        private IEnumerator TriggerColliderUpdateAndReselect(IXRSelectInteractor interactor1, IXRSelectInteractor interactor2)
        {
            grabInteractable.interactionManager.UnregisterInteractable(grabInteractable as IXRInteractable);
            yield return new WaitForEndOfFrame();
            grabInteractable.interactionManager.RegisterInteractable(grabInteractable as IXRInteractable);
            if (interactor1 != null) grabInteractable.interactionManager.SelectEnter(interactor1, grabInteractable);
            if (interactor2 != null) grabInteractable.interactionManager.SelectEnter(interactor2, grabInteractable);
        }

        public IEnumerator TriggerColliderUpdate()
        {
            yield return new WaitForSeconds(0.1f);
            grabInteractable.interactionManager.SelectExit(grabInteractable.GetOldestInteractorSelecting(), grabInteractable);
            grabInteractable.interactionManager.UnregisterInteractable(grabInteractable as IXRInteractable);
            yield return new WaitForEndOfFrame();
            grabInteractable.interactionManager.RegisterInteractable(grabInteractable as IXRInteractable);
        }

        private void ResizeAndAlignNoAnim(Molecule other, Atom bondingAtom1, Atom bondingAtom2, int electrons)
        {
            Vector3 from1To2 = Vector3.Scale(GetBondLength(electrons) * transform.localScale, (bondingAtom2.transform.position - bondingAtom1.transform.position).normalized);
            Vector3 atom2Pos = bondingAtom1.transform.position + from1To2;

            other.transform.localScale = transform.localScale;

            Vector3 moveTo = atom2Pos - bondingAtom2.transform.position;
            other.transform.position += moveTo;

            bondingAtom1.BondWith(bondingAtom2, electrons);
            CompleteMerge(other, bondingAtom1, bondingAtom2, electrons);
        }

        public void AddAtom(Atom atom)
        {
            atoms.Add(atom);
            atom.Molecule = this;
            atom.transform.parent = transform;
            atom.transform.localScale = atom.element.AtomSize() * Vector3.one;
        }

        public void RemoveAtom(Atom atom)
        {
            atoms.Remove(atom);
        }

        [ContextMenu("Center Molecule")]
        public void CenterMolecule()
        {
            // Get center point of transform respecting all children
            Vector3 center = Vector3.zero;
            int count = 0;

            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<Atom>(out _)) continue;
                count++;
                center += child.position;
            }
            center /= count;

            Vector3 toCenter = center - transform.position;

            // Move all children by toCenter offset
            foreach (Transform child in transform)
            {
                child.position -= toCenter;
            }
        }

        public void RestoreAngles(Atom atom1, Atom atom2)
        {
            // int maxAtomCount = 0;
            Bond fixedBond = atom1.GetBondTo(atom2);

            // foreach (Bond b in atom1.Bonds)
            // {
            //     int count = b.Other(atom1).GetConnectedAtoms(atom1).Count;
            //     if (count <= maxAtomCount) continue;

            //     maxAtomCount = count;
            //     fixedBond = b;
            // }

            // foreach (Bond b in atom2.Bonds)
            // {
            //     int count = b.Other(atom2).GetConnectedAtoms(atom2).Count;
            //     if (count <= maxAtomCount) continue;

            //     maxAtomCount = count;
            //     fixedBond = b;
            // }

            // if (fixedBond == null) return;

            Mergeable = false;
            bool bondBelongsToAtom1 = atom1.Bonds.Contains(fixedBond);

            StartCoroutine(SetAnglesForAtom(bondBelongsToAtom1 ? atom1 : atom2, fixedBond));
            StartCoroutine(SetAnglesForAtom(bondBelongsToAtom1 ? atom2 : atom1, atom1.GetBondTo(atom2)));

            Mergeable = true;
        }

        private List<Vector3> GetDirectionsForOtherBonds(Atom atom, Bond fixedBond)
        {
            Vector3 fixedBondDirection = (fixedBond.Other(atom).transform.position - atom.transform.position).normalized;
            Vector3 perpendicularToBond = Vector3.Cross(fixedBondDirection, atom.transform.up).normalized;
            List<Vector3> result = new List<Vector3>();

            switch (atom.Bonds.Except(new[] { fixedBond }).ToList().Count)
            {
                case 1:
                    // Only one other bond, so we can use the opposite direction
                    result.Add(-1 * fixedBondDirection);
                    break;
                case 2:
                    // Two other bonds, so we can use 120 degree angles
                    result.Add(Quaternion.AngleAxis(120, perpendicularToBond) * fixedBondDirection);
                    result.Add(Quaternion.AngleAxis(-120, perpendicularToBond) * fixedBondDirection);
                    break;
                case 3:
                    // Three other bonds, so we have to calculate 109.5 degrees tetrahedral angles
                    Vector3 firstDirection = Quaternion.AngleAxis(109.5f, perpendicularToBond) * fixedBondDirection;
                    // if (cAxis != null && Vector3.Angle(firstDirection,transform.TransformDirection(cAxis)) > 75f)
                    // {
                    //     firstDirection = Quaternion.AngleAxis(-109.5f, perpendicularToBond) * fixedBondDirection;
                    // }
                    result.Add(firstDirection);
                    result.Add(Quaternion.AngleAxis(109.5f, firstDirection) * fixedBondDirection);
                    result.Add(Quaternion.AngleAxis(-109.5f, firstDirection) * fixedBondDirection);
                    break;
                default:
                    break;
            }

            return result;
        }

        private IEnumerator SetAnglesForAtom(Atom atom, Bond fixedBond)
        {
            if (atom.AreNeighborsConnectedInOtherWay()) yield break;
            List<Vector3> bondDirections = GetDirectionsForOtherBonds(atom, fixedBond);
            Dictionary<Transform, Quaternion> dummies = new Dictionary<Transform, Quaternion>();

            foreach (Bond b in atom.Bonds.Except(new[] { fixedBond }).OrderBy(b => b.Other(atom).element == Element.C ? -1 : 1))
            {
                Vector3 currentDirection = (b.Other(atom).transform.position - atom.transform.position).normalized;
                float minAngle = bondDirections.Min(dir => Vector3.Angle(currentDirection, dir));
                Vector3 newDirection = bondDirections.First(dir => Vector3.Angle(currentDirection, dir) == minAngle);
                bondDirections.Remove(newDirection);
                Vector3 axis = Vector3.Cross(currentDirection, newDirection).normalized;
                float angle = Vector3.Angle(currentDirection, newDirection);
                // Quaternion rotation = Quaternion.FromToRotation(currentDirection, newDirection);

                List<Transform> connectedTransforms = b.Other(atom).GetConnectedTransforms(atom);
                connectedTransforms.Add(b.transform);

                foreach (Transform t in connectedTransforms)
                {
                    t.RotateAround(atom.transform.position, axis, angle);
                }
                continue;

                Transform dummy = new GameObject().transform;
                dummy.parent = atom.Molecule.transform;
                dummy.localPosition = atom.transform.localPosition;
                dummy.localRotation = Quaternion.identity; // Quaternion.LookRotation(b.Other(atom).transform.position - atom.transform.position, atom.transform.up);
                dummy.localScale = Vector3.one;

                foreach (Transform t in connectedTransforms)
                {
                    t.parent = dummy;
                }

                dummies[dummy] = Quaternion.FromToRotation(currentDirection, newDirection);
            }
            yield break;

            float duration = 1f;
            float elapsedTime = 0;
            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;
                foreach (var kvp in dummies)
                {
                    kvp.Key.localRotation = Quaternion.Lerp(Quaternion.identity, kvp.Value, elapsedTime / duration);
                }
                yield return null;
            }

            foreach (Transform dummy in dummies.Keys)
            {
                foreach (Transform t in dummy)
                {
                    if (t == dummy) continue;
                    t.parent = atom.Molecule.transform;
                }
                Destroy(dummy.gameObject);
            }
        }

        private float GetBondLength(int electrons)
        {
            switch (electrons)
            {
                case 1: return bondLength;
                case 2: return bondLength * 0.9f;
                case 3: return bondLength * 0.8f;
                case 4: return bondLength * 0.7f;
                default: return bondLength;
            }
        }

        private void Activated(ActivateEventArgs args)
        {
            if (!(args.interactorObject is XRRayInteractor interactor)) return;
            if (!interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit)) return;

            bool hitAtom = hit.collider.TryGetComponentInParent(out Atom atom);
            bool hitBond = hit.collider.TryGetComponentInParent(out Bond bond);

            if (hitAtom) atom.ToggleHighlight();
            if (hitBond) bond.ToggleHighlight();
        }

        public void MergeWith(Molecule other, Atom bondingAtom1, Atom bondingAtom2, int electrons = 1)
        {
            if (!Mergeable || !other.Mergeable) return;
            if (selectedSince > other.selectedSince) return;
            if (bondingAtom1.HasFullValence(electrons) || bondingAtom2.HasFullValence(electrons)) return;
            if (other == this) { bondingAtom1.BondWith(bondingAtom2, electrons); return; }
            if (selectedSince == other.selectedSince && GetInstanceID() < other.GetInstanceID()) return;

            // IXRSelectInteractor interactor1 = grabInteractable.GetOldestInteractorSelecting();
            // IXRSelectInteractor interactor2 = other.grabInteractable.GetOldestInteractorSelecting();
            // if (interactor1 != null && interactor1.IsSelecting(grabInteractable)) {
            //     grabInteractable.interactionManager.SelectExit(interactor1, grabInteractable);
            // }
            // if (interactor2 != null && interactor2.IsSelecting(other.grabInteractable)) {
            //     other.grabInteractable.interactionManager.SelectExit(interactor2, other.grabInteractable);
            // }

            // Align size and position
            ResizeAndAlignNoAnim(other, bondingAtom1, bondingAtom2, electrons);
            // StartCoroutine(ResizeAndAlign(other, bondingAtom1, bondingAtom2, electrons));

            mergeInfo = new MoleculeMergeInfo
            {
                OtherMolecule = other,
                AtomThis = bondingAtom1,
                AtomOther = bondingAtom2,
                Electrons = electrons,
            };

            /*
            Proposed order: (TODO)

            DeselectInteractors()       // SelectExit() for both interactables if selected
            UnregisterInteractables()   // Unregister both interactables from interaction manager

            PositionAndResizeOther()    // Move and resize other molecule to fit bonding atoms
            MergeMolecules()            // Reparent atoms and bonds, create new bond, destroy other molecule
            DestroyOther()
            FixBondAngles()             // Adjust angles of bonds around newly formed bond
            CenterMolecule()            // Center this molecule according to its children's positions

            RegisterInteractables()     // Register this interactable to interaction manager
            ReselectInteractors()       // SelectEnter() new interactable

            MoleculeChanged()           // Notify that the molecule has changed
            */

            mergeInfo = null;
        }


        private IEnumerator DeselectInteractors()
        {
            mergeInfo.PausedInteractors = new List<IXRSelectInteractor>();
            mergeInfo.PausedInteractors.AddRange(grabInteractable.interactorsSelecting);
            mergeInfo.PausedInteractors.AddRange(mergeInfo.OtherMolecule.grabInteractable.interactorsSelecting);

            foreach (IXRSelectInteractor interactor in mergeInfo.PausedInteractors)
            {
                grabInteractable.interactionManager.SelectExit(interactor, grabInteractable);
            }
            yield return new WaitForEndOfFrame();
        }

        private IEnumerator UnregisterInteractables()
        {
            XRInteractionManager manager = grabInteractable.interactionManager;
            manager.UnregisterInteractable(mergeInfo.OtherMolecule.grabInteractable as IXRInteractable);
            manager.UnregisterInteractable(grabInteractable as IXRInteractable);
            yield return new WaitForEndOfFrame();
        }

        private IEnumerator PositionAndResizeOther()
        {
            if (mainAxis == null) SetupMainAxis();

            Transform otherMoleculeT = mergeInfo.OtherMolecule.transform;
            float angleThis = 180f - GetAngleForNextBond(mergeInfo.AtomThis);

            Quaternion necessaryRotation = Quaternion.FromToRotation(otherMoleculeT.TransformDirection(mergeInfo.OtherMolecule.mainAxis), transform.TransformDirection(mainAxis));

            Vector3 desiredPositionOtherAtom = mergeInfo.AtomThis.transform.position + bondLength * transform.TransformDirection(Quaternion.AngleAxis(angleThis, PerpendicularToMainAxis) * mainAxis);
            Vector3 actualFuturePositionOtherAtom = otherMoleculeT.position + Vector3.Scale(transform.localScale, otherMoleculeT.rotation * necessaryRotation * mergeInfo.AtomOther.transform.localPosition);
            Vector3 necessaryTranslation = desiredPositionOtherAtom - actualFuturePositionOtherAtom;

            yield return StartCoroutine(otherMoleculeT.AnimatePositionRotationScale(necessaryTranslation, necessaryRotation, transform.localScale));
        }

        private void MergeMolecules()
        {
            foreach (Atom atom in mergeInfo.OtherMolecule.atoms)
                AddAtom(atom);

            foreach (Bond bond in mergeInfo.OtherMolecule.GetComponentsInChildren<Bond>())
                bond.transform.parent = transform;

            if (mergeInfo.OtherMolecule.transform.childCount != 0)
                Debug.LogWarning("Molecule should be empty after merging");

            mergeInfo.AtomThis.BondWith(mergeInfo.AtomOther, mergeInfo.Electrons);

            foreach (Collider collider in mergeInfo.OtherMolecule.grabInteractable.colliders)
                grabInteractable.colliders.Add(collider);
        }

        private void DestroyOther()
        {
            Destroy(mergeInfo.OtherMolecule.gameObject);
        }

        private IEnumerator FixBondAngles()
        {
            yield break;
        }

        private IEnumerator RegisterInteractable()
        {
            yield return new WaitForEndOfFrame();
            grabInteractable.interactionManager.RegisterInteractable(grabInteractable as IXRInteractable);
        }

        private IEnumerator ReselectInteractors()
        {
            yield return new WaitForEndOfFrame();
            foreach (IXRSelectInteractor interactor in mergeInfo.PausedInteractors)
            {
                grabInteractable.interactionManager.SelectEnter(interactor, grabInteractable);
            }
        }

        private void SetupMainAxis()
        {
            Vector3 newRight = (mergeInfo.AtomOther.transform.position - mergeInfo.AtomThis.transform.position).normalized;
            Quaternion rotation = Quaternion.FromToRotation(transform.right, newRight);
            transform.rotation *= rotation;
            mainAxis = Vector3.right;
        }

        private void UpdateMainAxis()
        {
            if (mainAxis != null) return;

            Func<Bond, bool> isCBond = bond => bond.atom1.element == Element.C && bond.atom2.element == Element.C;
            if (!Bonds.Any(isCBond)) return;

            Bond cBond = Bonds.First(isCBond);
            mainAxis = transform.InverseTransformDirection((cBond.atom2.transform.position - cBond.atom1.transform.position).normalized);
        }

        public void FlipAllAtomAlignments()
        {
            foreach (Atom atom in atoms)
                atom.pointsUpInMolecule = !atom.pointsUpInMolecule;
        }

        private float GetAngleForNextBond(Atom atom)
        {
            return atom.Neighbors.Count switch
            {
                0 => 180f,
                1 => 180f,
                2 => 120f,
                3 => 109.5f,
                _ => 180f
            };
        }
    }
}