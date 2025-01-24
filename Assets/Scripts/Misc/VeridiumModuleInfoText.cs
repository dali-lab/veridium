using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Veridium.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

namespace Veridium.Modules
{
    public class VeridiumModuleInfoText : MonoBehaviour
    {
        public VeridiumModule module;

        public HandDistanceGrabbable handDistanceGrabbable;

        public TextMesh nameTextCenter;
        public TextMesh nameTextLeft;
        public TextMesh nameTextRight;

        public TextMeshPro descriptionTextLeft;
        public TextMeshPro descriptionTextRight;

        void Start() {
            if (module == null) {
                Debug.LogError("Module not set");
                return;
            }

            nameTextCenter.text = module.displayName;
            nameTextLeft.text = module.displayName;
            nameTextRight.text = module.displayName;

            descriptionTextLeft.text = module.description;
            descriptionTextRight.text = module.description;

            /*nameTextCenter.gameObject.SetActive(false);
            nameTextLeft.gameObject.SetActive(false);
            nameTextRight.gameObject.SetActive(false);
            descriptionTextLeft.gameObject.SetActive(false);
            descriptionTextRight.gameObject.SetActive(false);*/
        }

        void Update() {
            if (handDistanceGrabbable.isSelected && handDistanceGrabbable.GetOldestInteractorSelecting() is HandDistanceGrabber) {
                Debug.Log(handDistanceGrabbable.GetOldestInteractorSelecting());
                HandDistanceGrabber grabber = (HandDistanceGrabber) handDistanceGrabbable.GetOldestInteractorSelecting();

                if (grabber.hand == Hand.Left) {
                    nameTextLeft.gameObject.SetActive(false);
                    nameTextRight.gameObject.SetActive(true);
                    descriptionTextLeft.gameObject.SetActive(false);
                    descriptionTextRight.gameObject.SetActive(true);
                } else {
                    nameTextLeft.gameObject.SetActive(true);
                    nameTextRight.gameObject.SetActive(false);
                    descriptionTextLeft.gameObject.SetActive(true);
                    descriptionTextRight.gameObject.SetActive(false);
                }

                nameTextCenter.gameObject.SetActive(false);

            } else if (handDistanceGrabbable.isHovered()) {
                nameTextCenter.gameObject.SetActive(true);
                nameTextLeft.gameObject.SetActive(false);
                nameTextRight.gameObject.SetActive(false);
                descriptionTextLeft.gameObject.SetActive(false);
                descriptionTextRight.gameObject.SetActive(false);
            } else {
                nameTextCenter.gameObject.SetActive(false);
                nameTextLeft.gameObject.SetActive(false);
                nameTextRight.gameObject.SetActive(false);
                descriptionTextLeft.gameObject.SetActive(false);
                descriptionTextRight.gameObject.SetActive(false);
            }
        }
    }
}

