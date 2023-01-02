using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Veridium_Interaction{

    public class HandDistanceGrabber : XRDirectInteractor
    {
        /// <summary>
        /// HandDistanceGrabber should be attached to a game object with a direct interactor
        /// and a sphere collider. This script requires that there be a DistanceGrab layer.
        /// All distance grabbables should be assigned to that layer. grabBegun and grabEnded
        /// should be added to the relevant callbacks in the direct interactor.
        /// </summary>

        public float rayDistance = 3f;              // Distance to detect DistanceGrabbables
        public bool distanceGrabActive = true;      // Modify this to enable or disable distance grab
        private int layerMask;                      // Collision filter for sphere cast
        private bool grabbing;                      // Whether the direct interactor is grabbing
        private Vector3 colliderOriginalCenter;     // The original offset of the sphere collider to move back to
        public Vector3 hitLocation;
        public float hitDistance;
        public bool distanceGrabbed;


        // Start is called before the first frame update
        void Start()
        {
            // Prepare the layer mask for the sphere trace collision filter
            layerMask = 1 << LayerMask.NameToLayer("DistanceGrab");

            // Store the original center
            colliderOriginalCenter = GetComponent<SphereCollider>().center;
            
        }

        // Update is called once per frame
        void Update()
        {

            // Only grab if not grabbing and distance grab is activated
            if (distanceGrabActive && !grabbing)
            {
                // Perform the sphere cast
                RaycastHit hit;
                bool hitted = Physics.SphereCast(transform.position, 0.05f, transform.forward, out hit, Mathf.Infinity, layerMask);

                // Set hovered if the sphere cast hit a DistanceGrabbable
                HandDistanceGrabbable hovered = null;
                if (hitted && hit.collider.gameObject.GetComponent<HandDistanceGrabbable>() != null)
                {
                    hovered = hit.collider.gameObject.GetComponent<HandDistanceGrabbable>();

                    hitDistance = hit.distance;

                    hitLocation = hit.point;

                } else
                {
                    hovered = null;

                    hitDistance = 0f;

                    hitLocation = Vector3.zero;
                }

                // Move the direct interactor's collider to the DistanceGrabbable's location
                if (hovered != null)
                {
                    GetComponent<SphereCollider>().center = transform.InverseTransformPoint(hovered.transform.position);
                }
                else {
                    // Move the collider back if not hovering
                    GetComponent<SphereCollider>().center = colliderOriginalCenter;  
                }

            } else {
                // Make sure the collider moves back when distance grabbing is not active
                GetComponent<SphereCollider>().center = colliderOriginalCenter;
            }
        }

        // Updates whether the direct interactor is grabbing.
        protected override void OnSelectEntered(XRBaseInteractable interactable)
        {
            base.OnSelectEntered(interactable);

            grabbing = true;

            if (hitDistance != 0) distanceGrabbed = true;

            GetComponent<AudioSource>().Play();
        }

        // Updates whether the direct interactor is grabbing.
        protected override void OnSelectExited(XRBaseInteractable interactable)
        {
            base.OnSelectExited(interactable);

            grabbing = false;

            distanceGrabbed = false;
        }

        // check if the controller is grabbing anything
        public bool grabbed()
        {
            return grabbing;
        }
    }
}
