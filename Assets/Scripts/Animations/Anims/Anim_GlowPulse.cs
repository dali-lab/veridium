using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Veridium.Animation{
    public class Anim_GlowPulse : AnimationBase
    {

        /// <summary>
        /// Glow pulse implements a sinusoidal glow pulse animation on the material of
        /// the GameObject. This animation is different from the others in that it can
        /// be set to pulse indefinitely until stopped.
        /// </summary>

        public Color emissionColor;             // Color that this object should glow. In most cases, should be set to the material's albedo
        public float blinksPerSecond = 0.5f;    // Number of bright peaks of the sin wave per second
        public float minIntensity = 0f;         // Minimum intensity of the glow effect. The emission will oscillate between the minimum and full brightness
        public int materialIndex = 0;           // The material index to apply the glow effect to
        private float timeAfterEnd;             // For trailing off glow
        private bool finishCycle;               // For trailing off glow
        public float maxIntensity = 0.6f;       // Maximum brightness that the glow effect will have


        public Anim_GlowPulse(){
            indefiniteDuration = true;
        }

        protected override void UpdateAnim()
        {
            base.UpdateAnim();

            // If the renderer exists, pulse the emission
            if(gameObject.GetComponent<Renderer>() != null) gameObject.GetComponent<Renderer>().materials[materialIndex].SetColor("_EmissionColor", emissionColor * Alpha(elapsedTime));
        }

        
        protected override void Update(){

            base.Update();

            if(!playing && finishCycle){
                timeAfterEnd += Time.deltaTime;

                if(gameObject.GetComponent<Renderer>() != null) gameObject.GetComponent<Renderer>().materials[materialIndex].SetColor("_EmissionColor", emissionColor * Alpha(timeAfterEnd));

                if(timeAfterEnd % (1/blinksPerSecond) > (1/blinksPerSecond) - 1.1 * Time.deltaTime){
                    finishCycle = false;
                    timeAfterEnd = 0;

                    // Turn off the emission if the animation is paused
                    if(gameObject.GetComponent<Renderer>() != null) gameObject.GetComponent<Renderer>().materials[materialIndex].DisableKeyword("_EMISSION");
                    if(selfDestruct) Destroy(this);
                }
            }
        }

        public override void Play()
        {
            base.Play();

            if(emissionColor == null) emissionColor = Color.white;

            // If the renderer exists, enable emission
            if(gameObject.GetComponent<Renderer>() != null){
                gameObject.GetComponent<Renderer>().materials[materialIndex].SetColor("_EmissionColor", emissionColor * Alpha(0f));
                gameObject.GetComponent<Renderer>().materials[materialIndex].EnableKeyword("_EMISSION");
            }
        }

        public override void End()
        {
            
        }

        public override void Pause()
        {
            base.Pause();

            timeAfterEnd = elapsedTime;
            finishCycle = true;
        }

        // Finds the intensity of the emission at a given time
        private float Alpha(float time){

            float angle = time * 2 * Mathf.PI * blinksPerSecond - Mathf.PI/2;

            float brightness = (Mathf.Sin(angle) + 1)/2;

            brightness *= (maxIntensity-minIntensity);
            
            brightness += minIntensity;

            return brightness;

        }
    }
}
