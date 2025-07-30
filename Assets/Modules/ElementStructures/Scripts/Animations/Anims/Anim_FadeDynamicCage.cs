using System.Collections.Generic;
using UnityEngine;
using Veridium.Animation;

namespace Veridium.Modules.ElementStructures
{
    public class Anim_FadeDynamicCage : AnimationBase
    {

        public float startingThicknessHighlighted = 0f;
        public float endingThicknessHighlighted = 1f;
        public float startingThicknessRegular = 0f;
        public float endingThicknessRegular = 1f;
        public EasingType easingType = EasingType.Linear;

        public DynamicCageHighlight dynamicCageHighlight;



        // Constructor
        public Anim_FadeDynamicCage()
        {
            duration = 1f;
        }

        // Called when started
        public override void Play()
        {
            base.Play();
        }

        // Called when paused
        public override void Pause()
        {
            base.Pause();
        }

        // Called every frame while playing
        protected override void UpdateAnim()
        {
            base.UpdateAnim();

            // Alpha value is opacity. Updated each frame
            float thicknessHihlighted = (endingThicknessHighlighted - startingThicknessHighlighted) * Easing.EaseFull(elapsedTimePercent, easingType) + startingThicknessHighlighted;
            float thicknessRegular = (endingThicknessRegular - startingThicknessRegular) * Easing.EaseFull(elapsedTimePercent, easingType) + startingThicknessRegular;

            dynamicCageHighlight.highlightedWidth = thicknessHihlighted;
            dynamicCageHighlight.notHighlightWidth = thicknessRegular;
            
            dynamicCageHighlight.UpdateCageHighlight(Matrix4x4.identity);
        }

        public override void End()
        {
            //gameObject.GetComponent<Renderer>().materials[materialIndex].DisableKeyword("_ALPHABLEND_ON");
        }

        // Called when restarted
        protected override void ResetChild()
        {
            base.ResetChild();
        }
    }
}

