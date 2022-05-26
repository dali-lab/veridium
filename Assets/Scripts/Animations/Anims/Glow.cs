using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;

namespace Veridium_Animation{
    [System.Serializable]
    public class Glow : AnimationBase{

        public Color emissionColor;             // Color that this object should glow
        public float minIntensity = 0f;         // Minimum intensity of the glow effect
        public int materialIndex = 0;           // The material index to apply the glow effect to
        private float timeAfterEnd;             // For trailing off glow
        private bool finishCycle;               // For trailing off glow
        public float maxIntensity = 0.6f;       // Maximum brightness that the glow effect will have
        public float fadeTime = 0.5f;           // Time to fade in or out
        public EasingType easingType;           // Easing function to use while fading


        public Glow(){
            indefiniteDuration = true;
        }

        public override void Update(){

            base.Update();

            if(!playing && finishCycle){
                timeAfterEnd += Time.deltaTime;

                if(gameObject.GetComponent<Renderer>() != null) gameObject.GetComponent<Renderer>().materials[materialIndex].SetColor("_EmissionColor", emissionColor * Alpha(1f-(timeAfterEnd/fadeTime)));

                if(timeAfterEnd > fadeTime){
                    finishCycle = false;
                    timeAfterEnd = 0;

                    // Turn off the emission if the animation is paused
                    if(gameObject.GetComponent<Renderer>() != null) gameObject.GetComponent<Renderer>().materials[materialIndex].DisableKeyword("_EMISSION");
                    base.End();
                    if(selfDestruct) MonoBehaviour.Destroy(manager);
                }
            }
        }

    
        // Called when animation is started
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

        // Called when animation ends
        public override void End()
        {
            
        }

        // Called when animation is paused
        public override void Pause()
        {
            base.Pause();

            timeAfterEnd = 0f;
            finishCycle = true;
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

            // If the renderer exists, pulse the emission
            if(elapsedTime < fadeTime){
                if(gameObject.GetComponent<Renderer>() != null) gameObject.GetComponent<Renderer>().materials[materialIndex].SetColor("_EmissionColor", emissionColor * Alpha(elapsedTime/fadeTime));
            }
        }

        // Finds the intensity of the emission at a given time
        private float Alpha(float time){

            return (maxIntensity - minIntensity) * Easing.EaseFull(time, easingType) + minIntensity;

        }
    }
}