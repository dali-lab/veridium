using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium_Interaction;

namespace Veridium_Animation{
    public class Anim_LockStructure : AnimationBase
    {
        public StructureBase structureBase;

        public Anim_LockStructure(){
            duration = 0;
            indefiniteDuration = true;
        }
        
        public override void Play()
        {
            base.Play();
            structureBase.Lock();
        }

        public override void Pause()
        {
            base.Pause();
            structureBase.Unlock();
        }

        protected override void ResetChild()
        {
            base.ResetChild();
            structureBase.Unlock();
        }

        protected override void UpdateAnim()
        {
            base.UpdateAnim();
            if(!structureBase.locked) structureBase.Lock();
        }
    }
}
