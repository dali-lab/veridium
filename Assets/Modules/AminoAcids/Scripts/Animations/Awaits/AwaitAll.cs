using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Veridium.Animation{
    public class AwaitAll : AwaitUserBase
    {
        
        public List<AwaitUserBase> awaiters;

        public override void Play(){

            base.Play();

            foreach(AwaitUserBase awaiter in awaiters)
            {
                awaiter.Play();
            }

        }

        protected override void ResetChild(){

            base.ResetChild();

            foreach(AwaitUserBase awaiter in awaiters)
            {
                awaiter.Reset();
            }
        }

        public override void Pause(){

            base.Pause();

            foreach (AwaitUserBase awaiter in awaiters)
            {
                awaiter.Pause();
            }

        }

        protected override void UpdateAnim(){
            base.UpdateAnim();

            foreach (AwaitUserBase awaiter in awaiters)
            {
                if (awaiter.awaitingAction) return;
            }

            CompleteAction();
        }

    }
}
