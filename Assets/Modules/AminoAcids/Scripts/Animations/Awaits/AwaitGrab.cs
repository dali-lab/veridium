using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Veridium.Animation;

namespace Veridium.Modules.AminoAcids {
    public class AwaitGrab : AwaitAny
    {
        public XRDirectInteractor leftHand;
        public XRDirectInteractor rightHand;
        public XRGrabInteractable optionalTarget;
        public bool requireBothHands;

        public override void Play()
        {
            base.Play();
            leftHand.selectEntered.AddListener(OnGrab);
            rightHand.selectEntered.AddListener(OnGrab);
        }

        private void OnGrab(SelectEnterEventArgs args)
        {
            if (optionalTarget != null && args.interactableObject != (optionalTarget as IXRSelectInteractable)) return;
            if (requireBothHands && !(leftHand.IsSelecting(args.interactableObject) && rightHand.IsSelecting(args.interactableObject))) return;

            CompleteAction();
            leftHand.selectEntered.RemoveListener(OnGrab);
            rightHand.selectEntered.RemoveListener(OnGrab);
        }
    }
}