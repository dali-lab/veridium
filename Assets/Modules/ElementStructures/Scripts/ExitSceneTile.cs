using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using Veridium.Interaction;

namespace Veridium.Modules.ElementStructures
{
    public class ExitSceneTile : HandDistanceGrabbable
    {

        public GameObject home;                                 // GameObject for this one to snap back to when dropped
        private float unHeldTimer = 0f;                         // How long this has not been interacted
        public float maxUnHeldTime = 1f;                        // How long before this snaps back home
        public float timeUntilExit = 3.5f;                        // How long before we exit the scene
        private bool interacted = true;                         // Whether this element is held or in a socket
        private float heldTimer = 0f;
        public TMPro.TextMeshPro countdownText;
        public TMPro.TextMeshPro stationaryCountdown;
        public UnityEvent exitEvent;

        // Start is called before the first frame update
        void Start()
        {

            countdownText.text = "";
            stationaryCountdown.text = "";
            
        }

        // Update is called once per frame
        void Update()
        {

            countdownText.text = "";
            stationaryCountdown.text = "";

            // Increment the timer if not interacted
            if(!interacted){

                if(unHeldTimer < maxUnHeldTime){

                    unHeldTimer += Time.deltaTime;

                } else {

                    // Teleport home when not interacted for long enough
                    unHeldTimer = 0f;
                    gameObject.transform.position = home.transform.position;
                    gameObject.transform.rotation = home.transform.rotation;
                    if(GetComponent<Rigidbody>() != null){
                        GetComponent<Rigidbody>().velocity = Vector3.zero;
                        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    }

                }
            } else if(GetComponent<XRGrabInteractable>().selectingInteractor is XRDirectInteractor) {

                if(heldTimer < timeUntilExit){

                    heldTimer += Time.deltaTime;

                    if(heldTimer > 0.2f) {
                        countdownText.text = "Exit in " + (timeUntilExit - heldTimer).ToString("F0") + "...";
                        stationaryCountdown.text = (timeUntilExit - heldTimer).ToString("F0") + "...";
                    }

                } else {

                    heldTimer = 0f;
                    ExitToMenu();

                }
            }
        }

        // Called by the grab interactable
        protected override void OnSelectEntering(XRBaseInteractor interactor){

            base.OnSelectEntering(interactor); // Run this method in parent

            interacted = true;
            unHeldTimer = 0f;
        }

        // Called by the grab interactable
        protected override void OnSelectExiting(XRBaseInteractor interactor) {
            
            base.OnSelectExiting(interactor); // Run this method in parent

            interacted = false;
            heldTimer = 0f;
        }

        public void ExitToMenu()
        {
            exitEvent.Invoke();
        }



        //public override bool IsSelectableBy(XRBaseInteractor interactor){
        //    bool baseCase = base.IsSelectableBy(interactor);

        //    if(!(interactor is XRDirectInteractor) && interactor.gameObject != home) return false;

        //    return baseCase;
        //}
    }
}
