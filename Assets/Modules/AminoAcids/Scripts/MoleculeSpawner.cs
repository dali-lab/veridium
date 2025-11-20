using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Veridium.Modules.AminoAcids
{
    public class MoleculeSpawner : MonoBehaviour
    {
        public Molecule MoleculePrefab;
        private Molecule molecule;
        private int collidingAtoms = 0;

        void Start()
        {
            RespawnMolecule();
        }

        private void RespawnMolecule()
        {
            if (!MoleculePrefab) return;
            molecule = Instantiate(MoleculePrefab, transform.position, transform.rotation, transform);
            molecule.Mergeable = false;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!other.attachedRigidbody.TryGetComponent(out Molecule molecule)) return;
            if (molecule != this.molecule) return;
            collidingAtoms++;
        }

        public void OnTriggerExit(Collider other)
        {
            if (!other.attachedRigidbody.TryGetComponent(out Molecule molecule)) return;
            if (molecule != this.molecule) return;

            collidingAtoms--;
            if (collidingAtoms > 0) return;

            molecule.Mergeable = true;
            molecule.transform.SetParent(null, true);
            RespawnMolecule();
        }
    }
}