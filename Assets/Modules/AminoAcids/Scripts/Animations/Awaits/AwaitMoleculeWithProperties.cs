using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium.Animation;

namespace Veridium.Modules.AminoAcids
{
    public class AwaitMoleculeWithProperties : AwaitAny
    {
        [Range(0, 20)]
        public int AtomsCount = 2;

        public override void Play()
        {
            base.Play();
            print("Awaiting molecule with properties");
            MoleculeManager.Instance.OnMoleculeChanged.AddListener(MoleculeChanged);
        }

        private void MoleculeChanged(Molecule molecule)
        {
            if (!molecule) return;

            if (molecule.atoms.Count != AtomsCount) return;

            CompleteAction();
            MoleculeManager.Instance.OnMoleculeChanged.RemoveListener(MoleculeChanged);
        }
    }
}