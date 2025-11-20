using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

namespace Veridium.Modules.AminoAcids
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class LaserPointer : MonoBehaviour
    {
        private XRInteractionManager manager;
        private XRGrabInteractable interactable;
        public GameObject leftHand;
        public GameObject rightHand;

        void Start()
        {
            interactable = GetComponent<XRGrabInteractable>();
            if (!interactable) return;

            manager = interactable.interactionManager;

            interactable.activated.AddListener(OnActivated);
            interactable.selectEntered.AddListener(OnSelected);
            interactable.selectExited.AddListener(OnDeselected);
            // interactable.deactivated.AddListener(OnDeactivated);
        }

        private void OnActivated(ActivateEventArgs args)
        {
            // if (!interactor.hasHover) return;

            // if (interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            // {
            //     if (hit.collider.TryGetComponent(out Atom atom))
            //     {
            //         atom.ToggleHighlight();
            //         return;
            //     }
            // }

            // IXRActivateInteractable target = interactor.interactablesHovered.FirstOrDefault() as IXRActivateInteractable;
            // ActivateEventArgs newArgs = new ActivateEventArgs
            // {
            //     interactorObject = interactor,
            //     interactableObject = target
            // };
            // target.OnActivated(newArgs);
            // lineRenderer.material = lineRenderer.material == neutralMaterial ? selectMaterial : neutralMaterial;
        }

        private void OnSelected(SelectEnterEventArgs args)
        {
            // lineRenderer.enabled = true;
            // lineVisual.enabled = true;
            if (((args.interactorObject as XRDirectInteractor).xrController as XRController).controllerNode == XRNode.LeftHand)
            {
                leftHand.SetActive(true);
            }
            else
            {
                rightHand.SetActive(true);
            }
        }
        
        private void OnDeselected(SelectExitEventArgs args)
        {
            // lineVisual.enabled = false;
            // lineRenderer.enabled = false;
            if (((args.interactorObject as XRDirectInteractor).xrController as XRController).controllerNode == XRNode.LeftHand)
            {
                leftHand.SetActive(false);
            }
            else
            {
                rightHand.SetActive(false);
            }
        }
    }
}