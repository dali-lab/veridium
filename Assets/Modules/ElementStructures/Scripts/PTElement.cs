using System.Collections;
using UnityEngine;
using Veridium.Interaction;

namespace Veridium.Modules.ElementStructures
{    
    public class PTElement : MonoBehaviour
    {

        /// <summary>
        /// Holds information about the element tile and returns home
        /// When dropped.
        /// </summary>

        public string elementName;                              // Name of the element
        public GameObject home;                                 // GameObject for this one to snap back to when dropped
        public float maxUnHeldTime = 1f;                        // How long before this snaps back home
        public CellType type = CellType.CUBIC;                  // The greater cell structure of the element
        public CellVariation variation = CellVariation.SIMPLE;  // The variation on the cell structure of the element

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        // Called by the grab interactable
        public void Interacted()
        {
            StopAllCoroutines();
        }

        // Called by the grab interactable
        public void UnInteracted()
        {
            StartCoroutine(WaitThenGoHome());
        }

        IEnumerator WaitThenGoHome()
        {
            // Teleport home when not interacted for long enough
            yield return new WaitForSeconds(1f);
            gameObject.transform.position = home.transform.position;
            gameObject.transform.rotation = home.transform.rotation;
            if (GetComponent<Rigidbody>() != null)
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
            GetComponent<AudioSource>().Play();

        }


        public void Lock()
        {
            GetComponent<XRGrabInteractable_Lockable>().Lock();
        }

        public void Unlock()
        {
            GetComponent<XRGrabInteractable_Lockable>().Unlock();
        }


        [ContextMenu("Insert in element loader")]
        public void InsertInElementLoader()
        {
            ElementLoader loader = FindObjectOfType<ElementLoader>();
            if (loader != null)
            {
                loader.InsertElement(this);
            }
        }
    }
}
