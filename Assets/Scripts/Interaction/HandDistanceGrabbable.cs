using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Veridium_Animation;

namespace Veridium_Interaction{
    public class HandDistanceGrabbable : XRGrabInteractable_Lockable
    {
        /// <summary>
        /// HandDistanceGrabbable should be attached to a game object with
        /// an XR interaction toolkit XRGrabInteractable. This script should
        /// be modified to highlight and unhighlight the game object.
        /// </summary>

        private bool hovered;               // Whether this distance grabbable is currently hovered
        public Anim_Highlight highlight;     // The Animation to highlight the grabbable

        protected override void OnHoverEntered(HoverEnterEventArgs args){

            base.OnHoverEntered(args);

            if(args.interactor is XRDirectInteractor){
                hovered = true;
                highlight.Highlight();
            }
        }

        protected override void OnHoverExited(HoverExitEventArgs args){

            base.OnHoverExited(args);

            if(args.interactor is XRDirectInteractor){
                hovered = false;
                highlight.Unhighlight();
            }
        }

        public bool isHovered(){
            return hovered;
        }
    }
}
