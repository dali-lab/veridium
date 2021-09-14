using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeeingIsBelieving.Interaction
{
    public class Grabbable : MonoBehaviour
    {
        [Tooltip("How powerful the throw is when this object is released")]
        public float throwForce = 2;
        [Tooltip("How many frames to average this object's velocity over when throwing it")]
        [Min(1)]
        public int velocityAverageFrames = 10;

        [HideInInspector] public bool grabbed;

        // List of game objects with the Grabber script attached that are currently intersecting with this game object
        private List<GameObject> potentialGrabbers;
        // The game object currently grabbing this object
        private GameObject grabber;

        // This game object's parent before it was grabbed
        private Transform oldParent;
        // Whether this game object's rigidbody was kinematic before it was grabbed
        private bool oldKinematic;
        // The position of this object during the previous frame
        private Vector3 oldPos;
        // List of velocities over the past frames
        private List<Vector3> velocities;

        void Start()
        {
            // Instantiate a new list of potential grabbers
            potentialGrabbers = new List<GameObject>();
            velocities = new List<Vector3>();
        }

        // Update is called once per frame
        void Update()
        {
            // If there is no grabber, this object is not yet grabbed
            if (!grabber)
            {
                // Iterate through all potential grabbers
                foreach (GameObject potentialGrabber in potentialGrabbers)
                {
                    // Check if the potential grabber is grabbing, and that it isn't already grabbing something
                    Grabber grabber = potentialGrabber.GetComponent<Grabber>();
                    if (grabber.IsGrabbing() && !grabber.GrabbedObject)
                    {
                        // If it is grabbing, initiate a grab on this object
                        StartGrab(potentialGrabber);
                        break;
                    }
                }
            }
            else
            {
                // If there already is a grabber, check to see if it is no longer grabbing
                if (!grabber.GetComponent<Grabber>().IsGrabbing())
                {
                    // If that is the case, end the grab
                    EndGrab();
                }
                // Add this frame's velocity to the list
                velocities.Add(gameObject.transform.position - oldPos);
                // If the number of saved velocities is greater than the desired number, delete one
                if (velocities.Count > velocityAverageFrames - 1)
                {
                    velocities.RemoveAt(0);
                }
                // Update the old position
                oldPos = gameObject.transform.position;
            }
        }

        // Mark this gameobject as grabbed, and potentialGrabber as the grabber
        public void StartGrab(GameObject potentialGrabber)
        {
            // Track that this object is grabbed
            grabbed = true;

            // Save a reference to the object grabbing this object
            grabber = potentialGrabber;

            // Save a reference to the grabbed object in the grabber script
            grabber.GetComponent<Grabber>().GrabbedObject = gameObject;

            // Save the object's parent, then parent it to the grabber
            oldParent = gameObject.transform.parent;
            gameObject.transform.parent = grabber.transform;

            // Save whether the object is kinematic, then set kinematic to true (so it follows the movement of its parent)
            oldKinematic = gameObject.GetComponent<Rigidbody>().isKinematic;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }

        // End the grab happening on this object
        public void EndGrab()
        {
            grabbed = false;

            // Reset this objects values to their state prior to being grabbed
            gameObject.transform.parent = oldParent;
            gameObject.GetComponent<Rigidbody>().isKinematic = oldKinematic;
            // Mark that the grabber is no longer grabbing an item
            grabber.GetComponent<Grabber>().GrabbedObject = null;

            // Get the average velocity of the grabbed object's movement
            Vector3 avgVelocity = Vector3.zero;
            foreach (Vector3 velocity in velocities)
            {
                avgVelocity += velocity;
            }
            avgVelocity /= velocities.Count;
            // add force to this object based on the velocity
            gameObject.GetComponent<Rigidbody>().AddForce(avgVelocity * 1000 * throwForce);

            // Mark that there is no longer an object grabbing this one
            grabber = null;
        }

        // Called when an object with a trigger collider enters this object's collider (as long as this object has a non-trigger collider and rigidbody)
        private void OnTriggerEnter(Collider other)
        {
            // Check if the colliding object has a Grabber script attached
            if (other.GetComponent<Grabber>())
            {
                // If it does, add it to the list of potential grabbers
                potentialGrabbers.Add(other.gameObject);
            }
        }

        // Called when an object with a trigger collider exits this object's collider (as long as this object has a non-trigger collider and rigidbody)
        private void OnTriggerExit(Collider other)
        {
            // Remove the colliding object from the list of potential grabbers
            potentialGrabbers.Remove(other.gameObject);
        }
    }
}
