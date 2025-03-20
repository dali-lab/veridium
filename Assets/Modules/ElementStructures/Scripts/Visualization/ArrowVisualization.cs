using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Veridium.Modules.ElementStructures
{
    public class ArrowVisualization : MonoBehaviour
    {
        public LineRenderer arrowHead;
        public LineRenderer arrowTail;
        public TextMeshPro label;

        public Color color;

        public void SetFromTo(Vector3 from, Vector3 to)
        {
            transform.localPosition = from;

            Vector3 direction = to - from;
            transform.localRotation = Quaternion.LookRotation(direction, Vector3.up);

            SetLength(direction.magnitude);
        }
        
        public void SetLength(float length)
        {
            arrowTail.SetPosition(1, new Vector3(0, 0, length - .07f));
            arrowHead.transform.localPosition = new Vector3(0, 0, length);

            label.transform.position = arrowHead.transform.position + new Vector3(0.1f, 0.1f, 0);
            label.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        public void SetColor(Color color)
        {
            // keep the alpha value of the color
            color.a = arrowHead.material.color.a;
            arrowHead.material.color = color;
            arrowTail.material.color = color;

            label.color = color;
        }

        public void SetOpacity(float opacity)
        {
            Color color = arrowHead.material.color;
            color.a = opacity;
            arrowHead.material.color = color;
            arrowTail.material.color = color;

            color = label.color;
            color.a = opacity;
            label.color = color;
        }

        public void SetLabel(string text)
        {
            label.text = text;
        }

        public void SetLineWidth(float width)
        {
            arrowHead.startWidth = width;
            arrowHead.endWidth = width;
            arrowTail.startWidth = width;
            arrowTail.endWidth = width;
        }
    }
}