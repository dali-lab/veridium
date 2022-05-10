using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Veridium_Interaction;

namespace Veridium_Animation{
    public class Await_Continue : AwaitUserBase
    {

        public GameObject continueButton;

        public override void Play()
        {
            base.Play();

            //FindObjectOfType<SegmentPlay>().gameObject.SetActive(false);

            //FindObjectOfType<SegmentPlay>().onInteractionStart.AddListener(OnInteractionStart);

            continueButton.SetActive(true);
        }

        /*

        public void OnInteractionStart(){

           CompleteAction();

           //FindObjectOfType<SegmentPlay>().onInteractionStart.RemoveListener(OnInteractionStart);

           //FindObjectOfType<SegmentPlay>().gameObject.SetActive(false);
           
           continueButton.SetActive(false);

        }
        */

        protected override void UpdateAnim(){

            base.UpdateAnim();

            if (continueButton.GetComponentInChildren<SegmentPlay>().isComplete == true) CompleteAction();

        }
    }
}
