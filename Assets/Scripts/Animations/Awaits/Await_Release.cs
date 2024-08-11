using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace Veridium_Animation{
    public class Await_Release : AwaitUserBase
    {

        public XRGrabInteractable grabInteractable;

        public override void Play()
        {
            base.Play();

            grabInteractable.selectExited.AddListener(Released);

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
