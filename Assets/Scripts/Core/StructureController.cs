using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SIB_Interaction{
    public class StructureController : XRGrabInteractable
    {

        /// <summary>
        /// Structure Controller handles the input from the grab handles 
        /// in the structure to solve for the appropriate motion of the
        /// structure. This handles one hand grab and two hand grab, the
        /// latter for scaling. 
        /// </summary>

        private bool scaleGrabberSelected, structureSelected;                   // Keeps track of the grabbed state of the handle
        public float minScale = 1f, maxScale = 1.5f, softScaleBound = 2.5f;    // Minimum and maximum scale of the structure. SoftScaleBound is a multiplier for max and min that gives the bounds during scaling
        public GameObject scaleGrabber, structure, hand1, hand2;                // GameObject References
        private bool twoHandGrab;                                               // Whether in two hand scaling mode, one hand rotation/translation mode
        private float twoHandDistance;                                          // Initial distance between hands in scaling mode
        private Vector3 interactorPosition = Vector3.zero, beginningScale;      // offset interaction position
        private Quaternion interactorRotation = Quaternion.identity;            // offset interaction rotation
        private Vector3 initialHandPosition1, initialHandPosition2;             // controller starting locations
        private Quaternion initialObjectRotation;                               // gameObject rotation
        private Vector3 initialObjectScale, initialObjectDirection;             // gameObject scale, direction of gameObject to midpoint of both controllers



        // extends OnSelectEntering from XRGrabInteractable. Stores initial information from the interactors
        protected override void OnSelectEntering(XRBaseInteractor interactor) {

            base.OnSelectEntering(interactor); // Run this method in parent

            // Store the attach transform of the interactor
            interactorPosition = interactor.attachTransform.localPosition;
            interactorRotation = interactor.attachTransform.localRotation;
            
            // offsets the interactor's attach transform to match the structure's
            bool hasAttach = attachTransform != null;
            interactor.attachTransform.position = hasAttach ? attachTransform.position : transform.position;
            interactor.attachTransform.rotation = hasAttach ? attachTransform.rotation : transform.rotation;

            structureSelected = true;

        }

        // extends OnSelectExiting from XRGrabInteractabe. Resets the structure to its state before grabbing
        protected override void OnSelectExiting(XRBaseInteractor interactor) {
            
            base.OnSelectExiting(interactor); // Run this method in parent

            // Reset the attach transform to its original position
            interactor.attachTransform.localPosition = interactorPosition;
            interactor.attachTransform.localRotation = interactorRotation;

            // Reset variables to zero
            interactorPosition = Vector3.zero;
            interactorRotation = Quaternion.identity;

            structureSelected = false;

        }

        // Update is called once per frame
        void Update()
        {
            scaleGrabber.SetActive(structureSelected);
            
            if(scaleGrabberSelected && structureSelected) {

                if(!twoHandGrab) {

                    attachTargetBoth();

                    twoHandGrab = true;
                }

                // Update scaling and location, then clamp the scale between the minimum and maximum
                updateTargetBoth();
                ClampScale();

            } else {
                
                twoHandGrab = false;

                scaleGrabber.transform.position = gameObject.transform.position;

                SmoothClampScale();
            }
        }

        private void ClampScale() {

            if (gameObject.transform.lossyScale.magnitude < minScale / softScaleBound) {

                gameObject.transform.localScale = gameObject.transform.localScale * (minScale / softScaleBound) / gameObject.transform.localScale.magnitude;

            } else if (gameObject.transform.lossyScale.magnitude > maxScale * softScaleBound) {

                gameObject.transform.localScale = gameObject.transform.localScale * maxScale * softScaleBound / gameObject.transform.localScale.magnitude;

            }
        }

        private void SmoothClampScale() {
            
            if (gameObject.transform.lossyScale.magnitude < minScale) {

                gameObject.transform.localScale = (1 / ((gameObject.transform.localScale.magnitude / minScale - 1) * Time.deltaTime * 5 + 1)) * gameObject.transform.localScale;
            } else if (gameObject.transform.lossyScale.magnitude > maxScale) {

                gameObject.transform.localScale = (1 / ((gameObject.transform.localScale.magnitude / maxScale - 1) * Time.deltaTime * 5 + 1)) * gameObject.transform.localScale;

            }

        }

        private void attachTargetBoth() {
            initialHandPosition1 = hand1.transform.position;
            initialHandPosition2 = hand2.transform.position;
            initialObjectRotation = gameObject.transform.rotation;
            initialObjectScale = gameObject.transform.localScale;
            initialObjectDirection = gameObject.transform.position - (initialHandPosition1 + initialHandPosition2) * 0.5f; 
        }

        private void updateTargetBoth() {
            Vector3 currentHandPosition1 = hand1.transform.position; // current first hand position
            Vector3 currentHandPosition2 = hand2.transform.position; // current second hand position

            Vector3 handDir1 = (initialHandPosition1 - initialHandPosition2).normalized; // direction vector of initial first and second hand position
            Vector3 handDir2 = (currentHandPosition1 - currentHandPosition2).normalized; // direction vector of current first and second hand position 

            Quaternion handRot = Quaternion.FromToRotation(handDir1, handDir2); // calculate rotation based on those two direction vectors

            float currentGrabDistance = Vector3.Distance(currentHandPosition1, currentHandPosition2);
            float initialGrabDistance = Vector3.Distance(initialHandPosition1, initialHandPosition2);
            float p = (currentGrabDistance / initialGrabDistance); // percentage based on the distance of the initial positions and the new positions

            Vector3 newScale = new Vector3(p * initialObjectScale.x, p * initialObjectScale.y, p * initialObjectScale.z); // calculate new object scale with p

            gameObject.transform.rotation = handRot * initialObjectRotation; // add rotation
            gameObject.transform.localScale = newScale; // set new scale
            
            // set the position of the object to the center of both hands based on the original object direction relative to the new scale and rotation
            gameObject.transform.position = (0.5f * (currentHandPosition1 + currentHandPosition2)) + (handRot * (initialObjectDirection * p));

        }

        // Called by XR grab interactable in the structure
        public void ScaleGrabberSelected() {
            scaleGrabberSelected = true;
        }

        // Called by XR grab interactable in the structure
        public void ScaleGrabberDeselected() {
            scaleGrabberSelected = false;
        }
    }
}