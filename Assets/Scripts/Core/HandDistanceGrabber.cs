using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIB_Interaction{
    public class HandDistanceGrabber : MonoBehaviour
    {

        public float startPointZOffset = -.5f, rayDistance = 3f;
        public bool distanceGrabActive = true;
        public GameObject headset;
        private HandDistanceGrabbable lastFrameHovered;
        private int layerMask = 1 << 7;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
            RaycastHit hit;
            Vector3 direction = (transform.position - (headset.transform.position + new Vector3(0,0,startPointZOffset))).normalized;
            Physics.SphereCast(transform.position, 0.5f, transform.forward, out hit, Mathf.Infinity, layerMask);

            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = hit.transform.gameObject.ToString();

            HandDistanceGrabbable hovered = null;
            if (hit.transform.gameObject.GetComponent<HandGrabCollider>() != null) hovered = hit.transform.gameObject.GetComponent<HandGrabCollider>().handDistanceGrabbable;

            //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = hovered.ToString();

            if (lastFrameHovered != null && hovered != lastFrameHovered){
                lastFrameHovered.UnHovered();
            }

            lastFrameHovered = null;

            if (hovered != null){
                hovered.Hovered(gameObject);
                lastFrameHovered = hovered;
            }
        }
    }
}
