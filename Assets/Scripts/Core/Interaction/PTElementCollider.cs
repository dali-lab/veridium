using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PTElementCollider : XRGrabInteractable
{
    public PTElement element;
    public HandDistanceGrabbable handDistanceGrabbable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnSelectEntering(XRBaseInteractor interactor){

        base.OnSelectEntering(interactor);

        if(interactor.GetType() == typeof(XRDirectInteractor)){
            handDistanceGrabbable.Selected();
        }
    
    }

    protected override void OnSelectExiting(XRBaseInteractor interactor) {

        base.OnSelectExiting(interactor);

        if(interactor.GetType() == typeof(XRDirectInteractor)){
            handDistanceGrabbable.UnSelected();
        }

    }
}
