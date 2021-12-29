using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIB_Animation{
    public class AwaitUserBase : AnimationBase
    {

        public bool completed {get; private set;}           // Whether the user has performed the specified action
        private bool permanentlyCompleted;                  // Does not reset when scrubbing; this is meant so that the user can scrub without repeating actions

        public AwaitUserBase(){
            indefiniteDuration = true;
            duration = 0;
            awaitingAction = true;
        }

        protected override void UpdateAnim()
        {
            base.UpdateAnim();
        }

        public override void Play()
        {
            base.Play();

            awaitingAction = true;
        }

        public override void Pause()
        {
            base.Pause();

            awaitingAction = false;
        }

        protected override void ResetChild()
        {
            base.ResetChild();

            completed = false;
        }

        // Performs the action automatically. This is to be done while scrubbing
        protected virtual void DoAction(){

        }

        // Should be called when the desired action is completed
        public virtual void completeAction(){

            Pause();

        }

    }
}
