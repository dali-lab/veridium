using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
using Veridium_Interaction;

namespace Veridium_Animation{
    [System.Serializable]
    public class AwaitContinue : AwaitUserBase{

        public override void Play()
        {
            base.Play();

            MonoBehaviour.FindObjectOfType<SegmentPlay>().gameObject.SetActive(false);

            MonoBehaviour.FindObjectOfType<SegmentPlay>().onInteractionStart.AddListener(OnInteractionStart);
            
        }

        public void OnInteractionStart(){

           CompleteAction();

           MonoBehaviour.FindObjectOfType<SegmentPlay>().onInteractionStart.RemoveListener(OnInteractionStart);

           MonoBehaviour.FindObjectOfType<SegmentPlay>().gameObject.SetActive(false);

        }
    }
}