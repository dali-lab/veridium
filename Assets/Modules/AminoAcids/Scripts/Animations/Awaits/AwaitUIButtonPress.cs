using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Veridium.Animation;

namespace Veridium.Modules.AminoAcids {
    public class AwaitUIButtonPress : AwaitAny
    {
        [SerializeField] private Button button;
        [SerializeField] private AwaitMolecule awaitMolecule;

        public override void Play()
        {
            base.Play();
            button.onClick.AddListener(OnButtonPressed);
        }

        private void OnButtonPressed()
        {
            button.onClick.RemoveListener(OnButtonPressed);
            if (awaitMolecule) awaitMolecule.Molfile = FindObjectOfType<AminoAcidSheet>().selectedAminoAcid.molfile;
            Debug.LogWarning(awaitMolecule.Molfile.name);
            CompleteAction();
        }
    }
}