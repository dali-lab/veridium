using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

namespace Veridium.Interaction
{
    /// <summary>
    /// OverridableGrabTransformer is a placeholder for a grab transformer that can be overridden.
    /// It is used to allow for custom grab behavior in the XR interaction toolkit.
    /// </summary>
    public class OverridableGrabTransformer : XRGeneralGrabTransformer
    {
        public GrabModifier[] grabModifiers; // Array of grab modifiers that can be applied to the grab transformer

        public override void OnGrabCountChanged(XRGrabInteractable grabInteractable, Pose targetPose, Vector3 localScale)
        {
            base.OnGrabCountChanged(grabInteractable, targetPose, localScale);

            // Notify all grab modifiers that a grab has occurred
            foreach (var modifier in grabModifiers)
            {
                modifier.OnGrabChanged(grabInteractable, this);
            }
        }

        public override void Process(XRGrabInteractable grabInteractable, XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale)
        {
            foreach (var modifier in grabModifiers)
            {
                bool processed = modifier.Process(grabInteractable, this, updatePhase, ref targetPose, ref localScale);

                if (processed) return;
            }

            base.Process(grabInteractable, updatePhase, ref targetPose, ref localScale);
        }
    }
}