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
            lineWidth = 0.012f;
            isSelectable = false;
            // get DistanceGrab and RayPoint layermasks for raycast
            layerMaskDistanceGrab = 1 << LayerMask.NameToLayer("DistanceGrab");
            layerMaskRayPoint = 1 << LayerMask.NameToLayer("RayPoint");
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.SetWidth(lineWidth, lineWidth);
            lineRenderer.SetVertexCount(2);                                 // two vertices: start (controller) and end (RayPoint layer)
            grabbing = GetComponent<HandDistanceGrabber>().grabbed();       // check if the controller is grabbing any tiles
            interactor = GetComponent<XRDirectInteractor>();
        }

        // Update is called once per frame
        void Update()
        {
            var origin = transform.position;  
            bool drawline = false;                                          // set true if conditions to render line are met.
            RaycastHit hit;         
            RaycastHit hitDistanceGrab;
            RaycastHit hitRayPoint;
            grabbing = GetComponent<HandDistanceGrabber>().grabbed();       // check if controller is grabbing anything

            bool hittedDistanceGrab = Physics.SphereCast(origin, 0.05f, transform.forward, out hitDistanceGrab, Mathf.Infinity, layerMaskDistanceGrab);
            bool hittedRayPoint = Physics.SphereCast(origin, 0.05f, transform.forward, out hitRayPoint, Mathf.Infinity, layerMaskRayPoint);
            bool hitted = Physics.SphereCast(origin, 0.05f, transform.forward, out hit, Mathf.Infinity, layerMaskRayPoint | layerMaskDistanceGrab);

            // chaining to ensure that it is not null
            if (hitDistanceGrab.collider == null || hitDistanceGrab.collider.gameObject == null || hitDistanceGrab.collider.gameObject.GetComponent<HandDistanceGrabbable>() == null) {
                isSelectable = false;
            } else {
                isSelectable = hitDistanceGrab.collider.gameObject.GetComponent<HandDistanceGrabbable>().IsSelectableBy(interactor);
            }

            // conditions for rendering line
            if (grabbing) {         // if controller is grabbing something, do not render line
                drawline = false;
            } else if (hittedDistanceGrab && isSelectable) {    // if hit a distanceGrabbable object and it is selectable
                drawline = true;
                if(!hitDistanceGrab.collider.gameObject.GetComponent<HandDistanceGrabbable>().drawRay) drawline = false;
            } else if (hittedRayPoint) {    // if hit raypoint layer mask
                drawline = true;
                
            }

            if (drawline) {
                Vector3 endPoint = hit.point;
                origin = origin + transform.forward * 0.0317f;      // shift line rendering origin to front of controller
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, origin);                // start of line
                lineRenderer.SetPosition(1, endPoint);              // end of line
            } else {
                lineRenderer.enabled = false;
            }
        }
    }
}
