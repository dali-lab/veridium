using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Veridium_Animation{
    public class AwaitSequence : AwaitUserBase
    {
        
        public List<AwaitUserBase> awaiters;
        private int waitingIndex;

        public override void Play(){

            base.Play();

            waitingIndex = 0;

            awaiters[0].Reset();
            awaiters[0].Play();

        }

        protected override void ResetChild(){

            base.ResetChild();

            waitingIndex = 0;

            for(int i = 0; i <= waitingIndex; i++) {
                if(awaiters[i] != null) awaiters[i].Reset();
            }

        }

        public override void Pause(){

            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text += "E";

            base.Pause();

            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text += "F";

            for(int i = 0; i <= waitingIndex; i++) {
                (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text += "G";
                if(awaiters[i] != null) awaiters[i].Pause();
                (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text += "H";
            }
            
            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text += "I";

        }

        protected override void UpdateAnim(){

            base.UpdateAnim();

            if(!awaiters[waitingIndex].awaitingAction){

                awaiters[waitingIndex].Pause();

                waitingIndex ++;

                if(waitingIndex >= awaiters.Count){
                    (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "";
                    CompleteAction();
                } else {
                    awaiters[waitingIndex].Reset();
                    awaiters[waitingIndex].Play();
                }
            }
        }
    }
}
