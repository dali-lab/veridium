using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium.Animation;
using System.Linq;

namespace Veridium.Modules.AminoAcids
{
    public enum BondToHighlight
    {
        Any,
        Specific,
        AABondCToR
    }

    public class AwaitHighlightBond : AwaitAny
    {
        public BondToHighlight bondToHighlight = BondToHighlight.Any;
        public List<Bond> optionalBondsToHighlight;
        public Molecule optionalMoleculeForBondToR;
        private List<Bond> highlightTargets = new List<Bond>();
        public bool desireHighlightState = true;

        public override void Play()
        {
            base.Play();

            switch (bondToHighlight)
            {
                case BondToHighlight.Any:
                    highlightTargets = FindObjectsOfType<Bond>().ToList();
                    break;
                case BondToHighlight.Specific:
                    highlightTargets = optionalBondsToHighlight;
                    break;
                case BondToHighlight.AABondCToR:
                    highlightTargets = new List<Bond>() { optionalMoleculeForBondToR.AlphaCToR };
                    break;
            }

            highlightTargets.ForEach(atom => atom.OnToggleHighlight.AddListener(OnToggleHighlight));
        }

        private void OnToggleHighlight(bool highlighted)
        {
            if (highlighted != desireHighlightState) return;

            highlightTargets.ForEach(atom => atom.OnToggleHighlight.RemoveListener(OnToggleHighlight));
            CompleteAction();
        }

        public void SetMoleculeForBond(Molecule mol)
        {
            optionalMoleculeForBondToR = mol;
        }
    }
}
