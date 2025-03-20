using UnityEngine;
using Veridium.Animation;
using Veridium.Interaction;

namespace Veridium.Modules.ElementStructures
{
    public class Anim_DrawArrow : AnimationBase
    {
        public StructureBase structureBase;

        public Vector3 from;
        public Vector3 to;
        public Color color;
        public string label;
        
        private CoordinateSystemVisualization coordinateSystem;

        private ArrowVisualization arrow;

        private int axisIndex = -1;
        
        // Called when animation is started
        public override void Play()
        {
            coordinateSystem = structureBase.GetComponentInChildren<CoordinateSystemVisualization>();
            if (coordinateSystem == null) throw new System.Exception("Coordinate system not found");

            arrow = coordinateSystem.DrawArrow(from, to, color, label);
            axisIndex = coordinateSystem.drawnArrows.Count - 1;

            coordinateSystem.SetAxisFadePercent(axisIndex, 0);

            base.Play();
        }

        // Called when animation ends
        public override void End()
        {
            coordinateSystem = null;
            arrow = null;
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
            float fade = Easing.EaseOut(elapsedTimePercent, EasingType.Quadratic);
            
            coordinateSystem.SetAxisFadePercent(axisIndex, fade);

            base.UpdateAnim();
        }

    }
}
