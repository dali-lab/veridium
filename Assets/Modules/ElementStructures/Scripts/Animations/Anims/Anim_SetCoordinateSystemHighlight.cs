using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium.Interaction;

namespace Veridium.Animation{
    public class Anim_SetCoordinateSystemHighlight : AnimationBase
    {
        public GameObject coordinateSystemPrefab;
        public StructureBase structureBase;

        public bool highlightA;
        public bool highlightB;
        public bool highlightC;

        private CoordinateSystemVisualization coordinateSystem;

        private bool previousHighlightA;
        private bool previousHighlightB;
        private bool previousHighlightC;
        
        // Called when animation is started
        public override void Play()
        {
            coordinateSystem = structureBase.GetComponentInChildren<CoordinateSystemVisualization>();
            if (coordinateSystem == null) throw new System.Exception("Coordinate system not found");

            previousHighlightA = coordinateSystem.GetAxisHighlightPercent(0) > 0.5f;
            previousHighlightB = coordinateSystem.GetAxisHighlightPercent(1) > 0.5f;
            previousHighlightC = coordinateSystem.GetAxisHighlightPercent(2) > 0.5f;

            base.Play();
        }

        // Called when animation ends
        public override void End()
        {
            coordinateSystem = null;
            
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
            float blend = Easing.EaseOut(elapsedTimePercent, EasingType.Bounce);

            coordinateSystem.SetAxisHighlightPercent(0, Mathf.Lerp(previousHighlightA ? 1.0f : 0.0f, highlightA ? 1.0f : 0.0f, blend));
            coordinateSystem.SetAxisHighlightPercent(1, Mathf.Lerp(previousHighlightB ? 1.0f : 0.0f, highlightB ? 1.0f : 0.0f, blend));
            coordinateSystem.SetAxisHighlightPercent(2, Mathf.Lerp(previousHighlightC ? 1.0f : 0.0f, highlightC ? 1.0f : 0.0f, blend));

            base.UpdateAnim();
        }

    }
}
