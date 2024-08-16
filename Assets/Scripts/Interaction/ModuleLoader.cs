using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Veridium.Modules 
{
    [RequireComponent(typeof(AudioSource))]
    public class ModuleLoader : XRSocketInteractor
    {
        public FadeScreen fadeScreen;

        protected override void OnSelectEntering(XRBaseInteractable interactable){
            Debug.Log("OnSelectEntered() on ModuleLoader");
            base.OnSelectEntering(interactable);
            if (interactable.transform.parent.TryGetComponent(out VeridiumModule module))
            {
                Debug.Log("VeridiumModule found.");
                GetComponent<AudioSource>().Play();
                fadeScreen.fadeThenLoadScene(module.GetScenePath());
            } else {
                
                Debug.Log("No VeridiumModule found.");
            }
        }

    }
}
