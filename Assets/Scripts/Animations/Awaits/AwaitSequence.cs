using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Veridium_Animation{
    [System.Serializable]
    public class AwaitSequence : AwaitUserBase{

        public List<AwaitUserBase> awaiters;
        private int waitingIndex;

        public override void Play()
        {
            base.Play();

            waitingIndex = 0;

            awaiters[0].Reset();
            awaiters[0].Play();
            
        }

        protected override void ResetChild(){

            base.ResetChild();

            waitingIndex = 0;

            foreach (AwaitUserBase awaiter in awaiters) {

                if(awaiter != null) awaiter.Reset();

            }

        }

        public override void Pause(){

            base.Pause();

            foreach (AwaitUserBase awaiter in awaiters) {

                //if(awaiter != null) awaiter.Pause();

            }

        }

        protected override void UpdateAnim(){

            base.UpdateAnim();

            if(!awaiters[waitingIndex].awaitingAction){

                awaiters[waitingIndex].Pause();

                waitingIndex ++;

                if(waitingIndex >= awaiters.Count){
                    CompleteAction();
                } else {
                    awaiters[waitingIndex].Reset();
                    awaiters[waitingIndex].Play();
                }
            }
        }
    }
}