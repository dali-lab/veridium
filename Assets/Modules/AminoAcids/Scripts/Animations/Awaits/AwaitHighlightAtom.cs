using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium.Animation;
using System.Linq;

namespace Veridium.Modules.AminoAcids
{
    public enum AtomToHighlight
    {
        Any,
        AlphaCarbon,
        Specific
    }

    public class AwaitHighlightAtom : AwaitAny
    {
        public AtomToHighlight atomToHighlight = AtomToHighlight.Any;
        public List<Atom> optionalAtomsToHighlight;
        public Molecule optionalMoleculeForAlphaC;
        public bool desireHighlightState = true;
        private List<Atom> highlightTargets = new List<Atom>();

        public override void Play()
        {
            base.Play();
            switch (atomToHighlight)
            {
                case AtomToHighlight.Any:
                    highlightTargets = FindObjectsOfType<Atom>().ToList();
                    break;
                case AtomToHighlight.AlphaCarbon:
                    highlightTargets = new List<Atom>() { optionalMoleculeForAlphaC.AlphaCarbon };
                    break;
                case AtomToHighlight.Specific:
                    highlightTargets = optionalAtomsToHighlight;
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

        public void SetMoleculeForAlphaC(Molecule mol)
        {
            optionalMoleculeForAlphaC = mol;
        }
    }
}
