using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIB_Animation{
    public class Anim_MoveTo : AnimationBase
    {

        public Vector3 endLocation;
        public bool updateLocation = true;
        public Quaternion endRotation;
        public bool updateRotation = true;
        public Vector3 endScale = new Vector3(1,1,1);
        public bool updateScale = true;
        public Transform endTransform;
        public bool useTransform;
        public GameObject target;
        public Easing.EasingType easingType = Easing.EasingType.Linear;
        private Vector3 startingPlace;
        private Quaternion startingRotation;
        private Vector3 startingScale;
        [HideInInspector] public bool easeOutOnly = false;
        
        public override void Play()
        {

            base.Play();

            if(target == null) target = gameObject;

            startingPlace = target.transform.position;
            startingRotation = target.transform.rotation;
            startingScale = target.transform.localScale;

        }

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

        public override void Pause()

        {
            base.Pause();
        }

    }
}
