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

        public void OnTriggerExit(Collider other)
        {
            if (!other.attachedRigidbody.TryGetComponent(out Molecule molecule)) return;
            if (molecule != this.molecule) return;
            molecule.Mergeable = true;

            RespawnMolecule();
        }
    }
}