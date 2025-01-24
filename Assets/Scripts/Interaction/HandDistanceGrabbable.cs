using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Veridium.Animation;

namespace Veridium.Interaction{
    public class HandDistanceGrabbable : XRGrabInteractable_Lockable
    {
        /// <summary>
        /// HandDistanceGrabbable should be attached to a game object with
        /// an XR interaction toolkit XRGrabInteractable. This script should
        /// be modified to highlight and unhighlight the game object.
        /// </summary>

        public bool hovered;               // Whether this distance grabbable is currently hovered
        public Anim_Highlight highlight;     // The Animation to highlight the grabbable
        public bool drawRay = true;

        protected override void OnHoverEntered(HoverEnterEventArgs args){

            base.OnHoverEntered(args);

            if(args.interactorObject is XRDirectInteractor){
                hovered = true;
                if (highlight != null) highlight.Highlight();
            }
        }

        protected override void OnHoverExited(HoverExitEventArgs args){

            base.OnHoverExited(args);

            if(args.interactorObject is XRDirectInteractor){
                hovered = false;
                if (highlight != null) highlight.Unhighlight();
            }
        }

        new public bool isHovered(){
            return hovered;
        }
    }
}
