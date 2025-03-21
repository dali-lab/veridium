using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using Oculus.Platform;
using Oculus.Interaction;

namespace Veridium.Modules.AminoAcids {
    public class Bond : MonoBehaviour
    {
        public Atom atom1;
        public Atom atom2;
        public Molecule Molecule => atom1.Molecule;
        public int electrons;

        public void Create(Atom atom1, Atom atom2, int electrons = 2) {
            this.atom1 = atom1;
            this.atom2 = atom2;
            this.electrons = electrons;

            atom1.Bonds.Add(this);
            atom2.Bonds.Add(this);

            transform.parent = atom1.Molecule.transform;
            transform.localScale = Vector3.one;
            transform.position = (atom1.transform.position + atom2.transform.position) / 2;
            transform.up = (atom2.transform.position - atom1.transform.position).normalized;
            
            for (int i = 0; i < electrons / 2; i++) {
                GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                cylinder.transform.parent = transform;
                cylinder.transform.localPosition = Vector3.zero;
                cylinder.transform.localRotation = Quaternion.identity;
                cylinder.transform.localScale = new Vector3(0.25f, 1f, 0.25f);
                cylinder.GetComponent<Renderer>().material = Resources.Load<Material>("Bond");
            }
        }

        public void Destroy() {
            atom1.Bonds.Remove(this);
            atom2.Bonds.Remove(this);
            Destroy(gameObject);
            Molecule.Split(atom1, atom2);
        }

        public Atom Other(Atom atom) {
            if (atom == atom1) return atom2;
            if (atom == atom2) return atom1;
            return null;
        }
    }
}