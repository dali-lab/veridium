using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium.Animation;
using Veridium.Interaction;

namespace Veridium.Modules.ElementStructures
{
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
