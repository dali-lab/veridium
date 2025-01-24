using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Veridium.Modules.AminoAcids {
    public class Molecule : Component {
        public HashSet<Atom> atoms;

        public void MergeWith(Molecule other) {
            if (atoms.Count < other.atoms.Count) return;
            if (atoms.Count == other.atoms.Count && GetInstanceID() < other.GetInstanceID()) return;

            foreach (Atom atom in other.atoms) {
                atoms.Add(atom);
                atom.molecule = this;
            }
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