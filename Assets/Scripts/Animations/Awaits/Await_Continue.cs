using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Veridium.Interaction;

namespace Veridium.Animation{
    public class Await_Continue : AwaitUserBase
    {

        // public GameObject continueButton;
        // public GameObject resetButton;

        // public Transform buttonOneSpawnPoint;
        // public Transform buttonTwoSpawnPoint;

        public AnimSequence lectureAnimSequence;
        public int resetIndex;

        // private GameObject continueButtonInstance;
        // private GameObject resetButtonInstance;

        public override void Play()
        {
            base.Play();

            // spawn button 1
            // spawn button 2
            // continueButtonInstance = Instantiate(continueButton, buttonOneSpawnPoint.position, Quaternion.identity);
            //resetButtonInstance = Instantiate(resetButton, buttonTwoSpawnPoint.position, Quaternion.identity);

            //continueButton.SetActive(true);
            //resetButton.SetActive(true);

            // continueButtonInstance.GetComponentInChildren<SegmentPlay>().onInteractionStart.AddListener(OnInteractionStart);
            //resetButtonInstance.GetComponentInChildren<SegmentPlay>().onInteractionStart.AddListener(OnInteractionStartReset);

            VeridiumButton.Instance.Enable();
            VeridiumButton.Instance.onInteracted.AddListener(onInteracted);
        }


        public void onInteracted(){
            CompleteAction();
            //continueButton.GetComponentInChildren<SegmentPlay>().onInteractionStart.RemoveListener(OnInteractionStart);
            //resetButton.GetComponentInChildren<SegmentPlay>().onInteractionStart.RemoveListener(OnInteractionStartReset);


            //continueButton.SetActive(false);
            //resetButton.SetActive(false);
            // Destroy(continueButtonInstance);
            //Destroy(resetButtonInstance);

        }

        // public void OnInteractionStartReset(){
        //     CompleteAction();
        //     //continueButton.GetComponentInChildren<SegmentPlay>().onInteractionStart.RemoveListener(OnInteractionStart);
        //     //resetButton.GetComponentInChildren<SegmentPlay>().onInteractionStart.RemoveListener(OnInteractionStartReset);

        //     //continueButton.SetActive(false);
        //     //resetButton.SetActive(false);
        //     Destroy(continueButtonInstance);
        //     //Destroy(resetButtonInstance);

        //     lectureAnimSequence.PlayAtSegment(resetIndex);

        // }

        protected override void UpdateAnim(){

            base.UpdateAnim();

        }
    }
}
