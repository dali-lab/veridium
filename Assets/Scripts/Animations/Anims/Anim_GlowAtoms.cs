using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium.Interaction;
using Veridium.Core;
using System.Linq;

namespace Veridium.Animation{
    public class Anim_GlowAtoms : AnimationBase
    {

        public StructureBase structure;
        private List<Anim_GlowPulse> anims;

        public Anim_GlowAtoms(){
            indefiniteDuration = true;
            duration = 0;
            anims = new List<Anim_GlowPulse>();
        }

        public override void Play()
        {
            base.Play();

            foreach (Atom atom in structure.structureBuilder.crystal.atoms.Values)
            {
                if(atom.drawnObject == null) continue;
                
                Anim_GlowPulse anim = atom.drawnObject.AddComponent<Anim_GlowPulse>() as Anim_GlowPulse;
                anim.emissionColor = new Color(1,1,0);
                anim.maxIntensity = 0.4f;
                anim.Play();
                anims.Add(anim);
            }
        }

        public override void Pause()
        {
            base.Pause();
            
            foreach (Anim_GlowPulse anim in anims)
            {
                anim.Pause();
                anim.selfDestruct = true;
            }
            anims.Clear();
        }


        protected override void ResetChild()
        {
            base.ResetChild();

            foreach (Anim_GlowPulse anim in anims)
            {
                anim.Pause();
                anim.selfDestruct = true;
            }
            anims.Clear();
        }

        protected override void UpdateAnim()
        {
            base.UpdateAnim();
        }

    }
}
