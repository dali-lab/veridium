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
    public class AwaitGrab : AwaitUserBase{

        public XRGrabInteractable grabInteractable;

        public override void Play()
        {
            base.Play();

            grabInteractable.selectEntered.AddListener(Grabbed);

            if(grabInteractable.isSelected && grabInteractable.selectingInteractor is XRDirectInteractor) CompleteAction();
            
        }

        void Grabbed(SelectEnterEventArgs args){

            if(args.interactor is XRDirectInteractor) {
                CompleteAction();
                grabInteractable.selectEntered.RemoveListener(Grabbed);
            }
        }
    }
}