using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium.Animation;
using Veridium.Interaction;
using UnityEngine.XR;
using UnityEngine.Events;
using Veridium.Modules.ElementStructures;



[System.Serializable]
public class PrimaryButtonEvent : UnityEvent<bool> { }

public class Testing : MonoBehaviour
{
    public PrimaryButtonEvent primaryButtonPress;

    private bool lastButtonState = false;
    private List<InputDevice> devicesWithPrimaryButton;

    private bool buttonState = false;

    [SerializeField] ElementLoader elementLoader;

    private void Start()
    {
        Debug.Log("CURRENT LANGUAGE OR TAG: " + Language.language);
    }

    private void Awake()
    {
        if (primaryButtonPress == null)
        {
            primaryButtonPress = new PrimaryButtonEvent();
        }

        devicesWithPrimaryButton = new List<InputDevice>();
    }

    void OnEnable()
    {
        List<InputDevice> allDevices = new List<InputDevice>();
        InputDevices.GetDevices(allDevices);
        foreach (InputDevice device in allDevices)
            InputDevices_deviceConnected(device);

        InputDevices.deviceConnected += InputDevices_deviceConnected;
        InputDevices.deviceDisconnected += InputDevices_deviceDisconnected;
    }

    private void OnDisable()
    {
        InputDevices.deviceConnected -= InputDevices_deviceConnected;
        InputDevices.deviceDisconnected -= InputDevices_deviceDisconnected;
        devicesWithPrimaryButton.Clear();
    }

    private void InputDevices_deviceConnected(InputDevice device)
    {
        bool discardedValue;
        if (device.TryGetFeatureValue(CommonUsages.primaryButton, out discardedValue))
        {
            devicesWithPrimaryButton.Add(device); // Add any devices that have a primary button.
        }
    }

    private void InputDevices_deviceDisconnected(InputDevice device)
    {
        if (devicesWithPrimaryButton.Contains(device))
            devicesWithPrimaryButton.Remove(device);
    }

    void Update()
    {
        bool tempState = false;
        foreach (var device in devicesWithPrimaryButton)
        {
            bool primaryButtonState = false;
            tempState = device.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonState) // did get a value
                        && primaryButtonState // the value we got
                        || tempState; // cumulative result from other controllers
        }

        if (tempState != lastButtonState) // Button state changed since last frame
        {
            primaryButtonPress.Invoke(tempState);
            lastButtonState = tempState;
        }
    }

    public void nextThing()
    {
        if (buttonState)
        {
            buttonState = !buttonState;
            return;
        }

        Debug.Log("Primary button press event");

        if (elementLoader.heldElement == null) return;

        if (elementLoader.lectureNameToGO.ContainsKey(elementLoader.heldElement.name)) // if the held element has a lecture
        {
            // play 
            elementLoader.lectureNameToGO[elementLoader.heldElement.name].GetComponent<AnimSequence>().PlayNextSegment();
        }
        buttonState = !buttonState;
    }
}