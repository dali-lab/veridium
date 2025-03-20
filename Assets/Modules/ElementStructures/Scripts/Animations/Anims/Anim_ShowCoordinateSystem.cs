using UnityEngine;
using Veridium.Animation;

namespace Veridium.Modules.ElementStructures
{
    [System.Serializable]
    public struct AxisData {
        public Vector3 direction;
        public Color color;
        public string label;
    }

    public class Anim_ShowCoordinateSystem : AnimationBase
    {
        public GameObject coordinateSystemPrefab;
        public StructureBase structureBase;

        public AxisData[] axes;

        private CoordinateSystemVisualization coordinateSystem;
        
        // Called when animation is started
        public override void Play()
        {
            coordinateSystem = structureBase.GetComponentInChildren<CoordinateSystemVisualization>();
            if (coordinateSystem == null)
            {
                GameObject coordinateSystem = Instantiate(coordinateSystemPrefab, structureBase.transform);
                coordinateSystem.transform.localPosition = new Vector3(0, 0, 0);
                coordinateSystem.transform.localRotation = Quaternion.identity;
                coordinateSystem.transform.localScale = new Vector3(1, 1, 1);

                this.coordinateSystem = coordinateSystem.GetComponent<CoordinateSystemVisualization>();
                this.coordinateSystem.ClearAxes();
            }

            foreach (AxisData axis in axes) {
                coordinateSystem.DrawArrow(Vector3.zero, axis.direction, axis.color, axis.label);
            }

            coordinateSystem.SetAxisHighlightPercent(0, 0);
            coordinateSystem.SetAxisHighlightPercent(1, 0);
            coordinateSystem.SetAxisHighlightPercent(2, 0);
            coordinateSystem.SetFadePercent(0);

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
            float opacity = Easing.EaseFull(elapsedTimePercent, EasingType.Quadratic);
            coordinateSystem.SetFadePercent(opacity);

            base.UpdateAnim();
        }

    }
}
