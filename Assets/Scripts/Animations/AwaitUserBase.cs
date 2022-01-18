using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIB_Animation{
    public class AwaitUserBase : AnimationBase
    {

        private bool permanentlyCompleted;                  // Does not reset when scrubbing; this is meant so that the user can scrub without repeating actions
        public bool skipSegment;

        public AwaitUserBase(){
            indefiniteDuration = true;
            duration = 0;
            awaitingAction = true;
        }

        protected override void ResetChild()
        {
            base.ResetChild();

            awaitingAction = true;
        }

        // Performs the action automatically. This is to be done while scrubbing
        protected virtual void DoAction(){

        }

        // Should be called when the desired action is completed
        public virtual void CompleteAction(){

            if (playing) awaitingAction = false;

            if (skipSegment && animSequence.CanMoveOn()) animSequence.PlayNextSegment();

        }

    }
}
