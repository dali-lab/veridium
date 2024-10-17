using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Veridium_Interaction {

    [RequireComponent(typeof(AudioSource))]
    public class ReturnHomeAfter : MonoBehaviour
    {
        public Transform home;
        public float timeToReturn = 1.0f;

        public void Start() {
            if (!home) home = transform.parent;
        }

        // Called by the grab interactable
        public void Interacted()
        {
            StopAllCoroutines();
        }

        // Called by the grab interactable
        public void UnInteracted()
        {
            if (!gameObject.activeSelf) return;
            StartCoroutine(WaitThenGoHome());
        }

        IEnumerator WaitThenGoHome()
        {
            // Teleport home when not interacted for long enough
            yield return new WaitForSeconds(timeToReturn);
            transform.position = home.position;
            transform.rotation = home.rotation;
            if (GetComponent<Rigidbody>() != null)
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
            GetComponent<AudioSource>().Play();

        }
    }
}
