using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Veridium_Animation{
    public class AwaitUserBase : AnimationBase
    {

        private bool permanentlyCompleted;                  // Does not reset when scrubbing; this is meant so that the user can scrub without repeating actions
        public bool skipSegment;
        public AnimSequence.AnimPlayer onComplete;

        public AwaitUserBase(){
            indefiniteDuration = true;
            duration = 0;
            awaitingAction = true;
        }

        public override void Play(){

            Reset();

            base.Play();

        }

        public override void End()
        {
            base.End();
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
            // Debug.Log(animSequence);

            if (skipSegment && animSequence.CanMoveOn()) animSequence.PlayNextSegment();
            
            Invoke("OnComplete", onComplete.timing + Time.deltaTime);

        }

        private void OnComplete(){

            switch (onComplete.actionType){
                case AnimSequence.ActionType.AnimationScript:
                    if(onComplete.animation != null) onComplete.animation.Play();
                    break;
                case AnimSequence.ActionType.UnityEvent:
                    onComplete.onPlay.Invoke();
                    break;
                case AnimSequence.ActionType.Animator:
                    break;
            }

        }
    }
}
