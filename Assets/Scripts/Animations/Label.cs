using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIB_Animation{
    public class Label : MonoBehaviour
    {
        /// <summary>
        /// Controls a label that floats in the air above its center. Used with
        /// the Label prefab. Uses DMM units: https://www.ryanhinojosa.com/2018/01/08/device-independent/
        /// </summary>

        public string labelText = "Sample Text";                                        // Text rendered on the label
        public Vector2 offset = new Vector2(0.1f, 0.1f);                                // Offset in 2d space because the label always faces the camera
        public float boxWidthDMM = 300f, fontSizeDMM = 24f, paddingDMM = 16f;           // DMM units for label properties
        public GameObject textLabel, lineRenderer;                                      // GameObject references
        public GameObject controlPoint1, controlPoint2, controlPoint3, controlPoint4;   // Four control points of the line

        // Start is called before the first frame update
        void Start()
        {

            // Set up the text box
            textLabel.GetComponent<RectTransform>().sizeDelta = new Vector2(boxWidthDMM/1000,0.025f);
            textLabel.transform.localPosition = new Vector3(offset.x,offset.y,0);
            textLabel.GetComponent<TMPro.TextMeshPro>().text = labelText;
            textLabel.GetComponent<TMPro.TextMeshPro>().fontSize = fontSizeDMM/1000;

            // Find the horizontal width for the line to take up
            float curveWidth = offset.x - boxWidthDMM/2000 - paddingDMM/1000;

            // Set the control points for the line
            controlPoint1.transform.localPosition = Vector3.zero;
            controlPoint2.transform.localPosition = new Vector3(curveWidth*0.3f, 0, 0);
            controlPoint3.transform.localPosition = new Vector3(curveWidth*0.7f,offset.y,0);
            controlPoint4.transform.localPosition = new Vector3(curveWidth,offset.y,0);
        }

        // Update is called once per frame
        void Update()
        {
            var camera = FindObjectsOfType<Camera>()[0];

            // Point the label toward the camera
            transform.rotation = Quaternion.LookRotation(textLabel.transform.position - camera.transform.position, Vector3.up);

            // Scale the label so that DMM remains constant
            transform.localScale = Vector3.one * (textLabel.transform.position - camera.transform.position).magnitude;
        }

        // Will probably go unused but useful to see how it works
        float DMMtoWorldSpace(float DMM){

            return(DMM * (textLabel.transform.position - FindObjectsOfType<Camera>()[0].transform.position).magnitude / 1000);

        }
    }
}
