using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Veridium.Modules.AminoAcids {
    public class Molfile : ScriptableObject
    {
        public string Name;
        public string MolecularFormula;
        public List<MolfileAtom> Atoms = new List<MolfileAtom>();
        public List<MolfileBond> Bonds = new List<MolfileBond>();
    }
}