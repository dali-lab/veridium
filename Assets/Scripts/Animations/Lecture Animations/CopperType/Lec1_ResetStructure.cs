using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SIB_Interaction;

namespace SIB_Animation{
    public class Lec1_ResetStructure : AnimationBase
    {

        public ElementLoader elementLoader;
        
        public override void Play()
        {
            base.Play();
            elementLoader.ResetStructure();
        }

        public override void End()
        {
            base.End();
        }

        public override void Pause()
        {
            base.Pause();
        }

        protected override void ResetChild()
        {
            base.ResetChild();
        }

        protected override void UpdateAnim()
        {
            base.UpdateAnim();
        }
    }
}
