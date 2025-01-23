using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Veridium.Interaction{

    public class JoystickSwitch : MonoBehaviour
    {

        public InputActionProperty m_HandMoveAction;
        public float threshold = 0.8f;
        public float resetThreshold = 0.2f;
        public StructureBase structureBase;
        private bool triggered = false;

        public Vector2 ReadInput()
        {
            var HandValue = m_HandMoveAction.action?.ReadValue<Vector2>() ?? Vector2.zero;

            return HandValue;
        }

        void Update(){

            CheckSwitch(ReadInput().x);

        }

        private void CheckSwitch(float input){

            if(!triggered){

                if (Mathf.Abs(input) > threshold){

                    triggered = true;
                    structureBase.Switch(input > 0);

                }

            } else if (Mathf.Abs(input) < resetThreshold){

                triggered = false;

            }
        }
    }
}
