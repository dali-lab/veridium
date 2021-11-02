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

            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = interactable.gameObject.GetComponent<PTElementCollider>().element.ToString();

            heldElement = interactable.gameObject.GetComponent<PTElementCollider>().element;
            structureBase.ElementAdded(heldElement);

        }

        protected override void OnSelectExiting(XRBaseInteractable interactable){

            base.OnSelectExiting(interactable);

            structureBase.ElementRemoved();

            heldElement = null;

        }
    }
}
