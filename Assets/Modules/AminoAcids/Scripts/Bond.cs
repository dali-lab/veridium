using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using Oculus.Platform;
using Oculus.Interaction;
using UnityEditor;
using System;

namespace Veridium.Modules.AminoAcids {
    public class Bond : MonoBehaviour
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

        public void Create(Atom atom1, Atom atom2, int electrons = 2) {
            this.atom1 = atom1;
            this.atom2 = atom2;
            this.electrons = electrons;

            atom1.Bonds.Add(this);
            atom2.Bonds.Add(this);

            float distance = Vector3.Distance(atom1.transform.localPosition, atom2.transform.localPosition);
            // print($"Distance from pos {atom1.transform.localPosition} to pos {atom2.transform.localPosition} is {distance}");

            transform.parent = atom1.Molecule.transform;
            transform.localScale = new Vector3(1, 0.5f * distance, 1);
            transform.position = (atom1.transform.position + atom2.transform.position) / 2;
            
            Vector3 bondDirection = (atom2.transform.position - atom1.transform.position).normalized;
            Vector3 forward = Vector3.Cross(bondDirection, Molecule.transform.forward);
            transform.rotation = Quaternion.LookRotation(forward, bondDirection);

#if UNITY_EDITOR
            Material material = AssetDatabase.LoadAssetAtPath<Material>($"Assets/Modules/AminoAcids/Materials/Bonds/{atom1.element}-{atom2.element}.mat");
#else
            Material material = new Material(Shader.Find("Shader Graphs/BondGraph"));
            material.SetColor("Color1", atom1.element.ToColor());
            material.SetColor("Color2", atom2.element.ToColor());
#endif
            
            Vector3[] positions = electrons switch {
                2 => singleBondPositions,
                4 => doubleBondPositions,
                6 => tripleBondPositions,
                8 => quadrupleBondPositions,
                _ => singleBondPositions
            };
            for (int i = 0; i < electrons / 2; i++) {
                GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                cylinder.transform.parent = transform;
                cylinder.transform.localPosition = positions[i];
                cylinder.transform.localRotation = Quaternion.identity;
                cylinder.transform.localScale = new Vector3(0.25f, 1f, 0.25f);
                cylinder.GetComponent<Renderer>().material = material;
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