using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace MikeNspired.UnityXRHandPoser
{
    public class TeleportRayEnabler : MonoBehaviour
    {
        public XRController leftController, rightController;

        public InputHelpers.Button activationButton;

        public float activationThreshold = .5f;

        private SnapTurnProviderBase snapTurn;

        private void Start()
        {
            snapTurn = GetComponent<SnapTurnProviderBase>();
            if (leftController)
            {
                leftController.GetComponent<XRBaseControllerInteractor>().selectExited.AddListener(DisableLeftHand);
                leftController.gameObject.SetActive(false);
            }

            if (rightController)
            {
                rightController.GetComponent<XRBaseControllerInteractor>().selectExited.AddListener(DisableRightHand);
                rightController.gameObject.SetActive(false);
            }
        }

        private void DisableLeftHand(SelectExitEventArgs interactable)
        {
            StartCoroutine(DisableInteractable(leftController.gameObject));
            snapDisabled = false;
        }

        private void DisableRightHand(SelectExitEventArgs interactable)
        {
            StartCoroutine(DisableInteractable(rightController.gameObject));
            snapDisabled = false;
        }

        private IEnumerator DisableInteractable(GameObject go)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            go.SetActive(false);
        }

        private void Update()
        {
            if (leftController)
                CheckController(leftController);
            if (rightController)
                CheckController(rightController);

            snapTurn.enabled = !snapDisabled;
        }

        private bool snapDisabled;

        private void CheckController(XRController controller)
        {
            controller.inputDevice.IsPressed(activationButton, out bool isActive, activationThreshold);
            if (isActive)
            {
                controller.gameObject.SetActive(true);
                snapDisabled = true;
            }

            controller.inputDevice.IsPressed(activationButton, out isActive, .1f);
            if (!isActive)
            {
                snapDisabled = false;
                controller.gameObject.SetActive(false);
            }
        }
    }
}