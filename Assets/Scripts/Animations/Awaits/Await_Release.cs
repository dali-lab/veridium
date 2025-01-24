using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace Veridium.Animation{
    public class Await_Release : AwaitUserBase
    {

        public XRGrabInteractable grabInteractable;

        public override void Play()
        {
            base.Play();

            grabInteractable.selectExited.AddListener(Released);

            if(!grabInteractable.isSelected || !(grabInteractable.GetOldestInteractorSelecting() is XRDirectInteractor)) CompleteAction();

        }

        void Released(SelectExitEventArgs args){

            if(args.interactorObject is XRDirectInteractor) CompleteAction();
            
        }

        protected override void UpdateAnim(){

            base.UpdateAnim();

            if(!grabInteractable.isSelected || !(grabInteractable.GetOldestInteractorSelecting() is XRDirectInteractor)) CompleteAction();
        }
    }
}
