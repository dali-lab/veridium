using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Veridium.Modules.AminoAcids {
    public class AtomBondingPoint : MonoBehaviour
    {
        [HideInInspector]
        public Atom Atom;
        private Collider bondingPointCollider;

        void Start()
        {
            Atom = GetComponentInParent<Atom>();
            bondingPointCollider = GetComponent<Collider>();
        } 

        public void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out AtomBondingPoint otherBondingPoint)) return;
            if (otherBondingPoint.Atom == Atom) return;

            Atom.Molecule.MergeWith(otherBondingPoint.Atom.Molecule, Atom, otherBondingPoint.Atom);
        }
    }
}