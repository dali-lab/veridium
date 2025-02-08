using UnityEngine;
using Veridium.Animation;
using Veridium.Interaction;

namespace Veridium.Modules.ElementStructures
{
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
            float highlightTime = Mathf.Min(elapsedTimePercent * 2.0f, 1.0f);
            float blinkTime = 1.0f - Mathf.Abs(highlightTime - 0.5f) * 2.0f;

            float highlightBlend = Easing.EaseOut(elapsedTimePercent, EasingType.Bounce);
            float blinkBlend = Easing.EaseOut(blinkTime, EasingType.Quadratic);

            coordinateSystem.SetAxisHighlightPercent(0, Mathf.Lerp(previousHighlightA ? 1.0f : 0.0f, highlightA ? 1.0f : 0.0f, highlightBlend));
            coordinateSystem.SetAxisHighlightPercent(1, Mathf.Lerp(previousHighlightB ? 1.0f : 0.0f, highlightB ? 1.0f : 0.0f, highlightBlend));
            coordinateSystem.SetAxisHighlightPercent(2, Mathf.Lerp(previousHighlightC ? 1.0f : 0.0f, highlightC ? 1.0f : 0.0f, highlightBlend));

            // only blink when highlighting enabled
            coordinateSystem.SetAxisBlinkPercent(0, !previousHighlightA && highlightA ? blinkBlend: 0.0f);
            coordinateSystem.SetAxisBlinkPercent(1, !previousHighlightB && highlightB ? blinkBlend: 0.0f);
            coordinateSystem.SetAxisBlinkPercent(2, !previousHighlightC && highlightC ? blinkBlend: 0.0f);

            base.UpdateAnim();
        }

    }
}
