using System.Collections.Generic;
using System.Linq;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Veridium.Modules.AminoAcids
{
    public class PeekableStructuralFormula : MonoBehaviour
    {
        private XRSimpleInteractable interactable;
        private new UnityEngine.Animation animation;
        private AminoAcidSheet sheet;
        public Sprite peekMaterial;
        public Molfile molfile;

        void Start()
        {
            animation = GetComponent<UnityEngine.Animation>();
            interactable = GetComponent<XRSimpleInteractable>();
            sheet = GetComponentInParent<AminoAcidSheet>();
            if (!interactable) return;

            interactable.firstHoverEntered.AddListener(OnHoverEnter);
            interactable.lastHoverExited.AddListener(OnHoverExit);
            interactable.activated.AddListener(OnActivate);
        }

        private void OnHoverEnter(HoverEnterEventArgs args)
        {
            if (!(args.interactorObject is XRRayInteractor)) return;
            animation.Play("Grow");
        }

        private void OnHoverExit(HoverExitEventArgs args)
        {
            if (!(args.interactorObject is XRRayInteractor)) return;
            animation.Play("Shrink");
        }

        private void OnActivate(ActivateEventArgs args)
        {
            sheet.ToggleAminoAcid(this);
        }

        [ContextMenu("Activate")]
        public void Activate()
        {
            List<IXRInteractor> interactors = new List<IXRInteractor>();
            interactable.interactionManager.GetRegisteredInteractors(interactors);
            ActivateEventArgs args = new ActivateEventArgs
            {
                interactorObject = interactors.First() as IXRActivateInteractor,
                interactableObject = interactable
            };

            (interactable as IXRActivateInteractable).OnActivated(args);
        }
    }
}
