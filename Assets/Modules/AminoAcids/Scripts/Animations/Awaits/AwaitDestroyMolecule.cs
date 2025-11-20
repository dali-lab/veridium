using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium.Animation;

namespace Veridium.Modules.AminoAcids
{
    public class AwaitDestroyMolecule : AwaitAny
    {
        public MoleculeDestroyer destroyer;


        public override void Play()
        {
            base.Play();
            if (!destroyer) return;

            destroyer.OnDestroyMolecule.AddListener(OnDestroyMolecule);
        }

        private void OnDestroyMolecule(Molecule molecule)
        {
            CompleteAction();
            destroyer.OnDestroyMolecule.RemoveListener(OnDestroyMolecule);
        }
    }
}
