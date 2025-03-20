using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Veridium.Modules.ElementStructures
{
    public class CoordinateSystemVisualization : MonoBehaviour
    {
        public GameObject arrowPrefab;

        public List<ArrowVisualization> drawnArrows;

        public float regularOpacity = 0.5f;
        public float highlightOpacity = 0.7f;

        public float regularThickness = 0.01f;
        public float highlightThickness = 0.02f;

        
        public Color blinkColor = Color.white;

        private Color aColor;
        private Color bColor;
        private Color cColor;

        public void SetFadePercent(float percent)
        {
            for (int i = 0; i < drawnArrows.Count; i++)
            {
                SetAxisFadePercent(i, percent);
            }
        }

        public void SetAxisFadePercent(int axisIndex, float percent)
        {
            ArrowVisualization axis = drawnArrows[axisIndex];
            axis.SetOpacity(Mathf.Lerp(0, regularOpacity, percent));
        }

        public void SetAxisHighlightPercent(int axisIndex, float percent)
        {
            ArrowVisualization axis = drawnArrows[axisIndex];

            axis.SetOpacity(Mathf.Lerp(regularOpacity, highlightOpacity, percent));
            axis.SetLineWidth(Mathf.Lerp(regularThickness, highlightThickness, percent));
        }

        public void SetAxisBlinkPercent(int axisIndex, float percent)
        {
            ArrowVisualization axis = drawnArrows[axisIndex];
            axis.SetColor(Color.Lerp(axis.color, blinkColor, percent));
        }

        public float GetAxisHighlightPercent(int axisIndex)
        {
            ArrowVisualization axis = drawnArrows[axisIndex];
            return (axis.arrowHead.material.color.a - regularOpacity) / (highlightOpacity - regularOpacity);
        }

        public ArrowVisualization DrawArrow(Vector3 from, Vector3 to, Color color, string label)
        {
            GameObject arrow = Instantiate(arrowPrefab, transform);
            ArrowVisualization arrowVis = arrow.GetComponent<ArrowVisualization>();

            arrowVis.SetFromTo(from, to);
            arrowVis.SetLabel(label);
            arrowVis.SetColor(color);
            arrowVis.SetOpacity(regularOpacity);
            arrowVis.SetLineWidth(regularThickness);

            arrowVis.color = color;
            
            drawnArrows.Add(arrowVis);
            return arrowVis;
        }

        public void ClearAxes()
        {
            foreach (ArrowVisualization arrow in drawnArrows)
            {
                Destroy(arrow.gameObject);
            }

            drawnArrows.Clear();
        }
    }
}
