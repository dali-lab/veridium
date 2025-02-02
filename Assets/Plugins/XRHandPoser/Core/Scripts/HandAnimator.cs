﻿// Copyright (c) MikeNspired. All Rights Reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MikeNspired.UnityXRHandPoser
{
    /// <summary>
    /// The main script that animates the hands. Located on the hand model, and as a child of the controller.
    /// Uses poses made from the rootbone of the skeleton. (Although any hierarchy of Transforms without skeletons can be animated with this class.)
    /// This class is setup/changed by the XRHandPoser/HandPoser when an object is grabbed.
    /// The animations are currently driven by 'triggerAnimationValue' called from a UnityEvent from 'XRControllerTriggerValueEvent' located on the hands
    /// </summary>
    public class HandAnimator : MonoBehaviour
    {
        [Tooltip("Draws spheres to see joints")] [SerializeField]
        private bool drawHelperSpheres = false;

        [Tooltip("Left or Right hand, used for determining which attachpoint needed")]
        public LeftRight handType;

        [Tooltip("Root bone of skeleton on hand")]
        public Pose RootBone;

        public Pose DefaultPose;

        [Tooltip("Animate to this pose  from Default Pose when pulling the trigger from values 0 to 1.")]
        public Pose AnimationPose;

        [Tooltip("Animation used when not holding an item and pulling the grip button")]
        public Pose SecondButtonPose;

        [Tooltip("Time hand skeleton animates to next pose")]
        public float animationTimeToNewPose = .1f;

        [Tooltip("Time to move hand to the item being grabbed")]
        public float handMoveToTargetAnimationTime = .1f;

        public float triggerAnimationValue;

        public bool isGrabbed;

        //The joints of the rootBone. These are the joints that are being animated.
        public List<Transform> currentJoints = new List<Transform>();

        //The joints of the pose being animated to. 
        private List<Transform> goalPoseJoints = new List<Transform>();

        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private Transform originalParent;

        private IEnumerator AnimateToPoseAnimation;
        private IEnumerator AnimateByTriggerValue;

        private Pose originalPose;
        private Pose originalAnimationPose;

        public UnityAction<bool> NewPoseStarting = delegate {  } ;

        //Restore original poses when an item is released
        public void ReleaseItemPoses() => BeginNewPoses(originalPose, originalAnimationPose);

        //Not used during gameplay. Used by editor script from button clicks in the inspector.
        public void AnimateToCurrent() => AnimateInstantly(DefaultPose);

        private void Awake()
        {
            originalPosition = transform.localPosition;
            originalRotation = transform.localRotation;
            originalParent = transform.parent;
            originalPose = DefaultPose;
            originalAnimationPose = AnimationPose;

            if (DefaultPose == null)
                DefaultPose = HandPoserSettings.Instance.DefaultPose;

            //Ensures the hand has the joints for posing setup.
            SetBones();

            //Start the default poses that are set in the inspector
            BeginNewPoses(DefaultPose, AnimationPose);
        }

        private void OnEnable() => Application.onBeforeRender += OnBeforeRender;

        private void OnDisable() => Application.onBeforeRender -= OnBeforeRender;


        //The main method used to control the animation value that animates to the AnimationPose.
        public void SetAnimationValue(float val)
        {
            triggerAnimationValue = val;
        }


        //Animates the the 'SecondButtonPose' typically the grip button when no item is being grabbed
        public void AnimateToSecondPose()
        {
            Debug.Log("called AnimateToSecondPose");
            if (isGrabbed) return;

            if (currentJoints.Count == 0)
            {
                Debug.Log("Setting bones");
                SetBones();
            }

            Debug.Log("after isGrabbed check");

            if (AnimateToPoseAnimation != null) StopCoroutine(AnimateToPoseAnimation);
            if (AnimateByTriggerValue != null) StopCoroutine(AnimateByTriggerValue);

            SetNewJoints(RootBone, goalPoseJoints);
            TransformStruct[] oldPoseTransforms = CopyTransformData(goalPoseJoints);

            SetNewJoints(SecondButtonPose, goalPoseJoints);
            TransformStruct[] secondPoseTransforms = CopyTransformData(goalPoseJoints);

            if (AnimateToPoseAnimation != null) StopCoroutine(AnimateToPoseAnimation);
            AnimateToPoseAnimation = AnimateToPoseOverTime(oldPoseTransforms, secondPoseTransforms);
            StartCoroutine(AnimateToPoseAnimation);
        }

        public void ReturnToDefaultPosing()
        {
            isGrabbed = false;
            BeginNewPoses(DefaultPose, AnimationPose);
        }


        public void SetBones() => SetNewJoints(RootBone, currentJoints);

        public void SetPoses(Pose primaryPose, Pose animationPose)
        {
            DefaultPose = primaryPose;
            AnimationPose = animationPose;
        }

        public void BeginNewPoses(Pose primaryPose, Pose animationPose)
        {
            
            NewPoseStarting.Invoke(isGrabbed);
            AnimationPose = animationPose;

            AnimateFromOldPoseToNewPose(primaryPose, animationPose);
            if (animationPose)
                StartAnimationByValue(animationPose);
            else if (AnimateByTriggerValue != null) StopCoroutine(AnimateByTriggerValue);
        }

        private void AnimateFromOldPoseToNewPose(Pose primaryPose, Pose animationPose)
        {
            //Set old pose to the original pose before starting this
            SetNewJoints(DefaultPose, goalPoseJoints);
            TransformStruct[] oldPose = CopyTransformData(goalPoseJoints);

            DefaultPose = primaryPose;
            TransformStruct[] newPose;

            //Get position joints should be if trigger is held for animation
            if (animationPose)
            {
                SetNewJoints(animationPose, goalPoseJoints);
                TransformStruct[] anim = CopyTransformData(goalPoseJoints);
                newPose = GetPosePositionInNewPose(oldPose, anim);
            }
            else //If no animation animate to normal pose
            {
                SetNewJoints(primaryPose, goalPoseJoints);
                newPose = CopyTransformData(goalPoseJoints);
            }

            //Set oldPose to the current position of the joints
            SetNewJoints(RootBone, goalPoseJoints);
            TransformStruct[] currentJointPositions = CopyTransformData(goalPoseJoints);

            //Start animation
            if (AnimateToPoseAnimation != null) StopCoroutine(AnimateToPoseAnimation);
            AnimateToPoseAnimation = AnimateToPoseOverTime(currentJointPositions, newPose);
            StartCoroutine(AnimateToPoseAnimation);
        }


        private void StartAnimationByValue(Pose animationPose)
        {
            //Setup new goal joints for animation
            AnimationPose = animationPose;
            SetNewJoints(animationPose, goalPoseJoints);

            //Start animation
            if (AnimateByTriggerValue != null) StopCoroutine(AnimateByTriggerValue);
            AnimateByTriggerValue = AnimateToPoseByValue(animationTimeToNewPose);
            StartCoroutine(AnimateByTriggerValue);
        }


        private bool animateToPoseComplete;

        private IEnumerator AnimateToPoseOverTime(TransformStruct[] originalPose, TransformStruct[] newPose)
        {
            animateToPoseComplete = false;
            float timer = 0;
            while (timer <= animationTimeToNewPose + Time.deltaTime)
            {
                for (int i = 0; i < currentJoints.Count; ++i)
                {
                    Transform joint = currentJoints[i];

                    if (joint.position == newPose[i].position && joint.rotation == newPose[i].rotation) continue;

                    var newPosition = Vector3.Lerp(originalPose[i].position, newPose[i].position, timer / animationTimeToNewPose);
                    var newRotation = Quaternion.Lerp(originalPose[i].rotation, newPose[i].rotation, timer / animationTimeToNewPose);

                    SetNewJoint(ref joint, newPosition, newRotation);
                }

                timer += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            animateToPoseComplete = true;
        }

        private IEnumerator AnimateToPoseByValue(float startDelay = 0)
        {
            while (!animateToPoseComplete) yield return new WaitForSeconds(Time.deltaTime);

            SetNewJoints(DefaultPose, goalPoseJoints);
            TransformStruct[] startingPose = CopyTransformData(goalPoseJoints);

            SetNewJoints(AnimationPose, goalPoseJoints);

            while (true)
            {
                for (int i = 0; i < currentJoints.Count; ++i)
                {
                    Transform joint = currentJoints[i];

                    if (joint.position == goalPoseJoints[i].position && joint.rotation == goalPoseJoints[i].rotation) continue;

                    var newPosition = Vector3.Lerp(startingPose[i].position, goalPoseJoints[i].localPosition, triggerAnimationValue);
                    var newRotation = Quaternion.Lerp(startingPose[i].rotation, goalPoseJoints[i].localRotation, triggerAnimationValue);
                    SetNewJoint(ref joint, newPosition, newRotation);
                }

                yield return new WaitForSeconds(Time.deltaTime);
            }
        }

        private TransformStruct[] GetPosePositionInNewPose(TransformStruct[] oldPose, TransformStruct[] anim)
        {
            //Get Current joint 
            TransformStruct[] startingPose = oldPose;
            TransformStruct[] endingPose = new TransformStruct[startingPose.Length];

            for (int i = 0; i < currentJoints.Count; ++i)
            {
                Transform joint = currentJoints[i];

                if (joint.position == goalPoseJoints[i].position && joint.rotation == goalPoseJoints[i].rotation) continue;

                endingPose[i].position = Vector3.Lerp(startingPose[i].position, anim[i].position, triggerAnimationValue);
                endingPose[i].rotation = Quaternion.Lerp(startingPose[i].rotation, anim[i].rotation, triggerAnimationValue);
            }

            return endingPose;
        }

        public void AnimateInstantly(Pose animation)
        {
            if (currentJoints.Count == 0 || currentJoints[0] == null)
                SetBones();

            if (AnimateToPoseAnimation != null) StopCoroutine(AnimateToPoseAnimation);
            if (AnimateByTriggerValue != null) StopCoroutine(AnimateByTriggerValue);
            SetNewJoints(animation, goalPoseJoints);
            AnimateInstant(goalPoseJoints);
        }

        private void AnimateInstant(List<Transform> goalPose)
        {
            for (int i = 0; i < currentJoints.Count; ++i)
            {
                Transform joint = currentJoints[i];
                if (joint.position == goalPose[i].position && joint.rotation == goalPose[i].rotation) continue;
                SetNewJoint(ref joint, goalPose[i].localPosition, goalPose[i].localRotation);
            }
        }

        #region MoveHandToObject

        private IEnumerator AnimateHandToPosition;
        private IEnumerator WaitForObjectToBeClose;

        public void MoveHandToTarget(Transform attachPoint, float interactableAttachEaseInTime, bool waitForHandToAnimateToPosition)
        {
            isGrabbed = true;
            StartCoroutine(MoveHandToTargetIE(attachPoint, interactableAttachEaseInTime, waitForHandToAnimateToPosition));
        }

        private IEnumerator MoveHandToTargetIE(Transform attachPoint, float interactableAttachEaseInTime, bool waitForHandToAnimateToPosition)
        {
            //Set hand parent to null to stop player movement from moving hand
            if (waitForHandToAnimateToPosition)
                yield return new WaitForSeconds(interactableAttachEaseInTime);


            transform.parent = null;

            if (AnimateHandToPosition != null) StopCoroutine(AnimateHandToPosition);
            AnimateHandToPosition = AnimateHandToTransform(handMoveToTargetAnimationTime, attachPoint);
            yield return StartCoroutine(AnimateHandToPosition);

            //Maintain hand position to target if target moves
            StartHandPositionTracking(attachPoint);
        }

        public void ReturnAnimationsToOriginal()
        {
            DefaultPose = originalPose;
            AnimationPose = originalAnimationPose;
        }

        public void ReturnHandToPlayer()
        {
            StopAllCoroutines();
            transform.parent = originalParent;

            StopHandPositionTracking();
            if (AnimateHandToPosition != null) StopCoroutine(AnimateHandToPosition);

            AnimateHandToPosition = AnimateHandTransformLocal(handMoveToTargetAnimationTime, new TransformStruct(originalPosition, originalRotation, Vector3.zero));
            StartCoroutine(AnimateHandToPosition);
        }

        private IEnumerator AnimateHandToTransform(float animationLength, Transform newTransform)
        {
            float timer = 0;
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;

            while (timer < animationLength + Time.deltaTime)
            {
                var newPosition = Vector3.Lerp(startPos, newTransform.position, timer / animationLength);
                var newRotation = Quaternion.Lerp(startRot, newTransform.rotation, timer / animationLength);

                transform.SetPositionAndRotation(newPosition, newRotation);

                yield return new WaitForSeconds(Time.deltaTime);
                timer += Time.deltaTime;
            }

            transform.SetPositionAndRotation(newTransform.position, newTransform.rotation);
        }

        public Rigidbody rb;

        private IEnumerator AnimateHandTransformLocal(float animationLength, TransformStruct newTransform)
        {
            float timer = 0;
            Vector3 startPos = transform.localPosition;
            Quaternion startRot = transform.localRotation;

            while (timer < animationLength + Time.deltaTime)
            {
                var newPosition = Vector3.Lerp(startPos, newTransform.position, timer / animationLength);
                var newRotation = Quaternion.Lerp(startRot, newTransform.rotation, timer / animationLength);

                transform.localPosition = newPosition;
                transform.localRotation = newRotation;

                yield return new WaitForSeconds(Time.deltaTime);
                timer += Time.deltaTime;
            }

            transform.localPosition = newTransform.position;
            transform.localRotation = newTransform.rotation;
        }

        private void StartHandPositionTracking(Transform target)
        {
            setPosition = true;
            this.handPositionTarget = target;
        }

        private void StopHandPositionTracking()
        {
            setPosition = false;
            this.handPositionTarget = null;
        }

        private Transform handPositionTarget;
        private bool setPosition;

        [BeforeRenderOrder(102)]

        private void OnBeforeRender()
        {
            if (setPosition)
            {
                transform.SetPositionAndRotation(handPositionTarget.position, handPositionTarget.rotation);
            }
        }

        #endregion

        private void SetNewJoint(ref Transform joint, Vector3 newPosition, Quaternion newRotation)
        {
            joint.localPosition = newPosition;
            joint.localEulerAngles = newRotation.eulerAngles;
        }

        private Vector3 InvertLocalPosition(Vector3 vec)
        {
            return new Vector3(vec.x, vec.y, -vec.z);
        }

        private Vector3 InvertLocalRotation(Quaternion rot)
        {
            Vector3 vec = rot.eulerAngles;
            return new Vector3(-vec.x, -vec.y, vec.z);
        }

        private TransformStruct[] CopyTransformData(List<Transform> joints)
        {
            TransformStruct[] transforms = new TransformStruct[joints.Count];
            for (int i = 0; i < joints.Count; ++i)
                transforms[i].SetTransformStruct(joints[i].localPosition, joints[i].localRotation, Vector3.one);
            return transforms;
        }

        private void SetNewJoints(Pose pose, List<Transform> jointList)
        {
            if (pose == null)
            {
                if (pose == null || jointList == null)
                {
                    Debug.LogError("Error: Please set a pose for hand to animate to :: Pose is Null");
                    return;
                }
            }

            jointList.Clear();
            SetJoints(pose.transform, jointList);
        }

        private void SetJoints(Transform curTransform, List<Transform> joints)
        {
            if (!curTransform.name.EndsWith("Ignore"))
                joints.Add(curTransform);
            for (int i = 0; i < curTransform.childCount; ++i)
            {
                Transform child = curTransform.GetChild(i);
                SetJoints(child, joints);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!RootBone) return;
            if (drawHelperSpheres)
                RootBone.DrawJoints(RootBone.transform);
            else
                RootBone.debugSpheresEnabled = false;
        }
    }
}