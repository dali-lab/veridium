using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace Veridium_Animation{
    public class Await_Release : AwaitUserBase
    {

        public XRGrabInteractable grabInteractable;

        void Awake(){

            grabInteractable.selectExited.AddListener(Released);

        }

        public override void Play()
        {
            base.Play();

            if(!grabInteractable.isSelected) CompleteAction();
        }

        void Released(SelectExitEventArgs args){

            CompleteAction();
            
        }

    }
}
