using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SIB_Interaction{
    public class ElementLoader : XRSocketInteractor
    {

        /// <summary>
        /// Element Loader is attached to a game object with a collider and allows element tiles to snap
        /// into the socket and prompts the structureBase to build that structure.
        /// Extends XRSocketInteractor
        /// </summary>

        public PTElement heldElement;
        public StructureBase structureBase;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        // Overrides OnSelectEntering, used to detect when element tiles are added to the slot
        protected override void OnSelectEntering(XRBaseInteractable interactable){

            base.OnSelectEntering(interactable);

            heldElement = interactable.gameObject.GetComponent<PTElement>();

            structureBase.ElementAdded(heldElement);
        }

        // Overrides OnSelectExiting, used to detect when element tiles are removed from the slot
        protected override void OnSelectExiting(XRBaseInteractable interactable){

            base.OnSelectExiting(interactable);

            structureBase.ElementRemoved();

            heldElement = null;
        }

    }
}