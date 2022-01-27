using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Veridium_Animation{

    public class Anim_PopUp : AnimationBase
    {

        Vector3 originalPosition;       // Initial position of the gameObject
        Vector3 originalScale;          // Initial scale of the gameObject
        public Vector3 startOffset;     // Beginning offset in local space for the animation
        public Vector3 endOffset;       // Ending offset in local space for the animation


        protected override void UpdateAnim(){

            base.UpdateAnim();

            // Updates the position and scale of the object 
            gameObject.transform.localPosition = originalPosition + Position(elapsedTimePercent);
            gameObject.transform.localScale = originalScale * Scale(elapsedTimePercent);

        }

        public override void Play()
        {

            base.Play();

            // Stores initial transform of the object
            originalPosition = gameObject.transform.localPosition;
            originalScale = gameObject.transform.localScale;

        }

        protected override void ResetChild()
        {

            base.ResetChild();

            // Resets the transform to initial conditions
            gameObject.transform.localPosition = originalPosition;
            gameObject.transform.localScale = originalScale;

            originalPosition = Vector3.zero;
            originalScale = Vector3.one;
        
        }

        // Finds the updated relative position of the object
        private Vector3 Position(float time){
            
            return Vector3.Lerp(startOffset, endOffset, Easing.EaseOut(time, EasingType.Elastic));

        }

        // Finds the updated relative uniform scale of the object
        private float Scale(float time){

            return Easing.EaseOut(time, EasingType.Exponential);

        }
    }
}
