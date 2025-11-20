using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Veridium.Animation;

namespace Veridium.Modules.AminoAcids {
    public class AwaitBondChange : AwaitAny
    {
        public BondManipulator bondManipulator;
        [Range(-1, 4)] [Tooltip("-1 means any")]
        public int bondStrengthBefore = -1; // -1 means any
        [Range(-1, 4)] [Tooltip("-1 means any")]
        public int bondStrengthAfter = -1; // -1 means any

        public override void Play()
        {
            base.Play();
            bondManipulator.alteredBond.AddListener(OnBondChanged);
        }

        private void OnBondChanged(Atom atom1, Atom atom2, int oldElectrons, int newElectrons)
        {
            if (bondStrengthBefore != -1 && oldElectrons != bondStrengthBefore) return;
            if (bondStrengthAfter != -1 && newElectrons != bondStrengthAfter) return;

            CompleteAction();
        }
    }
}