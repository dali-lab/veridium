// Copyright (c) MikeNspired. All Rights Reserved.

using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace MikeNspired.UnityXRHandPoser
{
    /// <summary>
    /// Forces the grabbing type from holding grip button to hold onto object, clicking the grip button once or any other option.
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable))]
    public class InteractableForceActionTrigger : MonoBehaviour
    {
        private XRGrabInteractable interactable;

        public XRBaseControllerInteractor.InputTriggerType ActionTriggerType;

        private XRBaseControllerInteractor.InputTriggerType originalActionTriggerType;

        // COMMENTED OUT BECAUSE DEPRECATED
        // public XRBaseControllerInteractor.SelectActionTriggerType ActionTriggerType;

        // private XRBaseControllerInteractor.SelectActionTriggerType originalActionTriggerType;

        // Start is called before the first frame update
        void Start()
        {
            OnValidate();
            interactable.selectEntered.AddListener(args => SetActionTrigger(args.interactor.GetComponent<XRBaseControllerInteractor>()));
            interactable.selectExited.AddListener(args => ReturnToOriginalActionTrigger(args.interactor.GetComponent<XRBaseControllerInteractor>()));
        }

        private void SetActionTrigger(XRBaseControllerInteractor controller)
        {
            originalActionTriggerType = controller.selectActionTrigger;
            controller.selectActionTrigger = ActionTriggerType;
        }

        private void ReturnToOriginalActionTrigger(XRBaseControllerInteractor controller)
        {
            controller.selectActionTrigger = originalActionTriggerType;
        }


        private void OnValidate()
        {
            interactable = GetComponent<XRGrabInteractable>();
        }
    }
}

