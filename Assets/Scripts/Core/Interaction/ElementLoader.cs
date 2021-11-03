using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SIB_Interaction{
    public class ElementLoader : XRSocketInteractor
    {
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

        protected override void OnSelectEntering(XRBaseInteractable interactable){

            base.OnSelectEntering(interactable);

            heldElement = interactable.gameObject.GetComponent<PTElement>();
            structureBase.ElementAdded(heldElement);

        }

        protected override void OnSelectExiting(XRBaseInteractable interactable){

            base.OnSelectExiting(interactable);

            structureBase.ElementRemoved();

            heldElement = null;
        }
    }
}
