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

            if(socketInteractor.selectTarget != null){
                if(socketInteractor.selectTarget.GetComponent<PTElement>() != null && (elementName == "" || socketInteractor.selectTarget.GetComponent<PTElement>().elementName == elementName)) CompleteAction();
            }
        }

        void Inserted(SelectEnterEventArgs args){

            if(args.interactable.GetComponent<PTElement>() != null && (elementName == "" || args.interactable.GetComponent<PTElement>().elementName == elementName)) CompleteAction();
        }

    }
}
