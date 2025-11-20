using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Veridium.Modules.AminoAcids
{
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class XRButton : MonoBehaviour
    {
        private XRSimpleInteractable interactable;
        public Transform buttonVisual;
        public Vector3 basePosition;
        public Vector3 pressedPosition;

        // Start is called before the first frame update
        void Start()
        {
            interactable = GetComponent<XRSimpleInteractable>();
            interactable.firstHoverEntered.AddListener(FirstHoverEntered);
            interactable.lastHoverExited.AddListener(LastHoverExited);
        }

        private void FirstHoverEntered(HoverEnterEventArgs args)
        {
            print("Button Pressed");
            buttonVisual.localPosition = pressedPosition;
        }

        private void LastHoverExited(HoverExitEventArgs args)
        {
            print("Button Released");
            buttonVisual.localPosition = basePosition;
        }
    }
}