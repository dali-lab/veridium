using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Veridium_Interaction{
    public class ElementLoader : XRSocketInteractor
    {

        /// <summary>
        /// Element Loader is attached to a game object with a collider and allows element tiles to snap
        /// into the socket and prompts the structureBase to build that structure.
        /// Extends XRSocketInteractor
        /// </summary>

        public PTElement heldElement;           // Current element in the slot
        public StructureBase structureBase;     // StructureBase that this loads elements for
        public Animator insertedAnimation;      // animator to enable when the element is inserted
        private int layerMask;


        // Overrides OnSelectEntering, used to detect when element tiles are added to the slot
        protected override void OnSelectEntering(XRBaseInteractable interactable){

            base.OnSelectEntering(interactable);

            heldElement = interactable.gameObject.GetComponent<PTElement>();

            if(heldElement != null){

                structureBase.ElementAdded(heldElement);
                GetComponent<AudioSource>().Play();
                if(insertedAnimation != null) insertedAnimation.SetBool("circuitActive", true);

            }
        }

        // Overrides OnSelectExiting, used to detect when element tiles are removed from the slot
        protected override void OnSelectExiting(XRBaseInteractable interactable){

            base.OnSelectExiting(interactable);

            structureBase.ElementRemoved();

            heldElement = null;

            if(insertedAnimation != null) insertedAnimation.SetBool("circuitActive", false);
        }

        public void Lock(){
            heldElement.Lock();
        }

        public void Unlock(){
            heldElement.Unlock();
        }

        public void ResetStructure(){

            structureBase.ElementRemoved();

            if(heldElement != null) structureBase.ElementAdded(heldElement);
        }

    }
}
