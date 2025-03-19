using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Veridium.Modules.AminoAcids {
    public class Atom : MonoBehaviour
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

        void Start() {
            molecule = GetComponentInParent<Molecule>();
            bonds = new HashSet<Bond>(molecule.GetComponentsInChildren<Bond>().Where(b => b.atom1 == this || b.atom2 == this));
        }

        void Update()
        {
            transform.LookAt(Camera.main.transform);
        }

        public void BondWith(Atom other) {
            if (Neighbors.Contains(other)) return;

            GameObject go = new GameObject("Bond");
            Bond bond = go.AddComponent<Bond>();
            bond.Create(this, other);
        }

        // public bool TryGetBondTo(Atom other, out Bond bond) {
        //     bond = null;
        //     if (!Neighbors.Contains(other)) return false;
        //     bond = bonds.First(bond => bond.Other(this) == other);
        //     return true;
        // }

        public void OnTriggerEnter(Collider collider) {
            if (!collider.TryGetComponent(out Atom otherAtom)) return;

            molecule.MergeWith(otherAtom.molecule, this, otherAtom);
        }
    }
}