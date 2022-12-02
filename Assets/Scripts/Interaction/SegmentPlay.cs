using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

namespace Veridium_Interaction
{
    public class SegmentPlay : HandDistanceGrabbable
    {

        public float maxHeldTime = 1f;                        // How long before we exit the scene
        private float heldTimer = 0f;

        private bool interacted = true;                         // Whether this element is held or in a socket
        private bool isReset;

        public Image progressBar;
        public GameObject playButton;
        public GameObject grabbableSphere;
        public Animator sphereAnim;
        public Transform resetPoint;
        public UnityEvent onInteractionStart;

        public bool isComplete;

        // Start is called before the first frame update
        void Start()
        {
            isReset = true;
            isComplete = false;
            // sphereAnim.SetBool("spawnSphere", true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            Debug.Log("Enabling segment play");

            isReset = true;
            isComplete = false;
        }

        // Update is called once per frame
        void Update()
        {

            // Increment the timer if not interacted
            if (!interacted && grabbableSphere != null)
            {
                if (!isReset)
                {
                    grabbableSphere.transform.position = resetPoint.position;
                    grabbableSphere.transform.rotation = resetPoint.rotation;
                    isReset = true;
                    heldTimer = 0f;
                    progressBar.enabled = false;
                }
            }
            else if (GetComponent<XRGrabInteractable>().selectingInteractor is XRDirectInteractor)
            {
                if (isReset)
                {
                    isReset = false;
                }

                if (heldTimer < maxHeldTime)
                {
                    heldTimer += Time.deltaTime;
                    progressBar.enabled = true;
                    progressBar.fillAmount = heldTimer/maxHeldTime;
                    isComplete = false;
                    //progressBar.fillAmount = Mathf.Lerp(0, 100, heldTimer/maxHeldTime);

                }
                else if (!isComplete)
                {

                    progressBar.enabled = false;
                    // invoke event
                    sphereAnim.SetBool("despawnSphere", true); // this shrinks the button and destroys it
                    onInteractionStart.Invoke();
                    isComplete = true;
                }
            }
        }

        // Called by the grab interactable
        protected override void OnSelectEntering(XRBaseInteractor interactor)
        {

            base.OnSelectEntering(interactor); // Run this method in parent

            interacted = true;
        }

        // Called by the grab interactable
        protected override void OnSelectExiting(XRBaseInteractor interactor)
        {
            base.OnSelectExiting(interactor); // Run this method in parent

            interacted = false;
            heldTimer = 0f;
        }

        public override bool IsSelectableBy(XRBaseInteractor interactor)
        {
            bool baseCase = base.IsSelectableBy(interactor);

            if (!(interactor is XRDirectInteractor)) return false;

            return baseCase;
        }
    }
}
