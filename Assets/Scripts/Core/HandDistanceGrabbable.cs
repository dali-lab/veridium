using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandDistanceGrabbable : XRGrabInteractable
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
        gameObject.GetComponent<Rigidbody>().isKinematic = hovered;

        if(hoveredLastFrame){
            
        } else if(hovered){

            UnHovered();

        }

        hovered = false;
        
    }

    public void Hovered(GameObject hand){
        GetComponent<ToggleOutline>().toggleOutline(true);

        colliderCenter = GetComponent<BoxCollider>().center;

        GetComponent<BoxCollider>().center = transform.InverseTransformPoint(hand.transform.position);

        hovered = true;
        hoveredLastFrame = true;
    }

    public void UnHovered(){
        GetComponent<ToggleOutline>().toggleOutline(false);

        GetComponent<BoxCollider>().center = colliderCenter;

        hovered = false;
    }
}
