using System;

namespace Veridium.Modules.AminoAcids {
    public class MolfileBond
    {
        public int Atom1ID;
        public int Atom2ID;
        public int BondType;
    
        public MolfileBond(int atom1ID, int atom2ID, int bondType) {
            Atom1ID = atom1ID;
            Atom2ID = atom2ID;
            BondType = bondType;
        }

        public static MolfileBond FromString(string str) {
            string[] parts = str.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            return new MolfileBond(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
        }
    }
}
