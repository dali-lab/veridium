using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ToothGrab : MonoBehaviour
{
    private Transform grabHandle;
    private Quaternion originalGrabHandleRotation;
    private Vector3 originalGrabHandlePosition;
    private float originalMaxRaycastDistance;

    XRGrabInteractable grabInteractable => GetComponent<XRGrabInteractable>();

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(Grab);
        grabInteractable.selectExited.AddListener(Drop);
        grabInteractable.hoverEntered.AddListener(HoverEnter);
        grabInteractable.hoverExited.AddListener(HoverExit);
    }
    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(Grab);
        grabInteractable.selectExited.RemoveListener(Drop);
        grabInteractable.hoverEntered.RemoveListener(HoverEnter);
        grabInteractable.hoverExited.RemoveListener(HoverExit);
    }

    private void Awake()
    {
        grabHandle = GetComponent<XRGrabInteractable>().attachTransform;
        originalGrabHandlePosition = grabHandle.position;
        originalGrabHandleRotation = grabHandle.rotation;
    }

    public void Grab(SelectEnterEventArgs args)
    {
        grabHandle.transform.rotation = args.interactor.transform.rotation;
        grabHandle.transform.position = args.interactor.transform.position;
        // originalMaxRaycastDistance = args.interactor.GetComponent<XRRayInteractor>().maxRaycastDistance;
        // args.interactor.GetComponent<XRRayInteractor>().maxRaycastDistance = 0; // equivalent to disabling it
        // args.interactor.GetComponent<XRInteractorLineVisual>().enabled = false;
    }

    public void Drop(SelectExitEventArgs args)
    {
        grabHandle.position = originalGrabHandlePosition;
        grabHandle.rotation = originalGrabHandleRotation;
        // args.interactor.GetComponent<XRRayInteractor>().maxRaycastDistance = originalMaxRaycastDistance; // re-enable
        // args.interactor.GetComponent<XRInteractorLineVisual>().enabled = true;
    }

    public void HoverEnter(HoverEnterEventArgs args)
    {
    }

    public void HoverExit(HoverExitEventArgs args)
    {
    }

}
