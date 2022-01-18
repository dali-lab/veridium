using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIB_Animation{
    public class Anim_Fade : AnimationBase
    {
        /// <summary>
        /// Fades in or out the opacity of a material 
        /// Should be added to a GameObject with a renderer that has a color and translucent workflow
        /// </summary>

        public float startingOpacity = 0f;                                  // Opacity to start from
        public float endingOpacity = 1f;                                    // Opacity to end at
        public Easing.EasingType easingType = Easing.EasingType.Linear;     // Easing type for the curve between start and end
        public int materialIndex = 0;                                       // The index of the material to apply the fading effect to

        // Constructor
        public Anim_Fade(){
            duration = 1f;
        }

        // Called when started
        public override void Play(){
            base.Play();
            Color color = gameObject.GetComponent<Renderer>().materials[materialIndex].color;
            color.a = startingOpacity;
            gameObject.GetComponent<Renderer>().materials[materialIndex].color = color;
        }

        // Called when paused
        public override void Pause(){
            base.Pause();
        }

        // Called every frame while playing
        protected override void UpdateAnim(){
            base.UpdateAnim();

            // Alpha value is opacity. Updated each frame
            float alpha = (endingOpacity - startingOpacity) * Easing.EaseFull(elapsedTimePercent, easingType) + startingOpacity;

            // Extract the color from the material and update the opacity
            Color color = gameObject.GetComponent<Renderer>().materials[materialIndex].color;
            color.a = alpha;
            gameObject.GetComponent<Renderer>().materials[materialIndex].color = color;
        }

        // Called when restarted
        protected override void ResetChild(){
            base.ResetChild();
        }

        
    }
}
