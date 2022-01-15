using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIB_Animation{
    public class Anim_Highlight : AnimationBase
    {

        public GameObject highlightObject;

        // Constructor sets indefinite duration 
        public Anim_Highlight(){
            indefiniteDuration = true;
            duration = 0;
        }

        // Ensure that the glow is off by default
        void Awake(){

            Unhighlight();

        }

        // Turn on highlight
        public override void Play(){

            base.Play();

            Highlight();

        }

        // Turn off highlight
        public override void Pause(){

            Reset();

        }

        // Turn off highlight
        protected override void ResetChild(){

            base.ResetChild();

            Unhighlight();

        }

        public void Highlight(){
            highlightObject.SetActive(true);
        }

        public void Unhighlight(){
            if(!playing) highlightObject.SetActive(false);
        }
    }
}
