using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SIB_Animation{
    public class Await_Grab : AwaitUserBase
    {
        
        public XRGrabInteractable grabInteractable;

        void Awake(){

            grabInteractable.selectEntered.AddListener(Grabbed);

        }

        public override void Play()
        {
            base.Play();

            if(grabInteractable.isSelected && grabInteractable.selectingInteractor is XRDirectInteractor) CompleteAction();
        }

        void Grabbed(SelectEnterEventArgs args){

            if(args.interactor is XRDirectInteractor) CompleteAction();

        }

    }
}
