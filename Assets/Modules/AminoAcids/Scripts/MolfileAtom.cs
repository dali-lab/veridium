using System;

namespace Veridium.Modules.AminoAcids {
    public class MolfileAtom
    {
        public int ID;
        public Element Element;

        public MolfileAtom(int id, Element element) {
            ID = id;
            Element = element;
        }

        public static MolfileAtom FromString(int id, string str) {
            string[] parts = str.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            return new MolfileAtom(id, (Element)Enum.Parse(typeof(Element), parts[3]));
        }
    }
}
