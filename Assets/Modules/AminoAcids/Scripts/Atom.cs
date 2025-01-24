using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Veridium.Modules.AminoAcids {
    public class Atom : Component
    {
        public Element element;
        public int AtomicNumber => element.ToAtomicNumber();
        public string Name;
        public int NaturalValenceElectrons;
        public int MaxValenceElectrons => element == Element.H || element == Element.He ? 2 : 8;
        public int CurrentValenceElectrons => NaturalValenceElectrons - bonds.Count + bonds.Sum(bond => bond.electrons);
        public HashSet<Bond> bonds;
        public HashSet<Atom> Neighbors => new HashSet<Atom>(bonds.Select(bond => bond.Other(this)));
        public Molecule molecule;

        public void TransferTo(Molecule other) {
            if (other == null) {
                GameObject go = new GameObject("Molecule");
                go.transform.SetPositionAndRotation(transform.position, transform.rotation);
                other = go.AddComponent<Molecule>();
            }

            molecule.atoms.Remove(this);
            other.AddAtom(this);
        }

        public void BondWith(Atom other) {
            if (Neighbors.Contains(other)) return;

            GameObject go = new GameObject("Bond");
            Bond bond = go.AddComponent<Bond>();
            bond.Create(this, other);
        }

        public bool TryGetBondTo(Atom other, out Bond bond) {
            bond = null;
            if (!Neighbors.Contains(other)) return false;
            bond = bonds.First(bond => bond.Other(this) == other);
            return true;
        }
    }
}