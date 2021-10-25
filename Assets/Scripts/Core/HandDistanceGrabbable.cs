using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandDistanceGrabbable : XRGrabInteractable
{

    private Vector3 colliderCenter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hovered(GameObject hand){
        GetComponent<ToggleOutline>().toggleOutline(true);

        colliderCenter = GetComponent<BoxCollider>().center;

        GetComponent<BoxCollider>().center = transform.InverseTransformPoint(hand.transform.position);

        (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = transform.InverseTransformPoint(hand.transform.position).ToString();

    }

    public void UnHovered(){
        GetComponent<ToggleOutline>().toggleOutline(false);

        GetComponent<BoxCollider>().center = colliderCenter;
    }
}
