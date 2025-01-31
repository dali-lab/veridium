using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Veridium.Interaction
{
    public class HandDistanceClickable : XRBaseInteractable
    {
        public bool hovered;               // Whether this distance grabbable is currently hovered
        public bool drawRay = true;

        protected override void OnHoverEntered(HoverEnterEventArgs args){

            base.OnHoverEntered(args);

            if(args.interactorObject is XRDirectInteractor){
                hovered = true;
            }
        }

        protected override void OnHoverExited(HoverExitEventArgs args){

            base.OnHoverExited(args);

            if(args.interactorObject is XRDirectInteractor){
                hovered = false;
            }
        }

        new public bool isHovered(){
            return hovered;
        }
    }
}