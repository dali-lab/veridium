using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandDistanceGrabbable : MonoBehaviour
{

    private Vector3 colliderCenter;
    private bool hovered;
    private bool hoveredLastFrame;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(hoveredLastFrame && ! hovered) UnHovered();

        hovered = false;
        
    }

    public void Hovered(GameObject hand){
        GetComponent<ToggleOutline>().toggleOutline(true);

        hovered = true;
        hoveredLastFrame = true;
    }

    public void UnHovered(){
        GetComponent<ToggleOutline>().toggleOutline(false);

        hovered = false;
        hoveredLastFrame = false;
    }
}
