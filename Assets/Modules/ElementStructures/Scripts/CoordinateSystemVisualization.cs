using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Veridium.Modules.ElementStructures
{
    [System.Serializable]
    public struct AxisVisualization
    {
        public LineRenderer line;
        public LineRenderer tip;
        public TextMeshPro label;
    }

    public class CoordinateSystemVisualization : MonoBehaviour
    {
        public AxisVisualization A;
        public AxisVisualization B;
        public AxisVisualization C;

        public float regularOpacity = 0.5f;
        public float highlightOpacity = 0.7f;

        public float regularThickness = 0.01f;
        public float highlightThickness = 0.02f;

        void Start() {
            setAxisThickness(A, regularThickness);
            setAxisThickness(B, regularThickness);
            setAxisThickness(C, regularThickness);
        }

        public void SetFadePercent(float percent)
        {
            setAxisOpacity(A, Mathf.Lerp(0, regularOpacity, percent));
            setAxisOpacity(B, Mathf.Lerp(0, regularOpacity, percent));
            setAxisOpacity(C, Mathf.Lerp(0, regularOpacity, percent));
        }

        public void SetAxisHighlightPercent(int axisIndex, float percent)
        {
            AxisVisualization axis = axisIndex == 0 ? A : axisIndex == 1 ? B : C;

            setAxisOpacity(axis, Mathf.Lerp(regularOpacity, highlightOpacity, percent));
            setAxisThickness(axis, Mathf.Lerp(regularThickness, highlightThickness, percent));
        }

        private void setAxisOpacity(AxisVisualization axis, float opacity)
        {
            Color color = axis.line.material.color;
            color.a = opacity;
            axis.line.material.color = color;

            color = axis.tip.material.color;
            color.a = opacity;
            axis.tip.material.color = color;

            color = axis.label.color;
            color.a = opacity;
            axis.label.color = color;
        }

        private void setAxisThickness(AxisVisualization axis, float thickness)
        {
            axis.line.startWidth = thickness;
            axis.line.endWidth = thickness;
            axis.tip.startWidth = thickness;
            axis.tip.endWidth = thickness;
        }

        public float GetAxisHighlightPercent(int axisIndex)
        {
            AxisVisualization axis = axisIndex == 0 ? A : axisIndex == 1 ? B : C;
            return (axis.line.material.color.a - regularOpacity) / (highlightOpacity - regularOpacity);
        }
    }
}
