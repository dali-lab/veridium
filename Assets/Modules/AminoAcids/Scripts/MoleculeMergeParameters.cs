using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;

namespace Veridium.Modules.AminoAcids
{
    public class MoleculeMergeInfo
    {
        public Molecule OtherMolecule;
        public Atom AtomThis;
        public Atom AtomOther;
        public int Electrons;
        public List<IXRSelectInteractor> PausedInteractors;
    }
}