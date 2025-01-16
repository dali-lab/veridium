using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium_Interaction;

namespace Veridium_Animation{
    public class Anim_ShowCoordinateSystem : AnimationBase
    {
        public GameObject coordinateSystemPrefab;
        public StructureBase structureBase;

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
            coordinateSystem.SetFadePercent(1);
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
