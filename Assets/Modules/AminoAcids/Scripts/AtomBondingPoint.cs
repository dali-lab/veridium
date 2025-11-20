using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Veridium.Modules.AminoAcids {
    public class AtomBondingPoint : MonoBehaviour
    {
        [HideInInspector]
        public Atom Atom;
        private Dictionary<AtomBondingPoint, PreviewBond> previewBonds;

        void Start()
        {
            Atom = GetComponentInParent<Atom>();
            previewBonds = new Dictionary<AtomBondingPoint, PreviewBond>();
            Atom.Molecule.grabInteractable.lastSelectExited.AddListener(ThisMoleculeDeselected);
        }

        private void ThisMoleculeDeselected(SelectExitEventArgs _)
        {
            foreach (var kvp in previewBonds)
            {
                kvp.Key.Atom.Molecule.grabInteractable.lastSelectExited.RemoveListener(OtherMoleculeDeselected);
                Destroy(kvp.Value.gameObject);
            }
            previewBonds.Clear();
        }

        private void OtherMoleculeDeselected(SelectExitEventArgs _)
        {
            foreach (var kvp in previewBonds)
            {
                kvp.Key.Atom.Molecule.grabInteractable.lastSelectExited.RemoveListener(OtherMoleculeDeselected);
                Atom.Molecule.MergeWith(kvp.Key.Atom.Molecule, Atom, kvp.Key.Atom, kvp.Value.electrons);
                Destroy(kvp.Value.gameObject);
            }
            previewBonds.Clear();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out AtomBondingPoint otherBondingPoint)) return;
            if (otherBondingPoint.Atom == Atom) return;
            if (!Atom.Molecule.Mergeable || !otherBondingPoint.Atom.Molecule.Mergeable) return;
            if (!Atom.Molecule.grabInteractable.isSelected && !otherBondingPoint.Atom.Molecule.grabInteractable.isSelected) return;
            if (previewBonds.ContainsKey(otherBondingPoint)) return;

            otherBondingPoint.Atom.Molecule.grabInteractable.lastSelectExited.AddListener(OtherMoleculeDeselected);
            UpdateNumberOfPreviewBonds(otherBondingPoint);
            // BondAtoms(null);
        }

        public void OnTriggerStay(Collider other)
        {
            if (!other.TryGetComponent(out AtomBondingPoint otherBondingPoint)) return;
            if (!previewBonds.ContainsKey(otherBondingPoint)) return;

            UpdateNumberOfPreviewBonds(otherBondingPoint);
        }

        public void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out AtomBondingPoint otherBondingPoint)) return;
            if (otherBondingPoint.Atom == Atom) return;
            // if (!Atom.Molecule.grabInteractable.isSelected) return;
            if (!previewBonds.ContainsKey(otherBondingPoint)) return;

            otherBondingPoint.Atom.Molecule.grabInteractable.lastSelectExited.RemoveListener(OtherMoleculeDeselected);
            Destroy(previewBonds[otherBondingPoint].gameObject);
            previewBonds.Remove(otherBondingPoint);
        }

        private void UpdateNumberOfPreviewBonds(AtomBondingPoint other)
        {
            int bondStrength = GetBondStrengthForDistanceTo(other);

            if (previewBonds.ContainsKey(other))
            {
                if (previewBonds[other].electrons == bondStrength)
                {
                    previewBonds[other].UpdateTransform();
                    return;
                }

                Destroy(previewBonds[other].gameObject);
            }

            previewBonds[other] = PreviewBond.Create(Atom, other.Atom, bondStrength);
        }

        private int GetBondStrengthForDistanceTo(AtomBondingPoint other)
        {
            int maxElectrons = Mathf.Min(Atom.MaxValenceElectrons - Atom.CurrentValenceElectrons, other.Atom.MaxValenceElectrons - other.Atom.CurrentValenceElectrons);
            float maxDistance = transform.lossyScale.x + other.transform.lossyScale.x;
            float distance = Vector3.Distance(Atom.transform.position, other.Atom.transform.position) / maxDistance;
            if (distance < 0.4f) return Mathf.Min(4, maxElectrons);
            if (distance < 0.6f) return Mathf.Min(3, maxElectrons);
            if (distance < 0.8f) return Mathf.Min(2, maxElectrons);
            return Mathf.Min(1, maxElectrons);
        }
    }
}