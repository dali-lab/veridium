using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Veridium.Animation;

namespace Veridium.Modules.AminoAcids {
    public class AwaitXRGrab : AwaitAny
    {
        public XRGrabInteractable interactable;

        public override void Play()
        {
            base.Play();
            if (!interactable) return;
            print("Active Await XR Grab");

            interactable.selectEntered.AddListener(OnSelectEnter);
        }

        private void OnSelectEnter(SelectEnterEventArgs args)
        {
            CompleteAction();
            interactable.selectEntered.RemoveListener(OnSelectEnter);
            print("Deactive Await XR Grab");
        }
    }
}