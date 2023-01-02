using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace Veridium_Animation
{
    public class Anim_SpinUp : AnimationBase
    {

        /// <summary>
        /// Implements a spin up animation. Includes a rotation, ease in scaling,
        /// and ease in upward translation.
        /// </summary>

        Vector3 originalPosition;           // Initial local position of the gameObject
        public float targetScale = 1;       // Initial local scale of the gameObject
        Quaternion originalRotation;        // Initial local rotation of the gameObject
        public float maxHeight = 0.0f;      // Distance above the initial height that the object will go
        public float startHeight = -0.5f;   // Distance below the initial height that the object will start
        public int numRotations = 1;        // Number of complete rotations that the object should complete
        public EasingType easingType;


        // Update is called once per frame
        protected override void UpdateAnim()
        {

            // Update the transform of the gameObject
            gameObject.transform.localPosition = originalPosition + new Vector3(0, Height(elapsedTimePercent), 0);
            gameObject.transform.localScale = targetScale * Vector3.one * Scale(elapsedTimePercent);
            gameObject.transform.localRotation = originalRotation * Rotation(elapsedTimePercent);
            
        }

        public override void Play()
        {

            base.Play();

            // Store the initial tranform of the gameObject
            originalPosition = gameObject.transform.localPosition;
            originalRotation = gameObject.transform.localRotation;

            // If the object is interactable, it shouldn't be during the animation
            if (GetComponent<XRGrabInteractable>() != null) {
                GetComponent<XRGrabInteractable>().enabled = false;
            }

        }

        public override void Pause()
        {

            base.Pause();

            // If the animation stops, it should be interactable again.
            if (GetComponent<XRGrabInteractable>() != null) 
            {
                GetComponent<XRGrabInteractable>().enabled = true;
            }

        }

        protected override void ResetChild()
        {

            base.ResetChild();

            // Reset the transform of the gameObject. Should only happen if the animation has already played or bad things may happen
            gameObject.transform.localPosition = originalPosition;
            gameObject.transform.localRotation = originalRotation;

            originalPosition = Vector3.zero;
            originalRotation = Quaternion.identity;

        }

        // Finds the relative vertical offset 
        private float Height(float time)
        {

            return (float) Easing.EaseOut(time, easingType) * (maxHeight - startHeight) + startHeight;

        }

        // Finds the relative uniform scale for the object
        private float Scale(float time)
        {

            return Easing.EaseOut(time, easingType);

        }

        // Finds the relative rotation
        private Quaternion Rotation(float time)
        {

            float rotation = Easing.EaseOut(time, easingType) * numRotations * 360;

            return Quaternion.Euler(0, rotation, 0);

        }

    }
}
