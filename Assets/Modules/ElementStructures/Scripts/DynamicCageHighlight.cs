using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Veridium.Modules.ElementStructures
{
    public class DynamicCageHighlight : MonoBehaviour
    {
        public StructureBase structureBase;

        public LineRenderer lr;

        public Material material;


        public Matrix4x4 highlightedPos;
        public float highlightedDistance = 0.2f;
        public float highlightedWidth = 0.1f;
        public Color highlightedColor = Color.yellow;
        
        public float notHighlightWidth = 0.05f;
        public Color notHighlightColor = new Color(0.5f, 0.5f, 0.5f, .4f);

        
        public Vector3[] unitCellHighlightPositions;


        // Start is called before the first frame update
        void Start()
        {
            structureBase = GetComponentInParent<StructureBase>();
            structureBase.onStructureDeformed.AddListener(UpdateCageHighlight);

            lr = gameObject.AddComponent<LineRenderer>();

            LineRenderer original = structureBase.structureBuilder.GetComponent<LineRenderer>();
            // System.Type type = original.GetType();
            // System.Reflection.FieldInfo[] fields = type.GetFields();
            // foreach (System.Reflection.FieldInfo field in fields) field.SetValue(lr, field.GetValue(original));
            lr.useWorldSpace = original.useWorldSpace;
            lr.material = new Material(material);
            lr.positionCount = unitCellHighlightPositions.Length;
            lr.startWidth = notHighlightWidth;
            lr.endWidth = notHighlightWidth;

            lr.SetPositions(unitCellHighlightPositions);

            UpdateCageHighlight(Matrix4x4.identity);
        }

        void OnDestroy()
        {
            structureBase.onStructureDeformed.RemoveListener(UpdateCageHighlight);
        }

        public void UpdateCageHighlight(Matrix4x4 deformationMatrix)
        {
            float highlight = Mathf.Max(0, 1 - Matrix4x4Distance(deformationMatrix, highlightedPos) / highlightedDistance);

            lr.material.color = Color.Lerp(notHighlightColor, highlightedColor, highlight);

            lr.startWidth = Mathf.Lerp(notHighlightWidth, highlightedWidth, highlight);
            lr.endWidth = Mathf.Lerp(notHighlightWidth, highlightedWidth, highlight);

            lr.SetPositions(unitCellHighlightPositions.Select(v => deformationMatrix.MultiplyPoint(v)).ToArray());
        }


        public float Matrix4x4Distance(Matrix4x4 a, Matrix4x4 b)
        {
            float distance = 0;
            for (int i = 0; i < 16; i++)
            {
                distance += (a[i] - b[i]) * (a[i] - b[i]);
            }
            return Mathf.Sqrt(distance);
        }
    }
}