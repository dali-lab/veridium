using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Veridium_Interaction{

    public class RayCaster : MonoBehaviour
    {
        /// <summary>
        /// RayCaster should be attached to the controllers. This script requires that there be a RayPoint layer mask.
        /// All game objects that want to have a line rendered from the controller to the object must have the RayPoint layer mask.
        /// </summary>

        private int layerMaskDistanceGrab;                  // Collision filter for ray cast on DistanceGrab layer
        private int layerMaskRayPoint;                      // Collision filter for ray cast on RayPoint layer
        private LineRenderer lineRenderer;
        private bool grabbing;                              // set true if controller is grabbing a tile
        private float lineWidth;                            // width for line renderer
        private bool isSelectable;                          // set true if game object is selectable
        private XRDirectInteractor interactor;

        // Start is called before the first frame update
        void Start()
        {

            lineWidth = 0.008f;
            isSelectable = false;
            // get DistanceGrab and RayPoint layermasks for raycast
            layerMaskDistanceGrab = 1 << LayerMask.NameToLayer("DistanceGrab");
            layerMaskRayPoint = 1 << LayerMask.NameToLayer("RayPoint");
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.SetWidth(lineWidth, lineWidth);
            lineRenderer.SetVertexCount(2);                                 // two vertices: start (controller) and end (RayPoint layer)

            // check if the controller is grabbing any tiles
            grabbing = GetComponent<HandDistanceGrabber>().grabbed();

            interactor = GetComponent<XRDirectInteractor>();
        }

        // Update is called once per frame
        void Update()
        {
            var origin = transform.position;            
            RaycastHit hit;
            grabbing = GetComponent<HandDistanceGrabber>().grabbed();

            bool hitted = Physics.Raycast(origin, transform.forward, out hit, Mathf.Infinity, layerMaskRayPoint | layerMaskDistanceGrab);
            
            // get the game object that the hit is colliding with
            // if it is not null, it is a grabbable
            // check if game object is selectable by XR interactor
            // hit.collider.gameObject.GetComponent<HandDistanceGrabbable>()
            // IsSelectableBy(XRBaseInteractor interactor)
            // Perform raycast on DistanceGrab and RayPoint layers
            if (hit.collider.gameObject.GetComponent<HandDistanceGrabbable>() != null) {    // if object has a hand distance grabbable component, check if it is selectable
                isSelectable = hit.collider.gameObject.GetComponent<HandDistanceGrabbable>().IsSelectableBy(interactor);
            }
            if (hitted && !grabbing && isSelectable) { // if raycast lands on the right layers and controller not grabbing any tile, render line
                Vector3 endPoint = hit.point;
                Debug.Log("after displacement: " + origin);
                origin = origin + transform.forward * 0.0317f;
                Debug.Log("after displacement: " + origin);
                lineRenderer.SetWidth(lineWidth, lineWidth);
                lineRenderer.SetPosition(0, origin);
                lineRenderer.SetPosition(1, endPoint);  
            } else {
                // if not casting on DistanceGrab or RayPoint layers, make line rendered invisible
                lineRenderer.SetWidth(0, 0);
            }
        }
    }
}
