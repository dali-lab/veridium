using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIB_Interaction{
    public class HandDistanceGrabber : MonoBehaviour
    {

        public float startPointZOffset = -.5f, rayDistance = 3f;
        public bool distanceGrabActive = true;
        public GameObject headset;
        private int layerMask;
        private bool grabbing;

        // Start is called before the first frame update
        void Start()
        {

            layerMask = 1 << LayerMask.NameToLayer("DistanceGrab");
            
        }

        // Update is called once per frame
        void Update()
        {
            if(distanceGrabActive && !grabbing){
            
                RaycastHit hit;
                Vector3 direction = (transform.position - (headset.transform.position + new Vector3(0,0,startPointZOffset))).normalized;
                Physics.SphereCast(transform.position, 0.05f, transform.forward, out hit, Mathf.Infinity, layerMask);

                HandDistanceGrabbable hovered = null;
                if (hit.collider.gameObject.GetComponent<HandDistanceGrabbable>() != null) hovered = hit.collider.gameObject.GetComponent<HandDistanceGrabbable>();

                if (hovered != null){
                    hovered.Hovered(gameObject);

                    GetComponent<SphereCollider>().center = transform.InverseTransformPoint(hovered.transform.position);
                } else {

                    GetComponent<SphereCollider>().center = Vector3.zero;
                    
                }
            }
        }

        public void grabBegun(){
            grabbing = true;
        }

        public void grabEnded(){
            grabbing = false;
        }
    }
}
