using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using Oculus.Interaction;

namespace Veridium.Modules.AminoAcids {
    public enum BondManipulationMode
    {
        Cycle,
        Single,
        Double,
        Triple,
        Destroy
    }

    public class BondManipulator : MonoBehaviour
    {
        private BondManipulationMode CurrentMode = BondManipulationMode.Cycle;
        public TextMeshPro modeText;
        public UnityEvent<Atom, Atom, int, int> alteredBond;
        private XRGrabInteractable interactable;

        void Start()
        {
            interactable = GetComponent<XRGrabInteractable>();
            interactable.activated.AddListener(Activated);
            modeText.text = CurrentMode.ToString();
        }

        void OnTriggerEnter(Collider other)
        {
            Bond bond = other.GetComponentInParent<Bond>();
            if (!bond) return;
            ManipulateBond(bond);
        }

        private void ManipulateBond(Bond bond)
        {
            int oldElectrons = bond.electrons;
            int newElectrons = 0;
            Atom atom1 = bond.atom1;
            Atom atom2 = bond.atom2;

            switch (CurrentMode)
            {
                case BondManipulationMode.Destroy:
                    bond.Destroy();
                    break;

                case BondManipulationMode.Single:
                case BondManipulationMode.Double:
                case BondManipulationMode.Triple:
                    newElectrons = CurrentMode switch
                    {
                        BondManipulationMode.Single => 1,
                        BondManipulationMode.Double => 2,
                        BondManipulationMode.Triple => 3,
                        _ => 1
                    };
                    if (bond.atom1.HasFullValence(newElectrons - bond.electrons) || bond.atom2.HasFullValence(newElectrons - bond.electrons))
                    {
                        Debug.LogWarning($"Cannot change bond to {newElectrons} electrons, as one of the atoms has a full valence.");
                        return;
                    }
                    bond.ChangeElectrons(newElectrons);
                    break;

                case BondManipulationMode.Cycle:
                    newElectrons = (bond.electrons % 3) + 1;
                    if (bond.atom1.HasFullValence(newElectrons - bond.electrons) || bond.atom2.HasFullValence(newElectrons - bond.electrons))
                    {
                        if (bond.electrons == 1)
                        {
                            Debug.LogWarning($"Cannot change bond to {newElectrons} electrons, as one of the atoms has a full valence.");
                            break;
                        }

                        newElectrons = 1;
                    }
                    bond.ChangeElectrons(newElectrons);
                    break;

                default:
                    break;
            }

            alteredBond.Invoke(atom1, atom2, oldElectrons, newElectrons);
        }

        private void Activated(ActivateEventArgs args)
        {
            CurrentMode = (BondManipulationMode)(((int)CurrentMode + 1) % System.Enum.GetValues(typeof(BondManipulationMode)).Length);
            modeText.text = CurrentMode.ToString();
        }

        [ContextMenu("Switch to Destroy")]
        public void SwitchToDestroy()
        {
            CurrentMode = BondManipulationMode.Destroy;
            modeText.text = CurrentMode.ToString();
        }
    }
}
