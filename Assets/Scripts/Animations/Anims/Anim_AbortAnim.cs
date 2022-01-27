using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Veridium_Animation{
    public class Anim_AbortAnim : AnimationBase
    {

        ///<summary>
        /// immediately ends another animation.
        ///</summary>
        
        public AnimationBase animationToAbort;          // This animation will be aborted if the animation is playing

        // Constructor
        public Anim_AbortAnim(){
            duration = 0.5f;
        }

        // When this animation plays, end the target animation
        public override void Play(){
            base.Play();

            if(animationToAbort.playing){
                animationToAbort.Pause();
                animationToAbort.End();
            }
        }

        // Resetting this animation plays the aborted animation
        protected override void ResetChild()
        {
            base.ResetChild();

            animationToAbort.Play();
        }
    }
}
