using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Veridium.Animation;

namespace Veridium.Modules.AminoAcids {
    public class AwaitMirrorMolecule : AwaitAny
    {
        public MoleculeMirror mirror;
        public Molecule moleculeToMirror;

        public override void Play()
        {
            base.Play();
            mirror.OnMirrorMolecule.AddListener(OnMirrorMolecule);
        }

        private void OnMirrorMolecule(Molecule molecule)
        {
            if (moleculeToMirror && molecule != moleculeToMirror) return;
            CompleteAction();
            mirror.OnMirrorMolecule.RemoveListener(OnMirrorMolecule);
        }

        public void SetMoleculeToMirror(Molecule mol)
        {
            moleculeToMirror = mol;
        }
    }
}