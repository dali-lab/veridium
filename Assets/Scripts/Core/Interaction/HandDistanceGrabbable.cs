using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandDistanceGrabbable : MonoBehaviour
{

    private Vector3 colliderCenter;
    private bool hovered;
    private bool hoveredLastFrame;
    public GameObject handCollider;
    private bool selected;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = hovered;

        (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = selected.ToString();

        if(hoveredLastFrame && ! hovered) UnHovered();

        hovered = false;

        if(selected) gameObject.transform.position = handCollider.transform.position;
        
    }

    public void Selected(){

        //if (interactor.GetType() == typeof(XRDirectInteractor)){
            selected = true;
        //}
    }

    public void UnSelected(){
        selected = false;
    }

    public void Hovered(GameObject hand){
        GetComponent<ToggleOutline>().toggleOutline(true);

        handCollider.transform.position = hand.transform.position;

        hovered = true;
        hoveredLastFrame = true;
    }

    public void UnHovered(){
        GetComponent<ToggleOutline>().toggleOutline(false);

        handCollider.transform.localPosition = Vector3.zero;

        hovered = false;
        hoveredLastFrame = false;
    }
}
