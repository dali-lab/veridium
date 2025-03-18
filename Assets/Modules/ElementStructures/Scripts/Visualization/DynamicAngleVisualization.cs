using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Veridium.Modules.ElementStructures {
    public class DynamicAngleVisualization : MonoBehaviour
    {
        public Atom rootAtom;
        public Atom incidentAtom1;
        public Atom incidentAtom2;

        public LineRenderer lineRenderer;
        public TextMesh angleText;

        public bool shouldShowText = true;

        // Start is called before the first frame update
        void Start()
        {
            lineRenderer.positionCount = 10;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            Vector3 b = incidentAtom1.drawnObject.transform.position - rootAtom.drawnObject.transform.position;
            Vector3 c = incidentAtom2.drawnObject.transform.position - rootAtom.drawnObject.transform.position;

            float angle = Vector3.Angle(b, c);

            angleText.text = angle.ToString("0.0") + "°";
            angleText.gameObject.SetActive(shouldShowText);

            transform.rotation = Quaternion.LookRotation(Vector3.Cross(-b, c).normalized, c);

            angleText.transform.rotation = Quaternion.LookRotation(
                Vector3.Dot(Camera.main.transform.forward, transform.forward) > 0 ? transform.forward : -transform.forward,
                Vector3.up);

            for (int i = 0; i < 10; i++)
            {
                float t = i / 9.0f;

                lineRenderer.SetPosition(i, new Vector3(
                    -Mathf.Sin(Mathf.Deg2Rad * angle * t),
                    Mathf.Cos(Mathf.Deg2Rad * angle * t),
                    0
                ));
            }
        }
    }
}