using System;
using System.Collections.Generic;

namespace Veridium.Modules.AminoAcids {
    [Serializable]
    public class MolfileBond
    {
        public MolfileAtom Atom1;
        public MolfileAtom Atom2;
        public int BondType;
    
        public MolfileBond(MolfileAtom atom1, MolfileAtom atom2, int bondType) {
            Atom1 = atom1;
            Atom2 = atom2;
            BondType = bondType;
        }

        public static MolfileBond FromString(string str, List<MolfileAtom> atomsByID) {
            string[] parts = str.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            return new MolfileBond(atomsByID[int.Parse(parts[0]) - 1], atomsByID[int.Parse(parts[1]) - 1], int.Parse(parts[2]));
        }

        public MolfileAtom GetNeighbor(MolfileAtom atom) {
            if (Atom1 == atom) return Atom2;
            if (Atom2 == atom) return Atom1;
            return null;
        }

        public bool ConnectsElements(Element element1, Element element2) {
            return (Atom1.Element == element1 && Atom2.Element == element2) || (Atom1.Element == element2 && Atom2.Element == element1);
        }
    }
}
