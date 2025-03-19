using UnityEngine;
using UnityEditor.AssetImporters;
using System.IO;
using System.Collections.Generic;
using System;

namespace Veridium.Modules.AminoAcids {
    [ScriptedImporter(1, "mol")]
    public class MolImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            Molfile molfile = ScriptableObject.CreateInstance<Molfile>();
            Dictionary<Element, int> atomCounts = new Dictionary<Element, int>() {
                {Element.C, 0}, {Element.H, 0}, {Element.N, 0}, {Element.O, 0}
            };

            string[] lines = File.ReadAllLines(ctx.assetPath);
            molfile.Name = lines[0];

            string[] countsLise = lines[3].Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            int atomCount = int.Parse(countsLise[0]);
            int bondCount = int.Parse(countsLise[1]);

            int linePointer = 4;

            // Atoms block
            while (linePointer < 4 + atomCount)
            {
                string[] atomLine = lines[linePointer].Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                MolfileAtom atom = MolfileAtom.FromString(linePointer - 3, lines[linePointer]);
                molfile.Atoms.Add(atom);
                int currentCount = atomCounts.ContainsKey(atom.Element) ? atomCounts[atom.Element] : 0;
                atomCounts[atom.Element] = currentCount + 1;

                linePointer++;
            }

            // Bonds block
            while (linePointer < 4 + atomCount + bondCount)
            {
                MolfileBond bond = MolfileBond.FromString(lines[linePointer]);
                molfile.Bonds.Add(bond);

                linePointer++;
            }

            string formula = "";
            foreach (KeyValuePair<Element, int> atom in atomCounts)
            {
                if (atom.Value > 0)
                {
                    formula += atom.Key;
                    if (atom.Value > 1)
                    {
                        formula += atom.Value;
                    }
                }
            }
            molfile.MolecularFormula = formula;

            ctx.AddObjectToAsset("main obj", molfile);
            ctx.SetMainObject(molfile);
        }
    }
}