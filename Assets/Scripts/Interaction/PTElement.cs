using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium_Core;

namespace Veridium_Interaction{
    
    public class PTElement : MonoBehaviour
    {

        /// <summary>
        /// Holds information about the element tile and returns home
        /// When dropped.
        /// </summary>

        public string elementName;                              // Name of the element
        public GameObject home;                                 // GameObject for this one to snap back to when dropped
        private float unHeldTimer = 0f;                         // How long this has not been interacted
        public float maxUnHeldTime = 1f;                        // How long before this snaps back home
        private bool interacted = true;                         // Whether this element is held or in a socket
        public CellType type = CellType.CUBIC;                  // The greater cell structure of the element
        public CellVariation variation = CellVariation.SIMPLE;  // The variation on the cell structure of the element

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

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


                    GetComponent<AudioSource>().Play();

                }

            }
            
        }

        // Called by the grab interactable
        public void Interacted(){
            interacted = true;
            unHeldTimer = 0;
        }

        // Called by the grab interactable
        public void UnInteracted(){
            interacted = false;
        }

        public void Lock(){
            GetComponent<XRGrabInteractable_Lockable>().Lock();
        }

        public void Unlock(){
            GetComponent<XRGrabInteractable_Lockable>().Unlock();
        }
    }
}
