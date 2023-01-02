using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Veridium_Animation
{
    public class Await_GripButtonsPress : AwaitUserBase
    {

        [SerializeField] GripButtonWatcher gripButtonWatcher;

        void Awake()
        {

            // Add listeners to the proper events
            gripButtonWatcher.gripButtonPress.AddListener(BothButtonsPressed);

        }

        public override void Play()
        {
            base.Play();

            // Check if the user has already completed the action
        }

        void BothButtonsPressed()
        {

            CompleteAction();
        }


    }
}
