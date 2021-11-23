using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandDistanceGrabbable : MonoBehaviour
{
    /// <summary>
    /// HandDistanceGrabbable should be attached to a game object with
    /// an XR interaction toolkit XRGrabInteractable. This script should
    /// be modified to highlight and unhighlight the game object.
    /// </summary>

    private bool hovered;               // Whether this distance grabbable is currently hovered
    private bool hoveredLastFrame;      // Whether this distance grabbable was hovered last frame
    public GameObject glowObject;       // The gameobject that is the highlight for this gameobject.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // Turn off highlight if not currently hovered
        if(hoveredLastFrame && !hovered) UnHovered();

        hovered = false;
        
    }

    // Hovered called by HandDistanceGrabber. Handles highlighting
    public void Hovered(GameObject hand){

        glowObject.SetActive(true);

        // Insert code to highlight the interactable

        hovered = true;
        hoveredLastFrame = true;
    }

    // UnHovered called by HandDistanceGrabbable. Handles unhighlighting
    public void UnHovered(){

        glowObject.SetActive(false);

        // Insert code to unhighlight the interactable

        hovered = false;
        hoveredLastFrame = false;
    }
}
