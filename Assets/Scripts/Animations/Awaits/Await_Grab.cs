using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Veridium.Animation{
    public class Await_Grab : AwaitUserBase
    {
        
        public XRGrabInteractable grabInteractable;

        public int numHandsRequired = 1;

        protected override void Start()
        {
            base.Start();
            grabInteractable.selectEntered.AddListener(Grabbed);
        }

        public override void Play()
        {
            base.Play();

            grabInteractable.selectEntered.AddListener(Grabbed);

            if(grabInteractable.isSelected && grabInteractable.GetOldestInteractorSelecting() is XRDirectInteractor) CompleteAction();
        }

        void Grabbed(SelectEnterEventArgs args){
            if (grabInteractable.interactorsSelecting.Where(x => x is XRDirectInteractor).Count() >= numHandsRequired) {
                CompleteAction();
                grabInteractable.selectEntered.RemoveListener(Grabbed);
            }
        }
    }
}
