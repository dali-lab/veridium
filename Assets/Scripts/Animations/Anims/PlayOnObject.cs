using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Veridium_Animation{
    [System.Serializable]
    public class PlayOnObject : AnimationBase{

        public GameObject target;
        public AnimScriptType animType;

        public PlayOnObject(){
            duration = 0;
        }

        public override void OnValidate(AnimationManager manager){

            base.OnValidate(manager);

            if(animType != null){
                animType.OnValidate(null);
            }
        }
    
        // Called when animation is started
        public override void Play()
        {
            base.Play();

            AnimPlayer animPlayer = gameObject.AddComponent<AnimPlayer>() as AnimPlayer;
            animPlayer.animScript = animType.animScript.Clone();
            animPlayer.animScript.OnValidate(animPlayer);
            animPlayer.animScript.selfDestruct = true;
            animPlayer.animScript.Play();

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
        }
    }
}