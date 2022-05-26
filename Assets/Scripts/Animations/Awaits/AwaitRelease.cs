using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

namespace Veridium_Animation{
    [System.Serializable]
    public class AwaitRelease : AwaitUserBase{

        public XRGrabInteractable grabInteractable;

        public override void Play()
        {
            base.Play();

            // Add listeners to the proper events
            grabInteractable.selectExited.AddListener(Released);

            // Check if the user has already completed the action
            if(!grabInteractable.isSelected || !(grabInteractable.selectingInteractor is XRDirectInteractor)) CompleteAction();
            
        }

        void Released(SelectExitEventArgs args){

            if(args.interactor is XRDirectInteractor) CompleteAction();
            
        }

        protected override void UpdateAnim(){

            base.UpdateAnim();

            if(!grabInteractable.isSelected || !(grabInteractable.selectingInteractor is XRDirectInteractor)) CompleteAction();
        }
    }
}