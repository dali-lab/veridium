using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using SIB_Animation;

public class HandDistanceGrabbable : MonoBehaviour
{
    /// <summary>
    /// HandDistanceGrabbable should be attached to a game object with
    /// an XR interaction toolkit XRGrabInteractable. This script should
    /// be modified to highlight and unhighlight the game object.
    /// </summary>

    private bool hovered;               // Whether this distance grabbable is currently hovered
    private bool hoveredLastFrame;      // Whether this distance grabbable was hovered last frame
    public Anim_Highlight highlight;     // The Animation to highlight the grabbable

    // Update is called once per frame
    void Update()
    {

        // Turn off highlight if not currently hovered
        if(hoveredLastFrame && !hovered) UnHovered();

        hovered = false;
        
    }

    // Hovered called by HandDistanceGrabber. Handles highlighting
    public void Hovered(GameObject hand){

        highlight.Highlight();

        // Insert code to highlight the interactable

        hovered = true;
        hoveredLastFrame = true;
    }

    // UnHovered called by HandDistanceGrabbable. Handles unhighlighting
    public void UnHovered(){

        highlight.Unhighlight();

        // Insert code to unhighlight the interactable

        hovered = false;
        hoveredLastFrame = false;
    }
}
