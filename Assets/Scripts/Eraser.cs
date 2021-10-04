using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Eraser : MonoBehaviour
{
    XRGrabInteractable grabInteractable => GetComponent<XRGrabInteractable>();

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(Grab);
        grabInteractable.selectExited.AddListener(Drop);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(Grab);
        grabInteractable.selectExited.RemoveListener(Drop);
    }

    private void Grab(SelectEnterEventArgs args)
    {

    }

    private void Drop(SelectExitEventArgs args)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGERED: " + other.gameObject.name);
        if (other.tag == "DrawPart")
        {
            Destroy(other.gameObject);
        }
    }
}
