using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

namespace Veridium.Interaction
{
    
    public abstract class GrabModifier : MonoBehaviour
    {
        public abstract bool Process(XRGrabInteractable grabInteractable, OverridableGrabTransformer transformer, XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale);

        public virtual void OnGrabChanged(XRGrabInteractable grabInteractable, OverridableGrabTransformer transformer)
        {
            // Optional: Override to handle grab events
        }
    }
}