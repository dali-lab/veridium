using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public enum HandType
{
    Left, Right
}

public class XRHandController : MonoBehaviour
{
    [SerializeField] public HandType handType;
    [SerializeField] private float thumbMoveSpeed = 0.2f;

    private Animator animator;
    private InputDevice inputDevice;
    private float thumbValue;
    private float indexFingerValue;
    private float threeFingersValue;

    // private bool useCustomHandAnim = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        inputDevice = GetInputDevice();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject grabbed = CheckHandInteractor();
        if (grabbed) // if there is an object grabbed
        {
            CustomAnimateHand(grabbed.tag);
        }
        else
        {
            AnimateHand();
        }
    }

    private InputDevice GetInputDevice()
    {
        InputDeviceCharacteristics controllerCharacteristic = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller;
        if (handType == HandType.Left)
        {
            controllerCharacteristic = controllerCharacteristic | InputDeviceCharacteristics.Left;
        }
        else
        {
            controllerCharacteristic = controllerCharacteristic | InputDeviceCharacteristics.Right;
        }

        List<InputDevice> inputDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristic, inputDevices);
        return inputDevices[0];
    }

    private GameObject CheckHandInteractor()
    {
        GameObject grabbed = this.transform.parent.GetComponent<XRDirectInteractor>()?.selectTarget?.gameObject; // nullish coalescing to check for grabbed gameobject
        return grabbed;
    }

    void AnimateHand()
    {   
        ResetCustomAnimations();
        inputDevice.TryGetFeatureValue(CommonUsages.trigger, out indexFingerValue);
        inputDevice.TryGetFeatureValue(CommonUsages.grip, out threeFingersValue);
        inputDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out bool primaryTouched);
        inputDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out bool secondaryTouched);
        if (primaryTouched || secondaryTouched)
        {
            thumbValue += thumbMoveSpeed;
        }
        else
        {
            thumbValue -= thumbMoveSpeed;
        }

        thumbValue = Mathf.Clamp01(thumbValue);

        animator.SetFloat("IndexFinger", indexFingerValue);
        animator.SetFloat("ThreeFingers", threeFingersValue);
        animator.SetFloat("Thumb", thumbValue);
    }

    void CustomAnimateHand(string objectTag)
    {
        ResetDefaultAnimations();
        // Debug.Log("using custom animation");

        inputDevice.TryGetFeatureValue(CommonUsages.grip, out threeFingersValue);

        switch(objectTag)
        {
            case "Pencil":
                animator.SetFloat("PencilGrab", threeFingersValue);
                break;
            case "Eraser":
                animator.SetFloat("EraserGrab", threeFingersValue);
                break;
            case "Marker":
                animator.SetFloat("MarkerGrab", threeFingersValue);
                break;
            default:
                break;
        }
        
    }

    void ResetCustomAnimations()
    {
        animator.SetFloat("PencilGrab", 0);
        animator.SetFloat("EraserGrab", 0);
        animator.SetFloat("MarkerGrab", 0);
    }

    void ResetDefaultAnimations()
    {
        animator.SetFloat("IndexFinger", 0);
        animator.SetFloat("ThreeFingers", 0);
        animator.SetFloat("Thumb", 0);
    }

}
