using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace SIB_Interaction{

    public class JoystickSwitch : MonoBehaviour
    {

        public InputActionProperty m_LeftHandMoveAction;
        public InputActionProperty m_RightHandMoveAction;
        public float threshold = 0.8f;
        public float resetThreshold = 0.2f;
        public StructureBase structureBase;
        private bool triggered;

        public Vector2 ReadInput()
        {
            var leftHandValue = m_LeftHandMoveAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
            var rightHandValue = m_RightHandMoveAction.action?.ReadValue<Vector2>() ?? Vector2.zero;

            return leftHandValue + rightHandValue;
        }

        void Update(){

            CheckSwitch(ReadInput().x);

        }

        private void CheckSwitch(float input){

            if(!triggered){

                if (Mathf.Abs(input) > threshold){

                    structureBase.Switch(input > 0);
                    triggered = true;

                }

            } else if (Mathf.Abs(input) < resetThreshold){

                triggered = false;

            }
        }
    }
}
