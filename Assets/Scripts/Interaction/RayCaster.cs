using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        private float lineWidth = (float) 0.008;             // width for line renderer

        // Start is called before the first frame update
        void Start()
        {

            // get DistanceGrab and RayPoint layermasks for raycast
            layerMaskDistanceGrab = 1 << LayerMask.NameToLayer("DistanceGrab");
            layerMaskRayPoint = 1 << LayerMask.NameToLayer("RayPoint");
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.SetWidth(lineWidth, lineWidth);
            lineRenderer.SetVertexCount(2);                                 // two vertices: start (controller) and end (RayPoint layer)

            // check if the controller is grabbing any tiles
            grabbing = GetComponent<HandDistanceGrabber>().grabbed();
        }

        // Update is called once per frame
        void Update()
        {
            var origin = transform.position;            
            RaycastHit hit;
            grabbing = GetComponent<HandDistanceGrabber>().grabbed();

            // Perform raycast on DistanceGrab and RayPoint layers
            bool hitted = Physics.Raycast(origin, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMaskRayPoint | layerMaskDistanceGrab);
            
            if (hitted && !grabbing) { // if raycast lands on the right layers and controller not grabbing any tile, render line
                var endPoint = hit.point;
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
