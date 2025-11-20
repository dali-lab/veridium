using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Veridium.Modules.AminoAcids
{
    public class SplitInteractableTest : MonoBehaviour
    {
        private static WaitForEndOfFrame waitDuration = new WaitForEndOfFrame();
        public Collider collider1, collider2;
        public XRGrabInteractable interactablePrefab;
        private XRGrabInteractable grabInteractable;

        // Start is called before the first frame update
        void Start()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
            grabInteractable.activated.AddListener(OnActivate);
        }

        private void OnActivate(ActivateEventArgs arg)
        {
            // Detach collider2 and make it into own grabInteractable
            XRGrabInteractable newInteractable = Instantiate(interactablePrefab, collider2.transform.position, collider2.transform.rotation);
            collider2.transform.parent = newInteractable.transform;
            StartCoroutine(Reregister(newInteractable));
        }

        private IEnumerator Reregister(XRGrabInteractable newInteractable)
        {
            XRInteractionManager manager = grabInteractable.interactionManager;
            manager.UnregisterInteractable(grabInteractable as IXRInteractable);
            manager.UnregisterInteractable(newInteractable as IXRInteractable);

            yield return waitDuration;

            grabInteractable.colliders.Remove(collider2);
            newInteractable.colliders.Add(collider2);

            yield return waitDuration;

            manager.RegisterInteractable(grabInteractable as IXRInteractable);
            manager.RegisterInteractable(newInteractable as IXRInteractable);
        }
    }
}