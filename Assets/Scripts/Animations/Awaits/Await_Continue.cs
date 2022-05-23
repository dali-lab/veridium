using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Veridium_Interaction;

namespace Veridium_Animation{
    public class Await_Continue : AwaitUserBase
    {

        public GameObject continueButton;
        public GameObject resetButton;
        public AnimSequence lectureAnimSequence;
        public int resetIndex;

        public override void Play()
        {
            base.Play();

            //FindObjectOfType<SegmentPlay>().gameObject.SetActive(false);

            continueButton.SetActive(true);
            resetButton.SetActive(true);

            continueButton.GetComponentInChildren<SegmentPlay>().onInteractionStart.AddListener(OnInteractionStart);
            resetButton.GetComponentInChildren<SegmentPlay>().onInteractionStart.AddListener(OnInteractionStartReset);

            //FindObjectOfType<SegmentPlay>().onInteractionStart.AddListener(OnInteractionStart);


        }


        public void OnInteractionStart(){
            CompleteAction();
            continueButton.GetComponentInChildren<SegmentPlay>().onInteractionStart.RemoveListener(OnInteractionStart);
            resetButton.GetComponentInChildren<SegmentPlay>().onInteractionStart.RemoveListener(OnInteractionStartReset);

           //FindObjectOfType<SegmentPlay>().onInteractionStart.RemoveListener(OnInteractionStart);

           //FindObjectOfType<SegmentPlay>().gameObject.SetActive(false);

           continueButton.SetActive(false);
           resetButton.SetActive(false);

        }

        public void OnInteractionStartReset(){
            lectureAnimSequence.PlayAtSegment(resetIndex);
            CompleteAction();
            continueButton.GetComponentInChildren<SegmentPlay>().onInteractionStart.RemoveListener(OnInteractionStart);
            resetButton.GetComponentInChildren<SegmentPlay>().onInteractionStart.RemoveListener(OnInteractionStartReset);

           //FindObjectOfType<SegmentPlay>().onInteractionStart.RemoveListener(OnInteractionStart);

           //FindObjectOfType<SegmentPlay>().gameObject.SetActive(false);

           continueButton.SetActive(false);
           resetButton.SetActive(false);

        }

        protected override void UpdateAnim(){

            base.UpdateAnim();

            /*

            if (continueButton.GetComponentInChildren<SegmentPlay>().isComplete == true) CompleteAction();

            if (resetButton.GetComponentInChildren<SegmentPlay>().isComplete == true){
                lectureAnimSequence.PlayAtSegment(resetIndex);
                CompleteAction();
            }

            */

        }
    }
}
