using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Veridium.Interaction;
using Veridium.Modules.ElementStructures;


namespace Veridium.Modules.ElementStructures {
    public class StructureCompressionHandle : MonoBehaviour
    {
        public Atom atomToTrack;
        public List<StructureCompressionHandle> others;

        public StructureBase structureBase;

        public Vector3 compressDirection = Vector3.up;

        public float maxStretch = 2.0f;
        public float minStretch = 0.5f;

        public float[] snapPoints;

        public float hardSnapThreshold = 0.03f;
        public float softSnapThreshold = 0.05f;

        private bool isSelected = false;

        // Start is called before the first frame update
        void Start()
        {
            others = new List<StructureCompressionHandle>(
                transform.parent.GetComponentsInChildren<StructureCompressionHandle>().Where(x => x != this)
            );
        }

        // Update is called once per frame
        void Update()
        {
            if (!atomToTrack.drawnObject) return;

            if (!isSelected) {
                transform.position = atomToTrack.drawnObject.transform.position;
            } else {
                float stretchFactor = getCurrentStretchFactor();
                stretchFactor = Mathf.Clamp(stretchFactor, minStretch, maxStretch);
                stretchFactor = snapFactor(stretchFactor);

                structureBase.SetStructureDeformation(Utils.getStretchTransformation(compressDirection, stretchFactor));
            }
        }

        [ContextMenu("Select")]
        public void Select() {
            foreach (StructureCompressionHandle handle in others) {
                handle.GetComponent<XRGrabInteractable_Lockable>().Lock();
            }

            isSelected = true;
        }

        [ContextMenu("Deselect")]
        public void Deselect() {
            foreach (StructureCompressionHandle handle in others) {
                handle.GetComponent<XRGrabInteractable_Lockable>().Unlock();
            }

            isSelected = false;
        }

        private float getCurrentStretchFactor() {
            Transform atomLocalSpace = atomToTrack.drawnObject.transform.parent;

            Vector3 posInAtomSpace = atomLocalSpace.InverseTransformPoint(transform.position);

            return Vector3.Dot(posInAtomSpace, compressDirection) / Vector3.Dot(atomToTrack.GetPosition(), compressDirection);
        }

        private float snapFactor(float factor) {
            float closestSnapPoint = snapPoints[0];

            foreach (float snapPoint in snapPoints) {
                if (Mathf.Abs(snapPoint - factor) < Mathf.Abs(closestSnapPoint - factor)) {
                    closestSnapPoint = snapPoint;
                }
            }

            float distToClosestSnapPoint = Mathf.Abs(closestSnapPoint - factor);

            if (distToClosestSnapPoint < hardSnapThreshold) {
                return closestSnapPoint;
            } else if (distToClosestSnapPoint < softSnapThreshold) {
                return Mathf.Lerp(closestSnapPoint, factor, (distToClosestSnapPoint - hardSnapThreshold) / (softSnapThreshold - hardSnapThreshold));
            } else {
                return factor;
            }
        }
    }
}
