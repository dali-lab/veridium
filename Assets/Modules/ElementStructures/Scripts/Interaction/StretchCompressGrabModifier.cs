using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;
using Veridium.Interaction;
using Veridium.Modules.ElementStructures;

namespace Veridium.ElementStructures.Interaction
{
    public class StretchCompressGrabModifier : GrabModifier
    {
        public StructureBase structureBase;

        public Vector3 compressDirection = Vector3.up;



        public float maxStretch = 2.0f;
        public float minStretch = 0.5f;

        public float[] snapPoints;

        public float hardSnapThreshold = 0.03f;
        public float softSnapThreshold = 0.05f;

        public float lastStretchFactor = 1.0f;



        private bool isStretching = false;

        private float startDistance = 1.0f;

        private Vector3 initialLocalPosition1;
        private Vector3 initialLocalPosition2;

        private Vector3 interactorLocalOrthogonalDirection;
        private Vector3 structureLocalOrthogonalDirection;


        private IXRSelectInteractor interactor1;
        private IXRSelectInteractor interactor2;

        public override void OnGrabChanged(XRGrabInteractable grabInteractable, OverridableGrabTransformer transformer)
        {
            Debug.Log("Grab changed in StretchCompressGrabModifier");

            if (grabInteractable.interactorsSelecting.Count != 2) return;

            interactor1 = grabInteractable.interactorsSelecting[0];
            interactor2 = grabInteractable.interactorsSelecting[1];

            List<Transform> atoms = structureBase.structureBuilder.crystal.atoms.Where(a => a.Value.drawnObject != null).Select(a => a.Value.drawnObject.transform).ToList();

            if (atoms.Count < 2) return;

            Transform closestAtom1 = atoms.OrderBy(a => Vector3.Distance(a.position, interactor1.transform.position)).First();
            Transform closestAtom2 = atoms.OrderBy(a => Vector3.Distance(a.position, interactor2.transform.position)).First();

            Vector3 atomDirection = (closestAtom2.localPosition - closestAtom1.localPosition).normalized;

            Matrix4x4 initialStretchMatrix = Utils.getStretchTransformation(compressDirection, lastStretchFactor);

            initialLocalPosition1 = initialStretchMatrix.inverse * structureBase.structureController.transform.InverseTransformPoint(interactor1.transform.position);
            initialLocalPosition2 = initialStretchMatrix.inverse * structureBase.structureController.transform.InverseTransformPoint(interactor2.transform.position);

            structureLocalOrthogonalDirection = Vector3.Cross(atomDirection, Vector3.forward).normalized;
            interactorLocalOrthogonalDirection = interactor1.transform.InverseTransformDirection(structureBase.structureBuilder.transform.TransformDirection(structureLocalOrthogonalDirection)).normalized;


            isStretching = false;

            if (Mathf.Abs(Vector3.Dot(atomDirection, compressDirection)) < 0.99f) return;
            if (Vector3.Distance(interactor1.transform.position, closestAtom1.transform.position) > 0.15f) return;
            if (Vector3.Distance(interactor2.transform.position, closestAtom2.transform.position) > 0.15f) return;

            Debug.Log("Stretching in direction: " + compressDirection);

            isStretching = true;
            startDistance = Vector3.Distance(interactor1.transform.position, interactor2.transform.position) / lastStretchFactor;
            //transformer.allowTwoHandedScaling = false;
            //transformer.permittedDisplacementAxes = 0;

        }


        public override bool Process(XRGrabInteractable grabInteractable, OverridableGrabTransformer transformer, XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale)
        {
            if (grabInteractable.interactorsSelecting.Count != 2) return false;
            if (!isStretching) return false;

            float currentDistance = Vector3.Distance(grabInteractable.interactorsSelecting[0].transform.position, grabInteractable.interactorsSelecting[1].transform.position);

            float stretchFactor = currentDistance / startDistance;
            stretchFactor = Mathf.Clamp(stretchFactor, minStretch, maxStretch);
            stretchFactor = snapFactor(stretchFactor);

            Matrix4x4 deformation = Utils.getStretchTransformation(compressDirection, stretchFactor);

            structureBase.SetStructureDeformation(deformation);

            Vector3 scaledInitialLocalPosition1 = deformation.MultiplyPoint(initialLocalPosition1);
            Vector3 scaledInitialLocalPosition2 = deformation.MultiplyPoint(initialLocalPosition2);

            Vector3 interactorCurrDirection = interactor2.transform.position - interactor1.transform.position;
            Vector3 structureCurrDirection = scaledInitialLocalPosition2 - scaledInitialLocalPosition1;

            targetPose.rotation = Quaternion.FromToRotation(structureLocalOrthogonalDirection, interactor1.transform.TransformDirection(interactorLocalOrthogonalDirection));
            targetPose.rotation = Quaternion.FromToRotation(targetPose.rotation * structureCurrDirection, interactorCurrDirection) * targetPose.rotation;


            targetPose.position = targetPose.position + interactor1.transform.position - Matrix4x4.TRS(targetPose.position, targetPose.rotation, localScale).MultiplyPoint(scaledInitialLocalPosition1);

            lastStretchFactor = stretchFactor;

            return true;
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