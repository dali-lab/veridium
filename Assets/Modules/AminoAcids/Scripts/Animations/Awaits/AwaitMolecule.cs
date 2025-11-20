using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Veridium.Animation;
using UnityEngine.Events;

namespace Veridium.Modules.AminoAcids {
    public class AwaitMolecule : AwaitAny
    {
        public Molfile Molfile;
        public UnityEvent<Molecule> OnMoleculeBuilt;

        protected override void Start()
        {
            base.Start();
            // OnMoleculeMatched = new UnityEvent<Dictionary<MolfileAtom, Atom>>();
        }

        public override void Play()
        {
            base.Play();
            print("Awaiting molecule " + Molfile.Name);
            MoleculeManager.Instance.OnMoleculeChanged.AddListener(MoleculeChanged);
        }

        public override void PlayFromStart()
        {
            MoleculeManager.Instance.OnMoleculeChanged.RemoveListener(MoleculeChanged);
            base.PlayFromStart();
        }

        public new void Reset()
        {
            MoleculeManager.Instance.OnMoleculeChanged.RemoveListener(MoleculeChanged);
            base.Reset();
        }

        private void MoleculeChanged(Molecule molecule)
        {
            if (!Molfile) return;

            Dictionary<MolfileAtom, Atom> dict = CheckMoleculeEquality(molecule);
            if (dict == null) return;

            Debug.LogWarning("Molecule is equal to molfile molecule");
            // OnMoleculeMatched.Invoke(dict);

            Molecule prefab = FindObjectsOfType<MoleculeSpawner>().First(s => s.MoleculePrefab.name == Molfile.name).MoleculePrefab;
            Molecule instance = Instantiate(prefab, molecule.transform.position, molecule.transform.rotation);
            instance.transform.localScale = molecule.transform.lossyScale;
            Destroy(molecule.gameObject);
            instance.Mergeable = true;

            OnMoleculeBuilt.Invoke(instance);

            CompleteAction();
        }

        [ContextMenu("Complete")]
        public void DebugComplete()
        {
            Molecule prefab = FindObjectsOfType<MoleculeSpawner>().First(s => s.MoleculePrefab.name == Molfile.name).MoleculePrefab;
            Molecule instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            instance.Mergeable = true;

            OnMoleculeBuilt.Invoke(instance);

            CompleteAction();
        }

        // [ContextMenu("Find Alanine")]
        // public void FindAlanine()
        // {
        //     GameObject go = FindObjectsOfType<MoleculeSpawner>().First(s => s.MoleculePrefab.name == "Alanine").gameObject;
        //     Debug.Log("Found Alanine", go);
        //     print(Molfile.name);
        //     print(Molfile.name == "Alanine");
        // }

        private Dictionary<MolfileAtom, Atom> CheckMoleculeEquality(Molecule molecule)
        {
            // Algorithm to determine if the molecule is equal to the molecule from the molfile
            // 1. Check if the molecule has the same number of atoms and bonds
            if (molecule.atoms.Count != Molfile.Atoms.Count) return null;// $"Different number of atoms ({molecule.atoms.Count} in molecule, {Molfile.Atoms.Count} in molfile)";
            if (molecule.GetComponentsInChildren<Bond>().Length != Molfile.Bonds.Count) return null;// $"Different number of bonds ({molecule.GetComponentsInChildren<Bond>().Length} in molecule, {Molfile.Bonds.Count} in molfile)";

            // 2. Check if the atoms and their bonds and neighbors are the same
            HashSet<MolfileAtom> visitedAtoms = new HashSet<MolfileAtom>();
            HashSet<MolfileBond> visitedBonds = new HashSet<MolfileBond>();
            bool atomPredicate(MolfileAtom ma, Atom a) => !visitedAtoms.Contains(ma) && ma.Element == a.element;
            bool bondPredicate(MolfileBond mb, Bond b) => !visitedBonds.Contains(mb) && mb.ConnectsElements(b.atom1.element, b.atom2.element);

            Dictionary<MolfileAtom, Atom> dict = new Dictionary<MolfileAtom, Atom>();

            foreach (Atom atom in molecule.atoms)
            {
                List<MolfileAtom> atomCandidates = Molfile.Atoms.Where(ma => atomPredicate(ma, atom)).ToList();
                if (atomCandidates.Count == 0) return null;// $"Element not found in molfile ({atom.element})";
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
                    foreach (Bond bond in atom.Bonds)
                    {
                        MolfileBond correspondingBond = correspondingBonds.FirstOrDefault(mb => bondPredicate(mb, bond));
                        // Debug.Log($"For bond {bond.atom1.element}-{bond.atom2.element}, found corresponding bond {correspondingBond?.Atom1.Element}-{correspondingBond?.Atom2.Element}");
                        if (correspondingBond == null) { nextCandidate = true; break; }
                        visitedBonds.Add(correspondingBond);
                    }

                    if (nextCandidate) continue;

                    foundCorrespondingAtom = true;
                    visitedAtoms.Add(candidate);
                    dict.Add(candidate, atom);
                    break;
                }

                if (!foundCorrespondingAtom) return null;// $"Atom {atom.element} not found in molfile";
            }

            // 3. If all atoms and bonds are the same, the molecule is equal to the molfile molecule
            return dict;
        }
    }
}