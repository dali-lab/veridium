using System;
using UnityEngine;

namespace Veridium.Modules.AminoAcids {
    [Serializable]
    public class MolfileAtom
    {
        public int ID;
        public Element Element;
        public Vector3 Position;

        public MolfileAtom(int id, Element element, Vector3 position) {
            ID = id;
            Element = element;
            Position = position;
        }

        public static MolfileAtom FromString(int id, string str) {
            string[] parts = str.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

            Element element = (Element)Enum.Parse(typeof(Element), parts[3]);

            float posX = float.Parse(parts[0]);
            float posY = float.Parse(parts[1]);
            float posZ = float.Parse(parts[2]);
            Vector3 position = new Vector3(posX, posY, posZ);

            return new MolfileAtom(id, element, position);
        }
    }
}
