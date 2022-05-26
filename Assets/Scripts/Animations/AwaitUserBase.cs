using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Veridium_Animation{
    [System.Serializable]
    public class AwaitUserBase : AnimationBase{

        private bool permanentlyCompleted;                  // Does not reset when scrubbing; this is meant so that the user can scrub without repeating actions
        public bool skipSegment;
        public SegmentAnimPlayer onComplete;

        public AwaitUserBase(){
            indefiniteDuration = true;
            duration = 0;
            awaitingAction = true;
        }

        public override void OnValidate(AnimationManager parent)
        {
            base.OnValidate(parent);

            if (onComplete != null)
            {
                onComplete.OnValidate(manager);
            }
        }

        public override void Play(){

            Reset();

            base.Play();

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

            if (manager is AnimSequence) {

                AnimSequence sequence = manager as AnimSequence;

                if (skipSegment && sequence.CanMoveOn()) sequence.PlayNextSegment();

            }
            
            //MonoBehaviour.Invoke("OnComplete", onComplete.timing + Time.deltaTime);
            OnComplete();

        }

        private void OnComplete(){

            onComplete.Execute();
        }
    }
}