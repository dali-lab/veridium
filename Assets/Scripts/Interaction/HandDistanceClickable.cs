using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandDistanceClickable : XRBaseInteractable
{
    public bool hovered;               // Whether this distance grabbable is currently hovered
    public bool drawRay = true;

    protected override void OnHoverEntered(HoverEnterEventArgs args){

        base.OnHoverEntered(args);

        if(args.interactor is XRDirectInteractor){
            hovered = true;
        }
    }

    protected override void OnHoverExited(HoverExitEventArgs args){

        base.OnHoverExited(args);

        if(args.interactor is XRDirectInteractor){
            hovered = false;
        }
    }

    public bool isHovered(){
        return hovered;
    }
}
