using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Veridium_Animation{
    [System.Serializable]
    public class AwaitAny : AwaitUserBase{
        [SerializeReference] public List<AwaitUserType> awaiters;

        public override void OnValidate(AnimationManager parent)
        {
            base.OnValidate(parent);

            if(awaiters != null){
                foreach(AwaitUserType awaitType in awaiters){
                    if(awaitType == null) awaiters[awaiters.IndexOf(awaitType)] = new AwaitUserType();
                    awaitType.OnValidate(manager);
                }
            }
        }

        public override void Play()
        {
            base.Play();

            foreach(AwaitUserType awaiter in awaiters)
            {
                awaiter.await.Play();
            }
            
        }

        protected override void ResetChild(){

            base.ResetChild();

            foreach(AwaitUserType awaiter in awaiters)
            {
                awaiter.await.Reset();
            }
        }

        public override void Pause(){

            base.Pause();

            foreach (AwaitUserType awaiter in awaiters)
            {
                awaiter.await.Pause();
            }

        }

        protected override void UpdateAnim(){

            base.UpdateAnim();

            bool completed = false;

            foreach (AwaitUserType awaiter in awaiters)
            {
                if(!awaiter.await.awaitingAction) completed = true;
            }

            if(completed) CompleteAction();

        }
    }
}