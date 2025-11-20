using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

namespace Veridium.Modules.AminoAcids {
    public class MoleculeMirror : MonoBehaviour
    {
        private static Vector3 mirroredScale = new Vector3(-1, 1, 1);
        private Collider selfCollider;
        private Molecule originalMolecule;
        public int originalCollidersColliding;
        private Molecule mirroredMolecule;
        public UnityEvent<Molecule> OnMirrorMolecule;

        void Start()
        {
            selfCollider = GetComponent<Collider>();
            OnMirrorMolecule = new UnityEvent<Molecule>();
        }

        void Update()
        {
            if (!originalMolecule || !mirroredMolecule) return;
            AlignMirroredMolecule();
        }

        void AlignMirroredMolecule()
        {
            Transform origT = originalMolecule.transform;
            Transform selfT = transform;
            Transform mirrorT = mirroredMolecule.transform;

            // Mirror Position
            Vector3 toOriginal = origT.position - selfT.position;
            Vector3 mirroredPosition = origT.position - 2 * Vector3.Dot(toOriginal, selfT.right) * selfT.right;
            mirrorT.position = mirroredPosition;

            // Mirror Rotation
            Vector3 mirroredForward = origT.forward - 2 * Vector3.Dot(origT.forward, selfT.right) * selfT.right;
            Vector3 mirroredUp = origT.up - 2 * Vector3.Dot(origT.up, selfT.right) * selfT.right;

            mirrorT.rotation = Quaternion.LookRotation(mirroredForward, mirroredUp);

            mirrorT.localScale = Vector3.Scale(origT.lossyScale, mirroredScale);
            
            // Vector3 eulerOriginal = originalMolecule.transform.rotation.eulerAngles;
            // Vector3 eulerSelf = transform.rotation.eulerAngles;
            // Vector3 eulerMirrored = Vector3.LerpUnclamped(eulerOriginal, eulerSelf, 2f);
            // eulerMirrored.x = -eulerOriginal.x;

            // // Quaternion finalRotation = Quaternion.LerpUnclamped(originalMolecule.transform.rotation, transform.rotation, 2f);
            // mirroredMolecule.transform.rotation = Quaternion.Euler(eulerMirrored);

            // // Vector3 perpendicularVector = Vector3.Project(transform.position - originalMolecule.transform.position, transform.right);
            // // mirroredMolecule.transform.position = originalMolecule.transform.position + 2f * perpendicularVector;

            // Vector3 toCenter = transform.position - originalMolecule.transform.position;
            // Vector3 reflected = Vector3.Reflect(toCenter, transform.right);
            // mirroredMolecule.transform.position = transform.position - reflected;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.attachedRigidbody.TryGetComponent(out Molecule molecule)) return;
            if (molecule == mirroredMolecule) return;
            if (molecule == originalMolecule) {
                originalCollidersColliding++;
                return;
            }

            SetMirroredMolecule(molecule);
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.attachedRigidbody.TryGetComponent(out Molecule molecule)) return;
            if (molecule != originalMolecule) return;

            originalCollidersColliding--;
            if (originalCollidersColliding == 0) ReleaseOriginalMolecule();
        }

        void SetMirroredMolecule(Molecule molecule)
        {
            if (originalMolecule) ReleaseOriginalMolecule();
            MirrorMolecule(molecule);
            OnMirrorMolecule.Invoke(mirroredMolecule);
        }

        void ReleaseOriginalMolecule() {
            Destroy(mirroredMolecule.gameObject);
            originalMolecule.OnMoleculeChanged.RemoveListener(ReactToChangedMolecule);
            mirroredMolecule = null;
            originalMolecule = null;
        }

        void MirrorMolecule(Molecule molecule) {
            originalMolecule = molecule;
            originalMolecule.Mergeable = false;

            Vector3 distanceToOriginal = transform.position - originalMolecule.transform.position;
            mirroredMolecule = Instantiate(molecule, originalMolecule.transform.position + 2 * distanceToOriginal, transform.rotation);
            mirroredMolecule.transform.localScale = Vector3.Scale(originalMolecule.transform.lossyScale, mirroredScale);
            AlignMirroredMolecule();
            
            originalMolecule.Mergeable = true;
            originalCollidersColliding = 1;
            originalMolecule.OnMoleculeChanged.AddListener(ReactToChangedMolecule);
        }

        void ReactToChangedMolecule(Molecule molecule) {
            Destroy(mirroredMolecule.gameObject);
            MirrorMolecule(molecule);
        }
    }
}
