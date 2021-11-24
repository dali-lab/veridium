using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DropReturnToHome : MonoBehaviour
{
    private Quaternion originalRotation;
    private Vector3 originalPosition;
    private XRGrabInteractable grabInteractable => GetComponent<XRGrabInteractable>();

    private void OnEnable()
    {
        originalRotation = transform.rotation;
        originalPosition = transform.position;
        grabInteractable.selectExited.AddListener(Drop);
    }

    private void OnDisable()
    {
        grabInteractable.selectExited.RemoveListener(Drop);
    }

    private void Drop(SelectExitEventArgs args)
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }
}
