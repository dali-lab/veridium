using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Veridium.Interaction;

namespace Veridium.Animation{
    public class Await_Insert : AwaitUserBase
    {

        public XRSocketInteractor socketInteractor;
        public string elementName = "";

        void Awake(){

            socketInteractor.selectEntered.AddListener(Inserted);

        }

        public override void Play()
        {
            base.Play();

            if(socketInteractor.GetOldestInteractableSelected() != null){
                if(socketInteractor.GetOldestInteractableSelected().transform.GetComponent<PTElement>() != null && (elementName == "" || socketInteractor.GetOldestInteractableSelected().transform.GetComponent<PTElement>().elementName == elementName)) CompleteAction();
            }
        }

        void Inserted(SelectEnterEventArgs args){

            if(args.interactableObject.transform.GetComponent<PTElement>() != null && (elementName == "" || args.interactableObject.transform.GetComponent<PTElement>().elementName == elementName)) CompleteAction();
        }

    }
}
