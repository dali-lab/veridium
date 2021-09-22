// Copyright (c) MikeNspired. All Rights Reserved.

using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace MikeNspired.UnityXRHandPoser
{
    /// <summary>
    /// Temporary class to get the trigger and grip input.
    /// A more robust solution will be released when unity integrates their new input system into UnityXR.
    /// </summary>
    public class XRControllerButtons : MonoBehaviour
    {
        [SerializeField] private XRController xrController;
        [SerializeField] private XRBaseControllerInteractor xrBaseInteractor;
        [SerializeField] private HandType handType;

        public UnityEventFloat OnTriggerValue;
        public UnityEvent OnGripPressed;
        public UnityEvent OnGripRelease;

        public bool gripValue;
        public float triggerValue;

        private InputDevice inputDevice;
        private bool IsGripped;

        public enum HandType
        {
            Left, Right
        }

        private void Start()
        {
            OnValidate();
            // if (xrController)
            //     inputDevice = xrController.inputDevice;
            // else
            //     enabled = false;
            if (xrBaseInteractor)
                inputDevice = GetInputDevice();
            else
                enabled = false;
        }

        private void OnValidate()
        {
            // if (!xrController) xrController = GetComponentInParent<XRController>();
            if (!xrBaseInteractor) xrBaseInteractor = GetComponentInParent<XRBaseControllerInteractor>();
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
            // Debug.Log("here");
            // Debug.Log(inputDevices.Count);
            // Debug.Log(inputDevices[0]);
            return inputDevices[0];
        }

        private void Update()
        {
            // inputDevice = GetInputDevice();
            inputDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerValue);

            OnTriggerValue.Invoke(triggerValue);

            if (inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out gripValue))
            {
                // Debug.Log("grip Value is: " + gripValue);
                if (!IsGripped && gripValue)
                {
                    // Debug.Log("here");
                    IsGripped = true;
                    OnGripPressed.Invoke();
                }
                else if (IsGripped && !gripValue)
                {
                    IsGripped = false;
                    OnGripRelease.Invoke();
                }
            }
        }
    }


    [System.Serializable]
    public class UnityEventFloat : UnityEvent<float>
    {
    }
}