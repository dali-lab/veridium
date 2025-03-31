using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

#if UNITY_EDITOR
namespace Veridium.Modules.AminoAcids {
    public class MolfileMolecule : MonoBehaviour
    {
        public Molfile molfile;

        [ContextMenu("Build Molecule")]
        public void BuildMolecule() {
            Dictionary<int, List<MolfileBond>> bondsByID = new Dictionary<int, List<MolfileBond>>();
            // Dictionary<Tuple<Element, Element>, Material> bondMaterials = new Dictionary<Tuple<Element, Element>, Material>();

            foreach (MolfileBond bond in molfile.Bonds) {
                if (!bondsByID.ContainsKey(bond.Atom1.ID)) {
                    bondsByID[bond.Atom1.ID] = new List<MolfileBond>();
                }
                if (!bondsByID.ContainsKey(bond.Atom2.ID)) {
                    bondsByID[bond.Atom2.ID] = new List<MolfileBond>();
                }
                bondsByID[bond.Atom1.ID].Add(bond);
                bondsByID[bond.Atom2.ID].Add(bond);

                // Tuple<Element, Element> elementTuple = bond.ToElementTuple();
                // if (!bondMaterials.ContainsKey(elementTuple)) {
                //     Material material = AssetDatabase.LoadAssetAtPath<Material>($"Assets/Modules/AminoAcids/Materials/Bonds/{bond.Atom1.Element}-{bond.Atom2.Element}.mat");
                //     if (material == null) {
                //         Debug.LogError($"No material found for bond type {bond.BondType} between {bond.Atom1.Element} and {bond.Atom2.Element}");
                //         continue;
                //     }
                //     bondMaterials[elementTuple] = material;
                // }
            }

            float avgLength = molfile.Bonds.Sum(bond => bond.Length) / molfile.Bonds.Count;
            float scale = Molecule.bondLength / avgLength;

            Dictionary<Element, Molecule> elementPrefabs = new Dictionary<Element, Molecule>();
            Dictionary<int, Atom> instantiatedAtoms = new Dictionary<int, Atom>();
            foreach (MolfileAtom atom in molfile.Atoms) {
                if (!elementPrefabs.ContainsKey(atom.Element)) {
                    Molecule prefab = AssetDatabase.LoadAssetAtPath<Molecule>($"Assets/Modules/AminoAcids/Prefabs/Single-Atom Molecules/{atom.Element}.prefab");
                    elementPrefabs[atom.Element] = prefab;
                }
                if (elementPrefabs[atom.Element] == null) {
                    Debug.LogError($"No prefab found for element {atom.Element}");
                    continue;
                }

                Molecule molecule = Instantiate(elementPrefabs[atom.Element], transform);
                molecule.Awake();
                molecule.transform.localPosition = scale * atom.Position;

                Atom instantiatedAtom = molecule.GetComponentInChildren<Atom>();
                instantiatedAtom.gameObject.name = $"{atom.ID} {atom.Element}";
                instantiatedAtoms[atom.ID] = instantiatedAtom;

            }

            // List<int> visitedAtoms = new List<int>() {1};
            // Queue<int> atomQueue = new Queue<int>();
            // foreach(MolfileBond bond in bondsByID[1]) {
            //     int otherAtomID = bond.GetNeighborID(1);
            //     atomQueue.Enqueue(otherAtomID);
            // }

            // while (atomQueue.Count > 0) {
            //     int atomID = atomQueue.Dequeue();
            //     MolfileAtom atom = molfile.Atoms[atomID - 1];
            //     Atom instantiatedAtom = instantiatedAtoms[atomID];
            //     if (visitedAtoms.Contains(atomID)) continue;

            //     int visitedNeighborID = bondsByID[atomID].First(bond => visitedAtoms.Contains(bond.GetNeighborID(atomID))).GetNeighborID(atomID);
            //     MolfileAtom visitedNeighbor = molfile.Atoms[visitedNeighborID - 1];
            //     Atom visitedAtom = instantiatedAtoms[visitedNeighborID];

            //     Vector3 direction = 2 * visitedAtom.Molecule.transform.localScale.x * (atom.Position - visitedNeighbor.Position);
            //     instantiatedAtom.Molecule.transform.position = visitedAtom.Molecule.transform.position + direction;

            //     foreach (MolfileBond bond in bondsByID[atomID]) {
            //         int otherAtomID = bond.GetNeighborID(atomID);
            //         if (visitedAtoms.Contains(otherAtomID) || atomQueue.Contains(otherAtomID)) continue;
                
            //         atomQueue.Enqueue(otherAtomID);
            //     }

            //     visitedAtoms.Add(atomID);
            // }

            foreach(MolfileBond bond in molfile.Bonds) {
                Atom atom1 = instantiatedAtoms[bond.Atom1.ID];
                Atom atom2 = instantiatedAtoms[bond.Atom2.ID];
                atom1.Molecule.MergeWith(atom2.Molecule, atom1, atom2, 2 * bond.BondType, true);
                atom2.Molecule.MergeWith(atom1.Molecule, atom2, atom1, 2 * bond.BondType, true);
            }

            Transform moleculeT = transform.GetChild(0);
            foreach (MolfileAtom atom in molfile.Atoms) instantiatedAtoms[atom.ID].transform.SetAsLastSibling();
            foreach (Bond b in GetComponentsInChildren<Bond>()) b.transform.SetAsLastSibling();

            moleculeT.name = molfile.name;
            moleculeT.parent = null;
        }
    }
}
#endif