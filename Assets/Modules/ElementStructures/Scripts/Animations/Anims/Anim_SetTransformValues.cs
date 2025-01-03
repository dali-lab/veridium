using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Veridium_Animation{
    public class Anim_SetTransformValues : AnimationBase
    {

        public Vector3 endLocation;
        public bool updateLocation = true;
        public Quaternion endRotation;
        public bool updateRotation = true;
        public Vector3 endScale = new Vector3(1,1,1);
        public bool updateScale = true;
        public GameObject target;
        
        public override void Play()
        {
            base.Play();

            if(target == null) target = gameObject;
            
            if(updateLocation) target.transform.localPosition = endLocation;
            if(updateRotation) target.transform.localRotation = endRotation;
            if(updateScale) target.transform.localScale = endScale;
        }

        protected override void UpdateAnim()
        {
            base.UpdateAnim();
        }

        public override void Pause()

        {
            base.Pause();
        }

    }
}
