#if UNITY_EDITOR
using UnityEngine;
using UnityEditor.AssetImporters;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem.Interactions;
using UnityEditor;

namespace Veridium.Modules.AminoAcids {
    [ScriptedImporter(1, "mol")]
    public class MolImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            Molfile molfile = ScriptableObject.CreateInstance<Molfile>();

            string[] lines = File.ReadAllLines(ctx.assetPath);
            molfile.Name = lines[0];

            string[] countsLise = lines[3].Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            int atomCount = int.Parse(countsLise[0]);
            int bondCount = int.Parse(countsLise[1]);

            List<MolfileAtom> atomsByID = new List<MolfileAtom>();
            int linePointer = 4;
            //AMK

            // Atoms block
            while (linePointer < 4 + atomCount)
            {
                MolfileAtom atom = MolfileAtom.FromString(linePointer - 3, lines[linePointer]);
                // int currentCount = atomCounts.ContainsKey(atom.Element) ? atomCounts[atom.Element] : 0;
                // atomCounts[atom.Element] = currentCount + 1;

                molfile.Atoms.Add(atom);
                atomsByID.Add(atom);

                linePointer++;
            }

            // Bonds block
            while (linePointer < 4 + atomCount + bondCount)
            {
                MolfileBond bond = MolfileBond.FromString(lines[linePointer], atomsByID);
                molfile.Bonds.Add(bond);

                linePointer++;
            }
            Debug.Log($"Added {molfile.Atoms.Count} atoms and {molfile.Bonds.Count} bonds");

            string formula = "";
            Dictionary<Element, int> atomCounts = new Dictionary<Element, int>();
            foreach (MolfileAtom atom in molfile.Atoms)
            {
                int currentCount = atomCounts.ContainsKey(atom.Element) ? atomCounts[atom.Element] : 0;
                atomCounts[atom.Element] = currentCount + 1;
            }
            foreach (KeyValuePair<Element, int> kvp in atomCounts)
            {
                formula += kvp.Key.ToString();
                if (kvp.Value > 1) formula += kvp.Value;
            }
            molfile.MolecularFormula = formula;

            ctx.AddObjectToAsset("main obj", molfile);
            ctx.SetMainObject(molfile);
        }
    }
}
#endif