using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium_Interaction;

namespace Veridium_Animation{
    public class Anim_HideCoordinateSystem : AnimationBase
    {
        public GameObject coordinateSystemPrefab;
        public StructureBase structureBase;

        private CoordinateSystemVisualization coordinateSystem;
        
        // Called when animation is started
        public override void Play()
        {
            coordinateSystem = structureBase.GetComponentInChildren<CoordinateSystemVisualization>();
            if (coordinateSystem == null) throw new System.Exception("Coordinate system not found");

            base.Play();
        }

        // Called when animation ends
        public override void End()
        {
            coordinateSystem.SetFadePercent(0.0f);
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
        }

        // Called every frame while animation is playing
        protected override void UpdateAnim()
        {
            float opacity = Easing.EaseFull(elapsedTimePercent, EasingType.Quadratic);
            coordinateSystem.SetFadePercent(1.0f - opacity);

            base.UpdateAnim();
        }

    }
}
