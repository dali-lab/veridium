using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;


namespace SIB_Animation{
    public class Anim_SpinUp : AnimationBase
    {

        Vector3 originalPosition;
        Vector3 originalScale;
        Quaternion originalRotation;

        public float maxHeight = 0.0f;
        public float startHeight = -0.5f;
        public int numRotations = 1;

        // Update is called once per frame
        protected override void UpdateAnim()
        {

            base.Update();

            gameObject.transform.localPosition = originalPosition + new Vector3(0, Height(elapsedTime), 0);
            gameObject.transform.localScale = originalScale * Scale(elapsedTime);
            gameObject.transform.localRotation = originalRotation * Rotation(elapsedTime);
            
        }

        public override void Play()
        {

            base.Play();

            originalPosition = gameObject.transform.localPosition;
            originalScale = gameObject.transform.localScale;
            originalRotation = gameObject.transform.localRotation;

            if (GetComponent<XRGrabInteractable>() != null) GetComponent<XRGrabInteractable>().enabled = false;

        }

        public override void Pause()
        {

            base.Pause();

            if (GetComponent<XRGrabInteractable>() != null) GetComponent<XRGrabInteractable>().enabled = true;

        }

        public override void Reset()
        {

            base.Reset();

            gameObject.transform.localPosition = originalPosition;
            gameObject.transform.localScale = originalScale;
            gameObject.transform.localRotation = originalRotation;

        }

        private float Height(float time){
            
            float percentDone = time/duration;

            return (float) EaseIn(time/duration) * (maxHeight - startHeight) + startHeight;

        }

        private float Scale(float time){

            return EaseIn(time/duration);

        }

        private Quaternion Rotation(float time){

            float rotation = EaseIn(time/duration) * numRotations * 360;

            return Quaternion.Euler(0, rotation, 0);

        }

        private float EaseIn(float percent){

            if (percent >= 0.99) return 1f;

            if (percent < 0) return 0f;

            return (float) (2 / (1 + Mathf.Pow(2.71828182846f, -6f * percent)) - 0.995f);

        }
    }
}
