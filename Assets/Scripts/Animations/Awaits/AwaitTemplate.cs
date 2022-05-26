using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Veridium_Animation{
    [System.Serializable]
    public class AwaitTemplate : AwaitUserBase{

        public override void Play()
        {
            base.Play();

            // Add listeners to the proper events

            // Check if the user has already completed the action
            
        }
    }
}