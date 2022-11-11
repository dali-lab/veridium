using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

[System.Serializable]
public class GripButtonEvent : UnityEvent { }

public class GripButtonWatcher : MonoBehaviour
{
    public GripButtonEvent gripButtonPress;

    private bool lastButtonState = false;
    private List<InputDevice> devicesWithGripButton;

    public bool bothPressed = false;

    [SerializeField] int totalGripsPressed = 0;

    private void Awake()
    {
        if (gripButtonPress == null)
        {
            gripButtonPress = new GripButtonEvent();
        }

        devicesWithGripButton = new List<InputDevice>();
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
        devicesWithGripButton.Clear();
    }

    private void InputDevices_deviceConnected(InputDevice device)
    {
        bool discardedValue;
        if (device.TryGetFeatureValue(CommonUsages.gripButton, out discardedValue))
        {
            devicesWithGripButton.Add(device); // Add any devices that have a grip button.
        }
    }

    private void InputDevices_deviceDisconnected(InputDevice device)
    {
        if (devicesWithGripButton.Contains(device))
            devicesWithGripButton.Remove(device);
    }

    void Update()
    {
        int currentGripsPressed = 0;
        foreach (var device in devicesWithGripButton)
        {
            bool gripButtonState = false;
            device.TryGetFeatureValue(CommonUsages.gripButton, out gripButtonState); // gripButtonState is the value of the gripButton
            if (gripButtonState)
            {
                currentGripsPressed++;
                Debug.Log("Added one to grips pressed");
            }
        }

        if (currentGripsPressed == 2 && totalGripsPressed < 2)
        {
            gripButtonPress.Invoke();
        }
        totalGripsPressed = currentGripsPressed;
    }

    public void Pressed()
    {
        Debug.Log("both grip buttons pressed!");
    }
}