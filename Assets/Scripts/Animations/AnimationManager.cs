using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Veridium_Animation{
    public class AnimationManager : MonoBehaviour
    {

        public InputActionProperty m_ScrubAction;
        public float scrubRate = 5f;
        public AnimSequence currentSequence;
        private bool scrubbing;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            float input = ReadInput().x;

            if(Mathf.Abs(input) > 0.1){

                if(!scrubbing) {

                    currentSequence.PauseSequence();
                    scrubbing = true;

                }

                currentSequence.ScrubSequence(input * scrubRate * Time.deltaTime);

            } else if (scrubbing){

                currentSequence.PlaySequence();
                scrubbing = false;

            }
            
        }

        public Vector2 ReadInput()
        {
            var HandValue = m_ScrubAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
            return HandValue;
        }
    }
}