using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Veridium_Animation{
    [System.Serializable]
    public class MoveTo : AnimationBase{

        public Vector3 endLocation;
        public bool updateLocation = true;
        public Quaternion endRotation;
        public bool updateRotation = true;
        public Vector3 endScale = new Vector3(1,1,1);
        public bool updateScale = true;
        public Transform endTransform;
        public bool useTransform;
        public GameObject target;
        public EasingType easingType = EasingType.Linear;
        private Vector3 startingPlace;
        private Quaternion startingRotation;
        private Vector3 startingScale;
        [HideInInspector] public bool easeOutOnly = false;
    
        // Called when animation is started
        public override void Play()
        {
            base.Play();

            if(target == null) target = gameObject;

            startingPlace = target.transform.position;
            startingRotation = target.transform.rotation;
            startingScale = target.transform.localScale;
        }

        // Called when animation ends
        public override void End()
        {
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
            base.UpdateAnim();

            Vector3 goal = useTransform ? endTransform.position : endLocation;
            Quaternion goalRotation = useTransform ? endTransform.rotation : endRotation;
            Vector3 goalScale = useTransform ? endTransform.lossyScale : endScale;

            float alpha;

            if(!easeOutOnly){
                alpha = (Easing.EaseFull(elapsedTimePercent, easingType));
            } else {
                alpha = (Easing.EaseOut(elapsedTimePercent, easingType));
            }

            if(updateLocation) target.transform.position = (goal - startingPlace) * alpha + startingPlace;
            if(updateRotation) target.transform.rotation = Quaternion.Slerp(startingRotation, goalRotation, alpha);
            if(updateScale) target.transform.localScale = (goalScale - startingScale) * alpha + startingScale;
        }
    }
}