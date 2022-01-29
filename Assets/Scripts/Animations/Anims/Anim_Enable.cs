using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Veridium_Animation{
    public class Anim_Enable : AnimationBase
    {
        ///<summary>
        /// Template for animations. Duplicate this file to create new animations
        /// Consult the animation README for more info on how to use this template
        ///</summary>
        
        // Called when animation is started
        public override void Play()
        {
            base.Play();

            GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;
        }

        // Called when animation ends
        public override void End()
        {
            base.End();
        }

        // Called when animation is paused
        public override void Pause()
        {
            base.Pause();
        }

        // Called when animation restarts
        protected override void ResetChild()
        {
            base.ResetChild();

            GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;
        }

        // Called every frame while animation is playing
        protected override void UpdateAnim()
        {
            base.UpdateAnim();
        }

    }
}
