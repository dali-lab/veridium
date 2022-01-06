using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SIB_Interaction;
using sib;
using System.Linq;

namespace SIB_Animation{
    public class Anim_GlowAtoms : AnimationBase
    {

        public StructureBase structure;
        private List<Atom> atoms;

        public Anim_GlowAtoms(){
            indefiniteDuration = true;
        }

        public override void Play()
        {
            base.Play();

            atoms.Clear();

            atoms = structure.structureBuilder.crystal.atoms.Values.ToList();

            foreach (Atom atom in atoms)
            {
                atom.drawnObject.GetComponentInChildren<Anim_GlowPulse>().Play();
            }
        }

        public override void Pause()
        {
            base.Pause();

            foreach (Atom atom in atoms)
            {
                atom.drawnObject.GetComponentInChildren<Anim_GlowPulse>().Pause();
            }
        }

        protected override void ResetChild()
        {
            base.ResetChild();

            foreach (Atom atom in atoms)
            {
                atom.drawnObject.GetComponentInChildren<Anim_GlowPulse>().Reset();
            }
        }

        protected override void UpdateAnim()
        {
            base.UpdateAnim();
        }

    }
}
