using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIB_Animation{
    public class Anim_Glow : AnimationBase
    {

        public Color emissionColor;             // Color that this object should glow
        public float minIntensity = 0f;         // Minimum intensity of the glow effect
        public int materialIndex = 0;           // The material index to apply the glow effect to
        private float timeAfterEnd;             // For trailing off glow
        private bool finishCycle;               // For trailing off glow
        public float maxIntensity = 0.6f;       // Maximum brightness that the glow effect will have
        public float fadeTime = 0.5f;           // Time to fade in or out
        public Easing.EasingType easingType;    // Easing function to use while fading


        public Anim_Glow(){
            indefiniteDuration = true;
        }

        protected override void UpdateAnim()
        {
            base.UpdateAnim();

            // If the renderer exists, pulse the emission
            if(elapsedTime < fadeTime){
                if(gameObject.GetComponent<Renderer>() != null) gameObject.GetComponent<Renderer>().materials[materialIndex].SetColor("_EmissionColor", emissionColor * Alpha(elapsedTime/fadeTime));
            }
        }

        
        protected override void Update(){

            base.Update();

            if(!playing && finishCycle){
                timeAfterEnd += Time.deltaTime;

                if(gameObject.GetComponent<Renderer>() != null) gameObject.GetComponent<Renderer>().materials[materialIndex].SetColor("_EmissionColor", emissionColor * Alpha(1f-(timeAfterEnd/fadeTime)));

                if(timeAfterEnd > fadeTime){
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

            timeAfterEnd = 0f;
            finishCycle = true;
        }

        // Finds the intensity of the emission at a given time
        private float Alpha(float time){

            return (maxIntensity - minIntensity) * Easing.EaseFull(time, easingType) + minIntensity;

        }
    }
}
