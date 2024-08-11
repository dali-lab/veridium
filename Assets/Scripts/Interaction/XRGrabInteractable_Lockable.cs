using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Veridium_Interaction{
    public class XRGrabInteractable_Lockable : XRGrabInteractable{
        
        public bool locked {get; private set;}
        public bool selectableBySocketWhenLocked = true;
        public bool selectableByGrabWhenLocked = false;
        private bool rigidBodyEnabled;

        public override bool IsSelectableBy(XRBaseInteractor interactor){
            bool baseCase = base.IsSelectableBy(interactor);
            if(locked){
                if(interactor is XRDirectInteractor){
                    return (baseCase && selectableByGrabWhenLocked);
                } else if (interactor is XRSocketInteractor){
                    return (baseCase && selectableBySocketWhenLocked);
                }
                return false;
            } else {
                return (baseCase);
            }
        }

        public override bool IsHoverableBy(XRBaseInteractor interactor){
            bool baseCase = base.IsHoverableBy(interactor);
            if(locked){
                if(interactor is XRDirectInteractor){
                    return (baseCase && selectableByGrabWhenLocked);
                } else if (interactor is XRSocketInteractor){
                    return (baseCase && selectableBySocketWhenLocked);
                }
                return false;
            } else {
                return (baseCase);
            }
        }

        public void Lock(){
            locked = true;
            if(GetComponent<Rigidbody>() != null) {
                rigidBodyEnabled = GetComponent<Rigidbody>().isKinematic;
                GetComponent<Rigidbody>().isKinematic = true;
            }
        }

        public void Unlock(){
            locked = false;
            if(GetComponent<Rigidbody>() != null){
                GetComponent<Rigidbody>().isKinematic = rigidBodyEnabled;
            }
        }
    }
}
