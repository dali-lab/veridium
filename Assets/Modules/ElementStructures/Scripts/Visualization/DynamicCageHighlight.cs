using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Veridium.Modules.ElementStructures
{
    [System.Serializable]
    public struct CoordinatePair
    {
        public Vector3 start;
        public Vector3 end;
    }

    public class DynamicCageHighlight : MonoBehaviour
    {
        public StructureBase structureBase;

        public LineRenderer lr;

        public Material material;


        public Matrix4x4[] highlightedPositions;
        public float highlightedDistance = 0.2f;
        public float highlightedWidth = 0.1f;
        public Color[] highlightedColors;

        public float notHighlightWidth = 0.05f;
        public Color notHighlightColor = new Color(0.5f, 0.5f, 0.5f, .4f);


        public Vector3[] unitCellHighlightPositions;

        public CoordinatePair[] cageBonds;
        public List<Bond> additionalBonds = new List<Bond>();


        // Start is called before the first frame update
        void Start()
        {
            structureBase = GetComponentInParent<StructureBase>();
            structureBase.onStructureDeformed.AddListener(UpdateCageHighlight);

            /*lr = gameObject.AddComponent<LineRenderer>();

            LineRenderer original = structureBase.structureBuilder.GetComponent<LineRenderer>();
            // System.Type type = original.GetType();
            // System.Reflection.FieldInfo[] fields = type.GetFields();
            // foreach (System.Reflection.FieldInfo field in fields) field.SetValue(lr, field.GetValue(original));
            lr.useWorldSpace = original.useWorldSpace;
            lr.material = new Material(material);
            lr.positionCount = unitCellHighlightPositions.Length;
            lr.startWidth = notHighlightWidth;
            lr.endWidth = notHighlightWidth;

            lr.SetPositions(unitCellHighlightPositions);*/

            foreach (Atom atom in structureBase.structureBuilder.crystal.atoms.Values)
            {
                Vector3 position = atom.GetPosition();
                
                
                if (atom.drawnObject != null)
                {
                    Debug.Log($"atom at position {position}");
                }
            }

            foreach (CoordinatePair bond in cageBonds)
            {
                Atom startAtom = structureBase.structureBuilder.crystal.GetAtomAtPosition(bond.start);
                Atom endAtom = structureBase.structureBuilder.crystal.GetAtomAtPosition(bond.end);

                Debug.Log($"Creating bond from {startAtom?.GetPosition()} to {endAtom?.GetPosition()}");

                if (startAtom != null && endAtom != null)
                {
                    Bond newBond = new Bond(startAtom, endAtom);
                    newBond.builder = structureBase.structureBuilder.gameObject;
                    newBond.Draw();

                    additionalBonds.Add(newBond);
                }
            }

            UpdateCageHighlight(Matrix4x4.identity);
        }

        void OnDestroy()
        {
            structureBase.onStructureDeformed.RemoveListener(UpdateCageHighlight);
        }

        public void UpdateCageHighlight(Matrix4x4 deformationMatrix)
        {

            float[] distances = highlightedPositions.Select(p => Matrix4x4Distance(deformationMatrix, p)).ToArray();

            int minIndex = distances.ToList().IndexOf(distances.Min());

            float highlight = Mathf.Max(0, 1 - distances[minIndex] / highlightedDistance);

            Color color = Color.Lerp(notHighlightColor, highlightedColors[minIndex], highlight);
            float width = Mathf.Lerp(notHighlightWidth, highlightedWidth, highlight);


            foreach (Bond bond in additionalBonds)
            {
                bond.UpdateDrawnPosition(deformationMatrix);
                bond.cylinderChild.GetComponent<Renderer>().material.color = color;
                bond.cylinderChild.transform.localScale = new Vector3(width, 0.26f, width);
            }
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