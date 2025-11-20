using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Veridium.Animation;

namespace Veridium.Modules.AminoAcids {
    public class AwaitSelectAminoAcid : AwaitAny
    {
        public AminoAcidSheet sheet;

        public override void Play()
        {
            base.Play();
            sheet.aminoAcidSelected.AddListener(OnAminoAcidSelected);
        }

        private void OnAminoAcidSelected(PeekableStructuralFormula aminoAcid)
        {
            if (aminoAcid == null) return;

            CompleteAction();
            sheet.aminoAcidSelected.RemoveListener(OnAminoAcidSelected);
        }
    }
}