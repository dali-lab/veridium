using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIB_Animation{
    public class Anim_GlowPulse : AnimationBase
    {

        /// <summary>
        /// Glow pulse implements a sinusoidal glow pulse animation on the material of
        /// the GameObject. This animation is different from the others in that it can
        /// be set to pulse indefinitely until stopped.
        /// </summary>

        public Color emissionColor;         // Color that this object should glow. In most cases, should be set to the material's albedo
        float blinksPerSecond = 0.5f;       // Number of bright peaks of the sin wave per second
        float minIntensity = 0.5f;          // Minimum intensity of the glow effect. The emission will oscillate between the minimum and full brightness

        protected override void UpdateAnim()
        {
            base.UpdateAnim();

            // If the renderer exists, pulse the emission
            if(gameObject.GetComponent<Renderer>() != null) gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", emissionColor * Alpha(elapsedTime));
        }

        public override void Play()
        {
            base.Play();

            if(emissionColor == null) emissionColor = Color.white;

            // If the renderer exists, enable emission
            if(gameObject.GetComponent<Renderer>() != null){
                gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", emissionColor * Alpha(0f));
                gameObject.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            }
        }

        public override void Pause()
        {
            base.Pause();

            // Turn off the emission if the animation is paused
            if(gameObject.GetComponent<Renderer>() != null) gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");;
        }

        // Finds the intensity of the emission at a given time
        private float Alpha(float time){

            float angle = time * 2 * Mathf.PI * blinksPerSecond;

            return Mathf.Sin(angle) * (1 - minIntensity) + minIntensity;

        }
    }
}
