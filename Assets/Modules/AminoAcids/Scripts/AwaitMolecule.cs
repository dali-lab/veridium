using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Veridium.Animation;

namespace Veridium.Modules.AminoAcids {
    public class AwaitMolecule : AwaitAny
    {
        public Molfile Molfile;

        public override void Play()
        {
            base.Play();
            print("Awaiting molecule " + Molfile.Name);
            MoleculeManager.Instance.OnMoleculeChanged.AddListener(MoleculeChanged);
        }

        private void MoleculeChanged(Molecule molecule)
        {
            if (!Molfile) return;

            string message = CheckMoleculeEquality(molecule);
            if (message == "") {
                print("Molecule is equal to molfile molecule");
            } else {
                print(message);
                return;
            }

            CompleteAction();
        }

        private string CheckMoleculeEquality(Molecule molecule) {
            // Algorithm to determine if the molecule is equal to the molecule from the molfile
            // 1. Check if the molecule has the same number of atoms and bonds
            if (molecule.atoms.Count != Molfile.Atoms.Count) return $"Different number of atoms ({molecule.atoms.Count} in molecule, {Molfile.Atoms.Count} in molfile)";
            if (molecule.GetComponentsInChildren<Bond>().Length != Molfile.Bonds.Count) return $"Different number of bonds ({molecule.GetComponentsInChildren<Bond>().Length} in molecule, {Molfile.Bonds.Count} in molfile)";

            // 2. Check if the atoms and their bonds and neighbors are the same
            HashSet<MolfileAtom> visitedAtoms = new HashSet<MolfileAtom>();
            HashSet<MolfileBond> visitedBonds = new HashSet<MolfileBond>();
            bool atomPredicate(MolfileAtom ma, Atom a) => !visitedAtoms.Contains(ma) && ma.Element == a.element;
            bool bondPredicate(MolfileBond mb, Bond b) => !visitedBonds.Contains(mb) && mb.ConnectsElements(b.atom1.element, b.atom2.element);

            foreach (Atom atom in molecule.atoms)
            {
                List<MolfileAtom> atomCandidates = Molfile.Atoms.Where(ma => atomPredicate(ma, atom)).ToList();
                if (atomCandidates.Count == 0) return $"Element not found in molfile ({atom.element})";
                // Debug.Log($"For atom {atom.element}, found {atomCandidates.Count} candidates");
                bool foundCorrespondingAtom = false;
                foreach (MolfileAtom candidate in atomCandidates)
                {
                    bool nextCandidate = false;

                    if (visitedAtoms.Contains(candidate)) continue;
                    List<MolfileBond> correspondingBonds = Molfile.Bonds.Where(mb => mb.Atom1.ID == candidate.ID || mb.Atom2.ID == candidate.ID).ToList();
                    // Debug.Log($"For atom {atom.element}, found {correspondingBonds.Count} corresponding bonds");
                    if (correspondingBonds.Count != atom.Bonds.Count) continue;

                    visitedBonds.Clear();
                    foreach (Bond bond in atom.Bonds) {
                        MolfileBond correspondingBond = correspondingBonds.FirstOrDefault(mb => bondPredicate(mb, bond));
                        // Debug.Log($"For bond {bond.atom1.element}-{bond.atom2.element}, found corresponding bond {correspondingBond?.Atom1.Element}-{correspondingBond?.Atom2.Element}");
                        if (correspondingBond == null) { nextCandidate = true; break; }
                        visitedBonds.Add(correspondingBond);
                    }

                    if (nextCandidate) continue;

                    foundCorrespondingAtom = true;
                    visitedAtoms.Add(candidate);
                    break;
                }

                if (!foundCorrespondingAtom) return $"Atom {atom.element} not found in molfile";
            }

            // 3. If all atoms and bonds are the same, the molecule is equal to the molfile molecule
            return "";
        }
    }
}