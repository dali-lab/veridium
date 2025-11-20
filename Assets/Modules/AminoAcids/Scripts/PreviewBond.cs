using System.Collections;
using UnityEngine;
using UnityEditor;

namespace Veridium.Modules.AminoAcids {
    public class PreviewBond : MonoBehaviour
    {
        private static Vector3[] singleBondPositions = new Vector3[] {
            Vector3.zero
        };
        private static Vector3[] doubleBondPositions = new Vector3[] {
            -0.2f * Vector3.forward,
            0.2f * Vector3.forward
        };
        private static Vector3[] tripleBondPositions = new Vector3[] {
            -0.2f * Vector3.forward + -0.1f * Vector3.right,
            0.2f * Vector3.forward + -0.1f * Vector3.right,
            0.2f * Vector3.right
        };
        private static Vector3[] quadrupleBondPositions = new Vector3[] {
            -0.2f * Vector3.forward + -0.2f * Vector3.right,
            0.2f * Vector3.forward + -0.2f * Vector3.right,
            -0.2f * Vector3.forward + 0.1f * Vector3.right,
            0.2f * Vector3.forward + 0.1f * Vector3.right
        };

        public Atom atom1;
        public Atom atom2;
        public Molecule Molecule => atom1.Molecule;
        public int electrons;
        private Material material;

        public static PreviewBond Create(Atom atom1, Atom atom2, int electrons)
        {
            PreviewBond bond = new GameObject("PreviewBond").AddComponent<PreviewBond>();
            bond.gameObject.hideFlags = HideFlags.HideInHierarchy;
            bond.atom1 = atom1;
            bond.atom2 = atom2;
            bond.electrons = electrons;
            bond.material = MoleculeManager.Instance.previewBondMaterial;

            bond.UpdateTransform();
            bond.CreateCylinders();

            return bond;
        }

        public void UpdateTransform() {
            Vector3 bondDirection = (atom2.transform.position - atom1.transform.position).normalized;
            Vector3 forward = Vector3.Cross(bondDirection, Vector3.forward);
            transform.rotation = Quaternion.LookRotation(forward, bondDirection);

            float distance = Vector3.Distance(atom1.transform.position, atom2.transform.position);
            float minScale = Mathf.Min(atom1.Molecule.transform.lossyScale.x, atom2.Molecule.transform.lossyScale.x);
            transform.localScale = new Vector3(minScale, 0.5f * distance, minScale);
            transform.position = (atom1.transform.position + atom2.transform.position) / 2;
        }

        private void CreateCylinders()
        {
            Vector3[] positions = electrons switch
            {
                1 => singleBondPositions,
                2 => doubleBondPositions,
                3 => tripleBondPositions,
                4 => quadrupleBondPositions,
                _ => singleBondPositions
            };
            for (int i = 0; i < electrons; i++)
            {
                GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                cylinder.transform.parent = transform;
                cylinder.transform.localPosition = positions[i];
                cylinder.transform.localRotation = Quaternion.identity;
                cylinder.transform.localScale = new Vector3(0.25f, 1f, 0.25f);
                cylinder.GetComponent<Renderer>().material = material;
            }
        }

        public void ChangeElectrons(int newElectrons)
        {
            if (newElectrons < 1 || newElectrons > 4)
            {
                Debug.LogError($"Cannot change bond electrons to {newElectrons}. Valid range is 1-4.");
                return;
            }

            foreach (Transform child in transform)
            {
                Molecule.grabInteractable.colliders.Remove(child.GetComponent<Collider>());
                Destroy(child.gameObject);
            }

            electrons = newElectrons;
            CreateCylinders();
        }

        private IEnumerator AnimateBondExtension(float duration)
        {
            Vector3 bondDirection = 10f * Molecule.bondLength * Vector3.Scale(atom1.Molecule.transform.localScale, (atom2.transform.position - atom1.transform.position).normalized);
            Vector3 bondPoint2 = atom1.transform.position + bondDirection;

            Vector3 startPos = atom1.transform.position;
            Vector3 endPos = (atom1.transform.position + bondPoint2) / 2;
            Vector3 startScale = new Vector3(1, 0, 1);
            Vector3 endScale = new Vector3(1, 5f * Vector3.Distance(atom1.transform.position, bondPoint2), 1);

            float elapsedTime = 0f, t;
            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;
                t = (elapsedTime / duration).EaseOut();

                transform.position = Vector3.Lerp(startPos, endPos, t);
                transform.localScale = Vector3.Lerp(startScale, endScale, t);

                yield return null;
            }
            transform.localScale = endScale;
        }
    }
}