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

        public string labelText = "Sample Text";                                            // Text rendered on the label
        public Vector3 offset = new Vector3(0f, -0.1f, 0.3f);                               // Offset in 2d space because the label always faces the camera
        public float boxWidthDMM = 300f, fontSizeDMM = 24f, paddingDMM = 16f;                // DMM units for label properties
        public GameObject textLabel, lineRenderer, lineConnector, plane;                    // GameObject references
        public GameObject controlPoint1, controlPoint2, controlPoint3, controlPoint4;       // Four control points of the line

        // Start is called before the first frame update
        void Start()
        {

            // Set up the text box
            textLabel.GetComponent<RectTransform>().sizeDelta = new Vector2(boxWidthDMM/1000,0.025f);
            textLabel.GetComponent<TMPro.TextMeshPro>().text = labelText;
            textLabel.GetComponent<TMPro.TextMeshPro>().fontSize = fontSizeDMM/100;
            plane.transform.localScale = new Vector3(boxWidthDMM/10000 + paddingDMM/10000, 1f, 0.025f + paddingDMM/10000);

            // Find the horizontal width for the line to take up
            float curveWidth = offset.x - boxWidthDMM/2000 - paddingDMM/1000;

            lineConnector.transform.localPosition = new Vector3(-1 * (boxWidthDMM/2000 + paddingDMM/1000), 0, 0);

            Vector3 right = -1*Vector3.Cross(Vector3.up, (GetComponent<Camera>().transform.position - textLabel.transform.position)).normalized;

            textLabel.transform.position = transform.position + Quaternion.LookRotation(right, Vector3.up) * offset;

            // Set the control points for the line
            controlPoint1.transform.localPosition = Vector3.zero;
            controlPoint2.transform.position = transform.position + right * curveWidth * 0.3f;
            controlPoint3.transform.position = curveWidth * -0.3f * right + lineConnector.transform.position;
            controlPoint4.transform.position = lineConnector.transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            var camera = FindObjectsOfType<Camera>()[0];

            // Point the label toward the camera
            textLabel.transform.rotation = Quaternion.LookRotation(textLabel.transform.position - camera.transform.position, Vector3.up);

            // Scale the label so that DMM remains constant
            textLabel.transform.localScale = Vector3.one * (textLabel.transform.position - camera.transform.position).magnitude;

            // Find the horizontal width for the line to take up
            float curveWidth = Vector3.Scale((lineConnector.transform.position - transform.position), new Vector3(1, 0, 1)).magnitude;

            Vector3 right = -1*Vector3.Cross(Vector3.up, (camera.transform.position - textLabel.transform.position)).normalized;
            
            textLabel.transform.position = transform.position + Quaternion.LookRotation(right, Vector3.up) * offset;

            // Set the control points for the line
            controlPoint1.transform.localPosition = Vector3.zero;
            controlPoint2.transform.position = transform.position + right * curveWidth * 0.3f;
            controlPoint3.transform.position = curveWidth * -0.3f * right + lineConnector.transform.position;
            controlPoint4.transform.position = lineConnector.transform.position;
        }

        // Will probably go unused but useful to see how it works
        float DMMtoWorldSpace(float DMM){

            return(DMM * (textLabel.transform.position - FindObjectsOfType<Camera>()[0].transform.position).magnitude / 1000);

        }
    }
}
