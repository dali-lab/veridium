using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Veridium.Modules.AminoAcids {
    public class MoleculeManager : MonoBehaviour
    {
        public static MoleculeManager Instance;
        public UnityEvent<Molecule> OnMoleculeChanged = new UnityEvent<Molecule>();
        public Molecule EmptyMoleculePrefab;

        void Start()
        {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
            }
        }

        public void UpdateMolecule(Molecule molecule)
        {
            OnMoleculeChanged.Invoke(molecule);
        }
    }
}