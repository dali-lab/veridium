using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StructureController : MonoBehaviour
{

    /// <summary>
    /// Structure Controller handles the input from the grab handles 
    /// in the structure to solve for the appropriate motion of the
    /// structure. This handles one hand grab and two hand grab, the
    /// latter for scaling. 
    /// </summary>

    private bool grab1selected = false, grab2selected = false;              // Keeps track of the grabbed states of the handles
    public GameObject grab1, grab2, structure, hand1, hand2;                // GameObject References
    public GameObject scaleObject;                                          // An empty game object to parent the structure to. Makes math easier.
    private bool twoHandGrab, oneHandGrab;                                  // Whether in two hand scaling mode, one hand rotation/translation mode
    private float twoHandDistance;                                          // Initial distance between hands in scaling mode

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // Choose between two hand scaling and one hand translation/rotation
        if(grab1selected && grab2selected) {

            // Setup 2 hand grab if this is the first frame of two hand grab
            if (!twoHandGrab) Start2HandGrab();

            // Update the scale object's transform based on where the hands have moved
            scaleObject.transform.position = (hand1.transform.position + hand2.transform.position)/2;

            scaleObject.transform.localScale = Vector3.one * ((((grab1.transform.position - grab2.transform.position).magnitude/twoHandDistance-1)/2)+1);

            scaleObject.transform.rotation = Quaternion.LookRotation(grab1.transform.position - grab2.transform.position, Vector3.up);
            
        } else if (grab1selected || grab2selected) {

            GameObject grabber = grab1selected ? grab1 : grab2;
            GameObject hand = grab1selected ? hand1 : hand2;

            // Setup 1 hand grab if this is the first frame of one hand grab
            if(!oneHandGrab) Start1HandGrab(grabber, hand);

            // Stop 2 hand grab if transitioning to 1 hand grab
            if(twoHandGrab) twoHandGrab = false;

            // Update the scaleObject's position and rotation to match the grabber
            scaleObject.transform.position = grabber.transform.position;
            scaleObject.transform.rotation = grabber.transform.rotation;

        } else if (oneHandGrab){

            // Transition to not grabbing
            gameObject.transform.parent = structure.transform;
            oneHandGrab = false;
        }

        // Lock the grabbers to the structure when not grabbing it
        if (!grab1selected) grab1.transform.position = gameObject.transform.position;
        if (!grab2selected) grab2.transform.position = gameObject.transform.position;
        
    }


    // Logic for the first frame of 1 hand grab
    private void Start1HandGrab(GameObject grabber, GameObject hand){
        
        // Store the world scale of the structure
        Vector3 scale = gameObject.transform.lossyScale;

        // Reparent the structure to the base so we can mess around with the scaleObject
        gameObject.transform.parent = structure.transform;

        // Reset the scaleObject to the grabber
        scaleObject.transform.localScale = Vector3.one;
        scaleObject.transform.rotation = grabber.transform.rotation;
        scaleObject.transform.position = hand.transform.position;

        // Reparent the structure to the scaleObject and correct its scale
        gameObject.transform.parent = scaleObject.transform;
        gameObject.transform.localScale = scale;
        oneHandGrab = true;

    }

    // Logic for the first frame of 2 hand grab
    private void Start2HandGrab(){

        // Reparent the structure to the base so we can mess around with the scaleObject
        gameObject.transform.parent = structure.transform;

        // Set the scale object's initial position and scale 
        scaleObject.transform.position = (hand1.transform.position + hand2.transform.position)/2;
        twoHandDistance = (hand1.gameObject.transform.position - hand2.gameObject.transform.position).magnitude;
        scaleObject.transform.localScale = Vector3.one;
        scaleObject.transform.rotation = Quaternion.LookRotation(grab1.transform.position - grab2.transform.position, Vector3.up);

        // Parent the structure to the scale object
        gameObject.transform.parent = scaleObject.transform;
        twoHandGrab = true;
        oneHandGrab = false;
    }

    // Called by XR grab interactable in the structure
    public void Grab1Selected() {
        grab1selected = true;
    }

    // Called by XR grab interactable in the structure
    public void Grab1Deselected() {
        grab1selected = false;
    }

    // Called by XR grab interactable in the structure
    public void Grab2Selected() {
        grab2selected = true;
    }

    // Called by XR grab interactable in the structure
    public void Grab2Deselected() {
        grab2selected = false;
    }
}
