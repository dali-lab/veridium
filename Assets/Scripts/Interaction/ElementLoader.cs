using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Veridium_Animation;

namespace Veridium_Interaction{
    public class ElementLoader : XRSocketInteractor
    {

        /// <summary>
        /// Element Loader is attached to a game object with a collider and allows element tiles to snap
        /// into the socket and prompts the structureBase to build that structure.
        /// Extends XRSocketInteractor
        /// </summary>

        public PTElement heldElement;           // Current element in the slot
        public StructureBase structureBase;     // StructureBase that this loads elements for
        public Animator insertedAnimation;      // animator to enable when the element is inserted
        private int layerMask;

        public GameObject lectures;
        private GameObject currLecture;

        private Dictionary<string, GameObject> lectureNameToGO;

        protected override void Start() {
            base.Start();

            lectureNameToGO = new Dictionary<string, GameObject>();

            foreach (Transform child in lectures.transform)
            {
                string lectureName = child.name;
                lectureNameToGO.Add(lectureName, child.gameObject);
                child.gameObject.SetActive(false);
            }
        }


        // Overrides OnSelectEntering, used to detect when element tiles are added to the slot
        protected override void OnSelectEntering(XRBaseInteractable interactable){
            Debug.Log("PUT A THING INTO THE THING!!!");

            base.OnSelectEntering(interactable);

            heldElement = interactable.gameObject.GetComponent<PTElement>();

            if (heldElement == null) return;

            structureBase.ElementAdded(heldElement);

            if (lectureNameToGO.ContainsKey(heldElement.elementName))
            {
                currLecture = lectureNameToGO[heldElement.elementName];
                Debug.Log("setting lecture " + heldElement.elementName + ": " + currLecture + " to active");
                currLecture.SetActive(true);
                currLecture.GetComponent<AnimSequence>().PlaySequenceFromStart();
            }

            GetComponent<AudioSource>().Play(); // Plays the breaaaahahhaha sound
            if(insertedAnimation != null) insertedAnimation.SetBool("circuitActive", true);
        }

        // Overrides OnSelectExiting, used to detect when element tiles are removed from the slot
        protected override void OnSelectExiting(XRBaseInteractable interactable){

            base.OnSelectExiting(interactable);

            structureBase.ElementRemoved();

            heldElement = null;

            if (currLecture) 
            {
                //currLecture.GetComponent<AnimSequence>().ResetSequence();
                currLecture.SetActive(false);
                currLecture = null;
            }

            if(insertedAnimation != null) insertedAnimation.SetBool("circuitActive", false);
        }

        public void Lock(){
            if(heldElement != null) heldElement.Lock();
        }

        public void Unlock(){
            heldElement.Unlock();
        }

        public void ResetStructure(){

            structureBase.ElementRemoved();

            if(heldElement != null) structureBase.ElementAdded(heldElement);
        }

    }
}
