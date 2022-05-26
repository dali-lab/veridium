using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using Veridium_Interaction;

namespace Veridium_Animation{
    [System.Serializable]
    public class AwaitInsert : AwaitUserBase{

        public XRSocketInteractor socketInteractor;
        public string elementName = "";

        public override void Play()
        {
            base.Play();

            // Add listeners to the proper events
            socketInteractor.selectEntered.AddListener(Inserted);

            // Check if the user has already completed the action
            if(socketInteractor.selectTarget != null){
                if(socketInteractor.selectTarget.GetComponent<PTElement>() != null && (elementName == "" || socketInteractor.selectTarget.GetComponent<PTElement>().elementName == elementName)) CompleteAction();
            }
            
        }

        void Inserted(SelectEnterEventArgs args){

            if(args.interactable.GetComponent<PTElement>() != null && (elementName == "" || args.interactable.GetComponent<PTElement>().elementName == elementName)) CompleteAction();
        }
    }
}