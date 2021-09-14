using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SeeingIsBelieving.Core
{
    /// <summary>
    /// The InputManager script allows other scripts to subscribe to events that are executed when buttons on the
    /// Oculus Touch Controllers are pressed, released, and touched.
    /// Also allows scripts to subscribe to events when joystick is moved.
    /// Basically a wrapper for OVRInput.
    ///
    /// </summary>

    public sealed class InputManager : MonoBehaviour
    {
        public enum Direction
        {
            UP,
            DOWN,
            LEFT,
            RIGHT
        }

        public enum InputState
        {
            ON_PRESS,
            ON_RELEASE,
            ON_TOUCH_START,
            ON_TOUCH_END

        }

        public enum Button
        {
            A,
            B,
            X,
            Y,
            RIGHT_TRIGGER,
            LEFT_TRIGGER,
            RIGHT_GRIP,
            LEFT_GRIP,
            RIGHT_JOYSTICK,
            LEFT_JOYSTICK
        }

        public enum Trigger
        {
            RIGHT,
            LEFT
        }

        public enum Grip
        {
            RIGHT,
            LEFT
        }

        public enum Joystick
        {
            RIGHT,
            LEFT
        }

        private static InputManager _inputManager;
        public static InputManager Instance
        {
            get
            {
                if (!_inputManager)
                {
                    _inputManager = FindObjectOfType(typeof(InputManager)) as InputManager;
                    if (!_inputManager)
                    {
                        UnityEngine.Debug.LogError("There needs to be one active InputManger script on a GameObject in your scene.");
                    }
                }

                return _inputManager;
            }
        }

        /* Note: this script uses virtual mappings. Generally, the mappings follow as below:
         * 
         * OVRInput.Button.One: Button A
         * OVRInput.Button.Two: Button B
         * OVRInput.Button.Three: Button X
         * OVRInput.Button.Four: Button Y
         * OVRInput.Button.PrimaryThumbstick: Left Joystick
         * OVRInput.Button.SecondaryThumbstick: Right Joystick
         * OVRInput.Axis1D.PrimaryHandTrigger: Left Grip
         * OVRInput.Axis1D.SecondaryHandTrigger: Right Grip
         * OVRInput.Axis1D.PrimaryIndexTrigger: Left Trigger
         * OVRInput.Axis1D.SecondaryIndexTrigger: Right Trigger
         * 
         * But this may change if your controllers are somehow re-mapped.
         */

        public UnityAction<bool> onHasController = null; //Called when controllers' connection state changes

        // Camera Rig
        public GameObject cameraRig;

        //------------------------Right Controller Events--------------------------------------------------------//
        private event Action OnButtonAPress = null; //Button A Pressed
        private event Action OnButtonARelease = null; //Button A Released
        private event Action OnButtonATouchStart = null; //Button A touch has been detected
        private event Action OnButtonATouchEnd = null; //Button A touch has ended

        private event Action OnButtonBPress = null; //Button B Pressed
        private event Action OnButtonBRelease = null; //Button B Released
        private event Action OnButtonBTouchStart = null; //Button B touch has been detected
        private event Action OnButtonBTouchEnd = null; //Button B touch has ended

        private event Action OnRightJoystickPress = null; //Right Joystick Pressed
        private event Action OnRightJoystickRelease = null; //Right Joystick Released
        private event Action OnRightJoystickTouchStart = null; //Right joystick touch has been detected
        private event Action OnRightJoystickTouchEnd = null; //Right joystick touch has ended

        private event Action<Direction> OnRightJoystickMovement = null; //Right Joystick moved more than halfway in a direction
        private event Action<Vector2> OnRightJoystickMonitor = null; //See how far the joystick has been moved; (Vector2 X/Y range of -1.0f to 1.0f)

        private event Action OnRightGripPress = null; //Right Grip Button Pressed
        private event Action OnRightGripRelease = null; //Right Grip Button Released
        private event Action OnRightGripTouchStart = null; //Right grip touch state has changed
        private event Action OnRightGripTouchEnd = null; //Right grip touch state has changed
        private event Action<float> OnRightGripMonitor = null; //Monitor force on right grip button

        private event Action OnRightTriggerPress = null; //Right Trigger Button Pressed
        private event Action OnRightTriggerRelease = null; //Right Trigger Button Released
        private event Action OnRightTriggerTouchStart = null; //Right trigger touch has been detected
        private event Action OnRightTriggerTouchEnd = null; //Right trigger touch has ended
        private event Action<float> OnRightTriggerMonitor = null; //Monitor force on right trigger button


        //------------------------Left Controller Events--------------------------------------------------------//
        private event Action OnButtonXPress = null; //Button X Pressed
        private event Action OnButtonXRelease = null; //Button X Released
        private event Action OnButtonXTouchStart = null; //Button X touch has been detected
        private event Action OnButtonXTouchEnd = null; //Button X touch has ended

        private event Action OnButtonYPress = null; //Button Y Pressed
        private event Action OnButtonYRelease = null; //Button Y Released
        private event Action OnButtonYTouchStart = null; //Button Y touch has been detected
        private event Action OnButtonYTouchEnd = null; //Button Y touch has ended

        private event Action OnLeftJoystickPress = null; //Left Joystick Pressed
        private event Action OnLeftJoystickRelease = null; //Left Joystick Released
        private event Action OnLeftJoystickTouchStart = null; //Left joystick touch has been detected
        private event Action OnLeftJoystickTouchEnd = null; //Left joystick touch has ended

        private event Action<Direction> OnLeftJoystickMovement = null; //Left Joystick moved more than halfway in a direction       
        private event Action<Vector2> OnLeftJoystickMonitor = null; //See how far the joystick has been moved; (Vector2 X/Y range of -1.0f to 1.0f)

        private event Action OnLeftGripPress = null; //Left grip button Pressed
        private event Action OnLeftGripRelease = null; //Left grip button Released
        private event Action OnLeftGripTouchStart = null; //Left grip touch state has changed
        private event Action OnLeftGripTouchEnd = null; //Left grip touch state has changed
        private event Action<float> OnLeftGripMonitor = null; //Monitor force on left grip button

        private event Action OnLeftTriggerPress = null; //Left Trigger Button Pressed
        private event Action OnLeftTriggerRelease = null; //Left Trigger Button Released
        private event Action OnLeftTriggerTouchStart = null; //Left trigger touch has been detected
        private event Action OnLeftTriggerTouchEnd = null; //Left trigger touch has ended
        private event Action<float> OnLeftTriggerMonitor = null; //Monitor force on left trigger button

        //------------------------Touch Booleans--------------------------------------------------------//
        public bool ButtonATouched { get; private set; } //whether button A is currently touched or not
        public bool ButtonBTouched { get; private set; } //whether button B is currently touched or not
        public bool ButtonXTouched { get; private set; } //whether button X is currently touched or not
        public bool ButtonYTouched { get; private set; } //whether button Y is currently touched or not
        public bool LeftJoystickTouched { get; private set; } //whether left joystick is touched or not
        public bool RightJoystickTouched { get; private set; } //whether right joystick is touched or not
        public bool LeftTriggerTouched { get; private set; } //whether left trigger is touched or not
        public bool RightTriggerTouched { get; private set; } //whether right trigger is touched or not
        public bool LeftGripTouched { get; private set; } //whether left grip is touched or not
        public bool RightGripTouched { get; private set; } //whether right grip is touched or not

        //------------------------Press Booleans--------------------------------------------------------//
        public bool ButtonAPressed { get; private set; } //whether button A is currently pressed or not
        public bool ButtonBPressed { get; private set; } //whether button B is currently pressed or not
        public bool ButtonXPressed { get; private set; } //whether button X is currently pressed or not
        public bool ButtonYPressed { get; private set; } //whether button Y is currently pressed or not
        public bool LeftJoystickPressed { get; private set; } //whether left joystick is pressed or not
        public bool RightJoystickPressed { get; private set; } //whether right joystick is pressed or not
        public bool LeftTriggerPressed { get; private set; } //whether left trigger is pressed or not
        public bool RightTriggerPressed { get; private set; } //whether right trigger is pressed or not
        public bool LeftGripPressed { get; private set; } //whether left grip is pressed or not
        public bool RightGripPressed { get; private set; } //whether right grip is pressed or not

        [Header("Left Hand Force")]

        [Tooltip("The minimum pressure needed to execute the OnLeftGripPress callback. A float between 0.0 and 1.0")]
        [Range(0.0f, 1.0f)]
        [SerializeField] private float LeftGripPress = 0.55f; //determines the force needed on the left grip button to execute a call to the OnLeftGripPress function 

        [Tooltip("The maximum pressure needed to execute the OnLeftGripRelease callback. A float between 0.0 and 1.0")]
        [Range(0.0f, 1.0f)]
        [SerializeField] private float LeftGripRelease = 0.35f; //determines the force needed on the left grip button to execute a call to the OnLeftGripRelease function

        [Tooltip("The minimum pressure needed to execute the OnLeftTriggerPress callback. A float between 0.0 and 1.0")]
        [Range(0.0f, 1.0f)]
        [SerializeField] private float LeftTriggerPress = 0.55f; //determines the force needed on the left trigger button to execute a call to the OnLeftTriggerPress function

        [Tooltip("The maximum pressure needed to execute the OnLeftTriggerRelease callback. A float between 0.0 and 1.0")]
        [Range(0.0f, 1.0f)]
        [SerializeField] private float LeftTriggerRelease = 0.35f; //determines the force needed on the left trigger button to execute a call to the OnLeftTriggerRelease function

        [Header("Right Hand Force")]

        [Tooltip("The minimum pressure needed to execute the OnRightGripPress callback. A float between 0.0 and 1.0")]
        [Range(0.0f, 1.0f)]
        [SerializeField] private float RightGripPress = 0.55f; //determines the force needed on the right grip button to execute a call to the OnRightGripPress function

        [Tooltip("The maximum pressure needed to execute the OnRightGripRelease callback. A float between 0.0 and 1.0")]
        [Range(0.0f, 1.0f)]
        [SerializeField] private float RightGripRelease = 0.35f; //determines the force needed on the right grip button to execute a call to the OnRightGripRelease function

        [Tooltip("The minimum pressure needed to execute the OnRightTriggerPress callback. A float between 0.0 and 1.0")]
        [Range(0.0f, 1.0f)]
        [SerializeField] private float RightTriggerPress = 0.55f; //determines the force needed on the right trigger button to execute a call to the OnRightTriggerPress function

        [Tooltip("The maximum pressure needed to execute the OnRightTriggerRelease callback. A float between 0.0 and 1.0")]
        [Range(0.0f, 1.0f)]
        [SerializeField] private float RightTriggerRelease = 0.35f; //determines the force needed on the right trigger button to execute a call to the OnRightTriggerPress function


        private bool _hasControllers = false;
        private bool _inputActive = true;
        private float _gripTouchRegisterMinForce = 0.0000000001f; // Grip buttons are not touch capacitive, therefore the grip touch callbacks will be called
                                                                  // if the user presses the grip buttons with a force >= _gripTouchRegisterMinForce
        void Start()
        {
            // If no camera rig was set, try to find one in the scene
            if (!cameraRig)
            {
                cameraRig = GameObject.Find("OVRCameraRig");
            }
        }

        void Update()
        {
            if (!_inputActive) return;
            _hasControllers = CheckForControllers(_hasControllers);

            CheckButtonsPress();
            CheckButtonsRelease();
            CheckButtonsTouch();
            CheckJoysticksPress();
            CheckJoysticksRelease();
            CheckJoysticksTouch();
            CheckJoysticksMovement();
            CheckGripsPress();
            CheckGripsRelease();
            CheckTriggersPress();
            CheckTriggersRelease();

        }

        private void OnDestroy()
        {
            OVRManager.HMDMounted -= PlayerFound;
            OVRManager.HMDUnmounted -= PlayerLost;
        }

        //----------------------------PUBLIC FUNCTIONS TO ATTACH CALLBACKS----------------------------------------//

        /// <summary>
        /// Attach a callback that will be called whenever the button has entered the specified state
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <param name="button"></param>
        public void AttachInputHandler(Action callback, InputState state, Button button)
        {
            switch (state)
            {
                case InputState.ON_PRESS:
                    AttachPressCallback(callback, button);
                    break;
                case InputState.ON_RELEASE:
                    AttachReleaseCallback(callback, button);
                    break;
                case InputState.ON_TOUCH_START:
                    AttachTouchStartCallback(callback, button);
                    break;
                case InputState.ON_TOUCH_END:
                    AttachTouchEndCallback(callback, button);
                    break;
                default:
                    UnityEngine.Debug.LogError("Error: Cannot attach input event handler. Unable to recognize specified input state: " + state);
                    break;

            }
        }

        /// <summary>
        /// Attach a callback to monitor the force applied to the specified trigger
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="button"></param>
        public void AttachInputHandler(Action<float> callback, Trigger trigger)
        {
            AttachTriggerMonitorCallback(callback, trigger);
        }

        /// <summary>
        /// Attach a callback to monitor the force applied to the specified grip
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="button"></param>
        public void AttachInputHandler(Action<float> callback, Grip grip)
        {
            AttachGripMonitorCallback(callback, grip);
        }

        /// <summary>
        /// Attach a callback to monitor the movement of the specified joystick
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="joystick"></param>
        public void AttachInputHandler(Action<Vector2> callback, Joystick joystick)
        {
            AttachJoystickMonitorCallback(callback, joystick);
        }

        /// <summary>
        /// Attach a callback that will be called whenever the joystick moves more than halfway in any direction
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="joystick"></param>
        public void AttachInputHandler(Action<Direction> callback, Joystick joystick)
        {
            AttachJoystickMovementCallback(callback, joystick);
        }

        //----------------------------PUBLIC FUNCTIONS TO DETACH CALLBACKS----------------------------------------------------//

        /// <summary>
        /// Detach callback
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="button"></param>
        public void DetachInputHandler(Action callback, InputState state, Button button)
        {
            switch (state)
            {
                case InputState.ON_PRESS:
                    DetachPressCallback(callback, button);
                    break;
                case InputState.ON_RELEASE:
                    DetachReleaseCallback(callback, button);
                    break;
                case InputState.ON_TOUCH_START:
                    DetachTouchStartCallback(callback, button);
                    break;
                case InputState.ON_TOUCH_END:
                    DetachTouchEndCallback(callback, button);
                    break;
                default:
                    UnityEngine.Debug.LogError("Error: Cannot detach input event handler. Unable to recognize specified input state: " + state);
                    break;

            }
        }

        /// <summary>
        /// Detach callback
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="trigger"></param>
        public void DetachInputHandler(Action<float> callback, Trigger trigger)
        {
            DetachTriggerMonitorCallback(callback, trigger);
        }

        /// <summary>
        /// Detach callback
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="grip"></param>
        public void DetachInputHandler(Action<float> callback, Grip grip)
        {
            DetachGripMonitorCallback(callback, grip);
        }

        /// <summary>
        /// Detach callback
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="joystick"></param>
        public void DetachInputHandler(Action<Vector2> callback, Joystick joystick)
        {
            DetachJoystickMonitorCallback(callback, joystick);
        }

        /// <summary>
        /// Detach callback
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="joystick"></param>
        public void DetachInputHandler(Action<Direction> callback, Joystick joystick)
        {
            DetachJoystickMovementCallback(callback, joystick);
        }


        //------------------------PUBLIC FUNCTIONS TO CHECK THE STATE OF BUTTONS, TRIGGERS, AND GRIPS-------------------------------//

        /// <summary>
        /// Returns whether the button is currently pressed or not. The script calling this
        /// must call this function in its Update function, whether directly or by passing the function call up.
        /// </summary>
        /// <param name="button"></param>
        /// <returns>A boolean on whether the button is being pressed or not</returns>
        public bool IsButtonPressed(Button button)
        {
            switch (button)
            {
                case Button.A:
                    return OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.Touch);
                case Button.B:
                    return OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.Touch);
                case Button.X:
                    return OVRInput.Get(OVRInput.Button.Three, OVRInput.Controller.Touch);
                case Button.Y:
                    return OVRInput.Get(OVRInput.Button.Four, OVRInput.Controller.Touch);
                case Button.RIGHT_TRIGGER:
                    return OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger, OVRInput.Controller.Touch); //returns true if pressed more than halfway
                case Button.LEFT_TRIGGER:
                    return OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.Touch); //returns true if pressed more than halfway
                case Button.RIGHT_GRIP:
                    return OVRInput.Get(OVRInput.Button.SecondaryHandTrigger, OVRInput.Controller.Touch); //returns true if pressed more than halfway
                case Button.LEFT_GRIP:
                    return OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.Touch); //returns true if pressed more than halfway
                case Button.RIGHT_JOYSTICK:
                    return OVRInput.Get(OVRInput.Button.SecondaryThumbstick, OVRInput.Controller.Touch);
                case Button.LEFT_JOYSTICK:
                    return OVRInput.Get(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.Touch);
                default:
                    UnityEngine.Debug.LogError("Error: Could not recognize button type when checking for press");
                    return false;

            }
        }

        /// <summary>
        /// Returns whether the button is currently touched or not. The script calling this
        /// must call this function in its Update function, whether directly or by passing the function call up.
        /// Grip buttons are not touch capacitive, so the user must be pressing the grip buttons with a force greater than or equal to
        /// _gripTouchRegisterMinForce to return true.
        /// </summary>
        /// <param name="button"></param>
        /// <returns>A boolean on whether the button is being touched or not</returns>

        public bool IsButtonTouched(Button button)
        {
            switch (button)
            {
                case Button.A:
                    return OVRInput.Get(OVRInput.Touch.One, OVRInput.Controller.Touch);
                case Button.B:
                    return OVRInput.Get(OVRInput.Touch.Two, OVRInput.Controller.Touch);
                case Button.X:
                    return OVRInput.Get(OVRInput.Touch.Three, OVRInput.Controller.Touch);
                case Button.Y:
                    return OVRInput.Get(OVRInput.Touch.Four, OVRInput.Controller.Touch);
                case Button.RIGHT_TRIGGER:
                    return OVRInput.Get(OVRInput.Touch.SecondaryIndexTrigger, OVRInput.Controller.Touch);
                case Button.LEFT_TRIGGER:
                    return OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, OVRInput.Controller.Touch);
                case Button.RIGHT_GRIP:
                    return OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) >= _gripTouchRegisterMinForce;
                case Button.LEFT_GRIP:
                    return OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch) >= _gripTouchRegisterMinForce;
                case Button.RIGHT_JOYSTICK:
                    return OVRInput.Get(OVRInput.Touch.SecondaryThumbstick, OVRInput.Controller.Touch);
                case Button.LEFT_JOYSTICK:
                    return OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, OVRInput.Controller.Touch);
                default:
                    UnityEngine.Debug.LogError("Error: Could not recognize button type when checking for touch");
                    return false;

            }
        }

        //----INTERNAL FUNCTIONS TO CHECK THE STATE OF BUTTONS, TRIGGERS, GRIPS, AND JOYSTICKS OF CONTROLLERS AND CALL CORRESPONDING CALLBACKS----//

        //attach callback to specific button's on-press UnityAction
        private void AttachPressCallback(Action callback, Button button)
        {
            switch (button)
            {
                case Button.A:
                    OnButtonAPress += callback;
                    break;
                case Button.B:
                    OnButtonBPress += callback;
                    break;
                case Button.X:
                    OnButtonXPress += callback;
                    break;
                case Button.Y:
                    OnButtonYPress += callback;
                    break;
                case Button.LEFT_GRIP:
                    OnLeftGripPress += callback;
                    break;
                case Button.RIGHT_GRIP:
                    OnRightGripPress += callback;
                    break;
                case Button.LEFT_TRIGGER:
                    OnLeftTriggerPress += callback;
                    break;
                case Button.RIGHT_TRIGGER:
                    OnRightTriggerPress += callback;
                    break;
                case Button.LEFT_JOYSTICK:
                    OnLeftJoystickPress += callback;
                    break;
                case Button.RIGHT_JOYSTICK:
                    OnRightJoystickPress += callback;
                    break;
                default:
                    UnityEngine.Debug.LogError("Error: Unable to attach press callback to specified button: " + button);
                    break;
            }
        }

        //detach callback from specific button's on-press UnityAction
        private void DetachPressCallback(Action callback, Button button)
        {
            switch (button)
            {
                case Button.A:
                    OnButtonAPress -= callback;
                    break;
                case Button.B:
                    OnButtonBPress -= callback;
                    break;
                case Button.X:
                    OnButtonXPress -= callback;
                    break;
                case Button.Y:
                    OnButtonYPress -= callback;
                    break;
                case Button.LEFT_GRIP:
                    OnLeftGripPress -= callback;
                    break;
                case Button.RIGHT_GRIP:
                    OnRightGripPress -= callback;
                    break;
                case Button.LEFT_TRIGGER:
                    OnLeftTriggerPress -= callback;
                    break;
                case Button.RIGHT_TRIGGER:
                    OnRightTriggerPress -= callback;
                    break;
                case Button.LEFT_JOYSTICK:
                    OnLeftJoystickPress -= callback;
                    break;
                case Button.RIGHT_JOYSTICK:
                    OnRightJoystickPress -= callback;
                    break;
                default:
                    UnityEngine.Debug.LogError("Error: Unable to detach press callback to specified button: " + button);
                    break;
            }
        }

        //attach callback to specific button's on-release UnityAction
        private void AttachReleaseCallback(Action callback, Button button)
        {
            switch (button)
            {
                case Button.A:
                    OnButtonARelease += callback;
                    break;
                case Button.B:
                    OnButtonBRelease += callback;
                    break;
                case Button.X:
                    OnButtonXRelease += callback;
                    break;
                case Button.Y:
                    OnButtonYRelease += callback;
                    break;
                case Button.LEFT_GRIP:
                    OnLeftGripRelease += callback;
                    break;
                case Button.RIGHT_GRIP:
                    OnRightGripRelease += callback;
                    break;
                case Button.LEFT_TRIGGER:
                    OnLeftTriggerRelease += callback;
                    break;
                case Button.RIGHT_TRIGGER:
                    OnRightTriggerRelease += callback;
                    break;
                case Button.LEFT_JOYSTICK:
                    OnLeftJoystickRelease += callback;
                    break;
                case Button.RIGHT_JOYSTICK:
                    OnRightJoystickRelease += callback;
                    break;
                default:
                    UnityEngine.Debug.LogError("Error: Unable to attach release callback to specified button: " + button);
                    break;
            }
        }

        //detach callback from specific button's on-release UnityAction
        private void DetachReleaseCallback(Action callback, Button button)
        {
            switch (button)
            {
                case Button.A:
                    OnButtonARelease -= callback;
                    break;
                case Button.B:
                    OnButtonBRelease -= callback;
                    break;
                case Button.X:
                    OnButtonXRelease -= callback;
                    break;
                case Button.Y:
                    OnButtonYRelease -= callback;
                    break;
                case Button.LEFT_GRIP:
                    OnLeftGripRelease -= callback;
                    break;
                case Button.RIGHT_GRIP:
                    OnRightGripRelease -= callback;
                    break;
                case Button.LEFT_TRIGGER:
                    OnLeftTriggerRelease -= callback;
                    break;
                case Button.RIGHT_TRIGGER:
                    OnRightTriggerRelease -= callback;
                    break;
                case Button.LEFT_JOYSTICK:
                    OnLeftJoystickRelease -= callback;
                    break;
                case Button.RIGHT_JOYSTICK:
                    OnRightJoystickRelease -= callback;
                    break;
                default:
                    UnityEngine.Debug.LogError("Error: Unable to attach release callback to specified button: " + button);
                    break;
            }
        }

        //attach callback to specific button's on touch start UnityAction
        private void AttachTouchStartCallback(Action callback, Button button)
        {
            switch (button)
            {
                case Button.A:
                    OnButtonATouchStart += callback;
                    break;
                case Button.B:
                    OnButtonBTouchStart += callback;
                    break;
                case Button.X:
                    OnButtonXTouchStart += callback;
                    break;
                case Button.Y:
                    OnButtonYTouchStart += callback;
                    break;
                case Button.LEFT_GRIP:
                    OnLeftGripTouchStart += callback;
                    break;
                case Button.RIGHT_GRIP:
                    OnRightGripTouchStart += callback;
                    break;
                case Button.LEFT_TRIGGER:
                    OnLeftTriggerTouchStart += callback;
                    break;
                case Button.RIGHT_TRIGGER:
                    OnRightTriggerTouchStart += callback;
                    break;
                case Button.LEFT_JOYSTICK:
                    OnLeftJoystickTouchStart += callback;
                    break;
                case Button.RIGHT_JOYSTICK:
                    OnRightJoystickTouchStart += callback;
                    break;
                default:
                    UnityEngine.Debug.LogError("Error: Unable to attach touch start callback to specified button: " + button);
                    break;
            }
        }

        //detach callback from specific button's on touch start UnityAction
        private void DetachTouchStartCallback(Action callback, Button button)
        {
            switch (button)
            {
                case Button.A:
                    OnButtonATouchStart -= callback;
                    break;
                case Button.B:
                    OnButtonBTouchStart -= callback;
                    break;
                case Button.X:
                    OnButtonXTouchStart -= callback;
                    break;
                case Button.Y:
                    OnButtonYTouchStart -= callback;
                    break;
                case Button.LEFT_GRIP:
                    OnLeftGripTouchStart -= callback;
                    break;
                case Button.RIGHT_GRIP:
                    OnRightGripTouchStart -= callback;
                    break;
                case Button.LEFT_TRIGGER:
                    OnLeftTriggerTouchStart -= callback;
                    break;
                case Button.RIGHT_TRIGGER:
                    OnRightTriggerTouchStart -= callback;
                    break;
                case Button.LEFT_JOYSTICK:
                    OnLeftJoystickTouchStart -= callback;
                    break;
                case Button.RIGHT_JOYSTICK:
                    OnRightJoystickTouchStart -= callback;
                    break;
                default:
                    UnityEngine.Debug.LogError("Error: Unable to detach touch start callback to specified button: " + button);
                    break;
            }
        }


        //attach callback to specific button's on touch end UnityAction
        private void AttachTouchEndCallback(Action callback, Button button)
        {
            switch (button)
            {
                case Button.A:
                    OnButtonATouchEnd += callback;
                    break;
                case Button.B:
                    OnButtonBTouchEnd += callback;
                    break;
                case Button.X:
                    OnButtonXTouchEnd += callback;
                    break;
                case Button.Y:
                    OnButtonYTouchEnd += callback;
                    break;
                case Button.LEFT_GRIP:
                    OnLeftGripTouchEnd += callback;
                    break;
                case Button.RIGHT_GRIP:
                    OnRightGripTouchEnd += callback;
                    break;
                case Button.LEFT_TRIGGER:
                    OnLeftTriggerTouchEnd += callback;
                    break;
                case Button.RIGHT_TRIGGER:
                    OnRightTriggerTouchEnd += callback;
                    break;
                case Button.LEFT_JOYSTICK:
                    OnLeftJoystickTouchEnd += callback;
                    break;
                case Button.RIGHT_JOYSTICK:
                    OnRightJoystickTouchEnd += callback;
                    break;
                default:
                    UnityEngine.Debug.LogError("Error: Unable to attach touch end callback to specified button: " + button);
                    break;
            }
        }

        //detach callback to specific button's on touch end UnityAction
        private void DetachTouchEndCallback(Action callback, Button button)
        {
            switch (button)
            {
                case Button.A:
                    OnButtonATouchEnd -= callback;
                    break;
                case Button.B:
                    OnButtonBTouchEnd -= callback;
                    break;
                case Button.X:
                    OnButtonXTouchEnd -= callback;
                    break;
                case Button.Y:
                    OnButtonYTouchEnd -= callback;
                    break;
                case Button.LEFT_GRIP:
                    OnLeftGripTouchEnd -= callback;
                    break;
                case Button.RIGHT_GRIP:
                    OnRightGripTouchEnd -= callback;
                    break;
                case Button.LEFT_TRIGGER:
                    OnLeftTriggerTouchEnd -= callback;
                    break;
                case Button.RIGHT_TRIGGER:
                    OnRightTriggerTouchEnd -= callback;
                    break;
                case Button.LEFT_JOYSTICK:
                    OnLeftJoystickTouchEnd -= callback;
                    break;
                case Button.RIGHT_JOYSTICK:
                    OnRightJoystickTouchEnd -= callback;
                    break;
                default:
                    UnityEngine.Debug.LogError("Error: Unable to detach touch end callback to specified button: " + button);
                    break;
            }
        }

        //attach callback to trigger button's on monitor UnityAction
        private void AttachTriggerMonitorCallback(Action<float> callback, Trigger trigger)
        {
            switch (trigger)
            {
                case Trigger.RIGHT:
                    OnRightTriggerMonitor += callback;
                    break;
                case Trigger.LEFT:
                    OnLeftTriggerMonitor += callback;
                    break;
                default:
                    UnityEngine.Debug.LogError("Error: Unable to attach monitor callback to specified trigger: " + trigger);
                    break;
            }
        }

        //detach callback from trigger button's on monitor UnityAction
        private void DetachTriggerMonitorCallback(Action<float> callback, Trigger trigger)
        {
            switch (trigger)
            {
                case Trigger.RIGHT:
                    OnRightTriggerMonitor -= callback;
                    break;
                case Trigger.LEFT:
                    OnLeftTriggerMonitor -= callback;
                    break;
                default:
                    UnityEngine.Debug.LogError("Error: Unable to attach monitor callback to specified trigger: " + trigger);
                    break;
            }
        }

        //attach callback to grip button's on monitor UnityAction
        private void AttachGripMonitorCallback(Action<float> callback, Grip grip)
        {
            switch (grip)
            {
                case Grip.RIGHT:
                    OnRightGripMonitor += callback;
                    break;
                case Grip.LEFT:
                    OnLeftGripMonitor += callback;
                    break;
                default:
                    UnityEngine.Debug.LogError("Error: Unable to attach monitor callback to specified grip: " + grip);
                    break;
            }
        }

        //detach callback from grip button's on monitor UnityAction
        private void DetachGripMonitorCallback(Action<float> callback, Grip grip)
        {
            switch (grip)
            {
                case Grip.RIGHT:
                    OnRightGripMonitor -= callback;
                    break;
                case Grip.LEFT:
                    OnLeftGripMonitor -= callback;
                    break;
                default:
                    UnityEngine.Debug.LogError("Error: Unable to attach monitor callback to specified grip: " + grip);
                    break;
            }
        }

        //attach callback to joystick's on monitor UnityAction
        private void AttachJoystickMonitorCallback(Action<Vector2> callback, Joystick joystick)
        {
            switch (joystick)
            {
                case Joystick.RIGHT:
                    OnRightJoystickMonitor += callback;
                    break;
                case Joystick.LEFT:
                    OnLeftJoystickMonitor += callback;
                    break;
                default:
                    UnityEngine.Debug.LogError("Error: Unable to attach monitor callback to specified josytick: " + joystick);
                    break;
            }
        }

        //detach callback from joystick's on monitor UnityAction
        private void DetachJoystickMonitorCallback(Action<Vector2> callback, Joystick joystick)
        {
            switch (joystick)
            {
                case Joystick.RIGHT:
                    OnRightJoystickMonitor -= callback;
                    break;
                case Joystick.LEFT:
                    OnLeftJoystickMonitor -= callback;
                    break;
                default:
                    UnityEngine.Debug.LogError("Error: Unable to attach monitor callback to specified trigger: " + joystick);
                    break;
            }
        }

        //attach callback to joystick's on movement UnityAction
        private void AttachJoystickMovementCallback(Action<Direction> callback, Joystick joystick)
        {
            switch (joystick)
            {
                case Joystick.RIGHT:
                    OnRightJoystickMovement += callback;
                    break;
                case Joystick.LEFT:
                    OnLeftJoystickMovement += callback;
                    break;
                default:
                    UnityEngine.Debug.LogError("Error: Unable to attach monitor callback to specified josytick: " + joystick);
                    break;
            }
        }

        //detach callback from joystick's on movement UnityAction
        private void DetachJoystickMovementCallback(Action<Direction> callback, Joystick joystick)
        {
            switch (joystick)
            {
                case Joystick.RIGHT:
                    OnRightJoystickMovement -= callback;
                    break;
                case Joystick.LEFT:
                    OnLeftJoystickMovement -= callback;
                    break;
                default:
                    UnityEngine.Debug.LogError("Error: Unable to attach monitor callback to specified trigger: " + joystick);
                    break;
            }
        }

        //Function to check for all button presses excluding grip and trigger buttons
        private void CheckButtonsPress()
        {
            if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.Touch))
            {
                if (OnButtonAPress != null)
                {
                    OnButtonAPress();
                }

                ButtonAPressed = true;
            }

            if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.Touch))
            {
                if (OnButtonBPress != null)
                {
                    OnButtonBPress();
                }

                ButtonBPressed = true;
            }

            if (OVRInput.GetDown(OVRInput.Button.Three, OVRInput.Controller.Touch))
            {
                if (OnButtonXPress != null)
                {
                    OnButtonXPress();
                }

                ButtonXPressed = true;
            }

            if (OVRInput.GetDown(OVRInput.Button.Four, OVRInput.Controller.Touch))
            {
                if (OnButtonYPress != null)
                {
                    OnButtonYPress();
                }

                ButtonYPressed = true;
            }

        }

        //Function to check for all button releases excluding grip and trigger buttons
        private void CheckButtonsRelease()
        {
            if (OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.Touch))
            {
                if (OnButtonARelease != null)
                {
                    OnButtonARelease();
                }

                ButtonAPressed = false;
            }



            if (OVRInput.GetUp(OVRInput.Button.Two, OVRInput.Controller.Touch))
            {
                if (OnButtonBRelease != null)
                {
                    OnButtonBRelease();
                }

                ButtonBPressed = false;
            }



            if (OVRInput.GetUp(OVRInput.Button.Three, OVRInput.Controller.Touch))
            {
                if (OnButtonXRelease != null)
                {
                    OnButtonXRelease();
                }

                ButtonXPressed = false;
            }



            if (OVRInput.GetUp(OVRInput.Button.Four, OVRInput.Controller.Touch))
            {
                if (OnButtonYRelease != null)
                {
                    OnButtonYRelease();
                }

                ButtonYPressed = false;
            }
        }

        //Check if there is a touch or untouch sensed by the capacitive touch buttons
        private void CheckButtonsTouch()
        {

            bool _buttonATouch = OVRInput.Get(OVRInput.Touch.One, OVRInput.Controller.Touch);
            bool _buttonBTouch = OVRInput.Get(OVRInput.Touch.Two, OVRInput.Controller.Touch);
            bool _buttonXTouch = OVRInput.Get(OVRInput.Touch.Three, OVRInput.Controller.Touch);
            bool _buttonYTouch = OVRInput.Get(OVRInput.Touch.Four, OVRInput.Controller.Touch);
            bool _leftTriggerTouch = OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, OVRInput.Controller.Touch);
            bool _rightTriggerTouch = OVRInput.Get(OVRInput.Touch.SecondaryIndexTrigger, OVRInput.Controller.Touch);

            if (_buttonATouch != ButtonATouched)
            {
                if (_buttonATouch) //button went from untouched to touched
                {
                    if (OnButtonATouchStart != null)
                    {
                        OnButtonATouchStart();
                    }
                }
                else //button went from touched to untouched
                {
                    if (OnButtonATouchEnd != null)
                    {
                        OnButtonATouchEnd();
                    }
                }

                ButtonATouched = !ButtonATouched;
            }

            if (_buttonBTouch != ButtonBTouched)
            {
                if (_buttonBTouch) //button went from untouched to touched
                {
                    if (OnButtonBTouchStart != null)
                    {
                        OnButtonBTouchStart();
                    }
                }
                else //button went from touched to untouched
                {
                    if (OnButtonBTouchEnd != null)
                    {
                        OnButtonBTouchEnd();
                    }
                }

                ButtonBTouched = !ButtonBTouched;
            }

            if (_buttonXTouch != ButtonXTouched)
            {
                if (_buttonXTouch) //button went from untouched to touched
                {
                    if (OnButtonXTouchStart != null)
                    {
                        OnButtonXTouchStart();
                    }
                }
                else //button went from touched to untouched
                {
                    if (OnButtonXTouchEnd != null)
                    {
                        OnButtonXTouchEnd();
                    }
                }

                ButtonXTouched = !ButtonXTouched;
            }

            if (_buttonYTouch != ButtonYTouched)
            {
                if (_buttonYTouch) //button went from untouched to touched
                {
                    if (OnButtonYTouchStart != null)
                    {
                        OnButtonYTouchStart();
                    }
                }
                else //button went from touched to untouched
                {
                    if (OnButtonYTouchEnd != null)
                    {
                        OnButtonYTouchEnd();
                    }
                }

                ButtonYTouched = !ButtonYTouched;
            }

            //Check Triggers' touch state
            if (_leftTriggerTouch != LeftTriggerTouched)
            {
                if (_leftTriggerTouch) //button went from untouched to touched
                {
                    if (OnLeftTriggerTouchStart != null)
                    {
                        OnLeftTriggerTouchStart();
                    }
                }
                else //button went from untouched to touched
                {
                    if (OnLeftTriggerTouchEnd != null)
                    {
                        OnLeftTriggerTouchEnd();
                    }
                }

                LeftTriggerTouched = !LeftTriggerTouched;

            }


            if (_rightTriggerTouch != RightTriggerTouched)
            {
                if (_rightTriggerTouch) //button went from untouched to touched
                {
                    if (OnRightTriggerTouchStart != null)
                    {
                        OnRightTriggerTouchStart();
                    }
                }
                else //button went from untouched to touched
                {
                    if (OnRightTriggerTouchEnd != null)
                    {
                        OnRightTriggerTouchEnd();
                    }
                }

                RightTriggerTouched = !RightTriggerTouched;

            }

        }

        //Check if joysticks are being pressed down on
        private void CheckJoysticksPress()
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.Touch))
            {
                if (OnLeftJoystickPress != null)
                {
                    OnLeftJoystickPress();
                }

                LeftJoystickPressed = true;
            }

            if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick, OVRInput.Controller.Touch))
            {
                if (OnRightJoystickPress != null)
                {
                    OnRightJoystickPress();
                }

                RightJoystickPressed = true;
            }
        }

        //Check if joysticks have been released
        private void CheckJoysticksRelease()
        {
            if (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.Touch))
            {
                if (OnLeftJoystickRelease != null)
                {
                    OnLeftJoystickRelease();
                }

                LeftJoystickPressed = false;
            }

            if (OVRInput.GetUp(OVRInput.Button.SecondaryThumbstick, OVRInput.Controller.Touch))
            {
                if (OnRightJoystickRelease != null)
                {
                    OnRightJoystickRelease();
                }

                RightJoystickPressed = false;
            }

        }

        private void CheckJoysticksMovement()
        {
            CheckLeftJoystickMovement();
            CheckRightJoystickMovement();
        }

        //Check movement of left joystick
        private void CheckLeftJoystickMovement()
        {

            if (OnLeftJoystickMovement != null)
            {
                if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp)) //true if left joystick has been moved upwards more than halfway
                {
                    OnLeftJoystickMovement(Direction.UP);
                }

                if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown)) //true if left joystick has been moved downwards more than halfway
                {
                    OnLeftJoystickMovement(Direction.DOWN);
                }

                if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft)) //true if left joystick has been moved left more than halfway
                {
                    OnLeftJoystickMovement(Direction.LEFT);
                }

                if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight)) //true if left joystick has been moved right more than halfway
                {
                    OnLeftJoystickMovement(Direction.RIGHT);
                }

            }

            //access the joystick's current state as a Vector2
            if (OnLeftJoystickMonitor != null) OnLeftJoystickMonitor(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick));
        }

        //Check movement of right joystick
        private void CheckRightJoystickMovement()
        {

            if (OnRightJoystickMovement != null)
            {
                if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp)) //true if right joystick has been moved upwards more than halfway
                {
                    OnRightJoystickMovement(Direction.UP);

                }

                if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickDown)) //true if right joystick has been moved downwards more than halfway
                {
                    OnRightJoystickMovement(Direction.DOWN);
                }

                if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft)) //true if right joystick has been moved left more than halfway
                {
                    OnRightJoystickMovement(Direction.LEFT);
                }

                if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight)) //true if right joystick has been moved right more than halfway
                {
                    OnRightJoystickMovement(Direction.RIGHT);

                }
            }

            //access the joystick's current state as a Vector2
            if (OnRightJoystickMonitor != null) OnRightJoystickMonitor(OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick));
        }

        //Check if a touch/untouch has been sensed by the joysticks
        private void CheckJoysticksTouch()
        {
            bool _leftJoystickTouch = OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, OVRInput.Controller.Touch);
            bool _rightJoystickTouch = OVRInput.Get(OVRInput.Touch.SecondaryThumbstick, OVRInput.Controller.Touch);

            if (_leftJoystickTouch != LeftJoystickTouched)
            {

                if (_leftJoystickTouch) //button went from untouched to touched
                {
                    if (OnLeftJoystickTouchStart != null)
                    {
                        OnLeftJoystickTouchStart();
                    }
                }
                else //button went from touched to untouched
                {
                    if (OnLeftJoystickTouchEnd != null)
                    {
                        OnLeftJoystickTouchEnd();
                    }
                }

                LeftJoystickTouched = !LeftJoystickTouched;

            }

            if (_rightJoystickTouch != RightJoystickTouched)
            {
                if (_rightJoystickTouch) //button went from untouched to touched
                {
                    if (OnRightJoystickTouchStart != null)
                    {
                        OnRightJoystickTouchStart();
                    }
                }
                else //button went from touched to untouched
                {
                    if (OnRightJoystickTouchEnd != null)
                    {
                        OnRightJoystickTouchEnd();
                    }
                }

                RightJoystickTouched = !RightJoystickTouched;

            }

        }

        //Checks if grip buttons have been pressed
        //Because grip buttons are not touch capacitive, the user must be pressing on the buttons with a force >= 0.01f for this function to be executed
        private void CheckGripsTouch(float _leftGripForce, float _rightGripForce)
        {
            bool _leftGripTouch = _leftGripForce >= _gripTouchRegisterMinForce ? true : false;
            bool _rightGripTouch = _rightGripForce >= _gripTouchRegisterMinForce ? true : false;

            if (_leftGripTouch != LeftGripTouched)
            {
                if (_leftGripTouch) //button went from untouched to touched
                {
                    if (OnLeftGripTouchStart != null)
                    {
                        OnLeftGripTouchStart();
                    }
                }
                else //button went from touched to untouched
                {
                    if (OnLeftGripTouchEnd != null)
                    {
                        OnLeftGripTouchEnd();
                    }
                }

                LeftGripTouched = !LeftGripTouched;

            }

            if (_rightGripTouch != RightGripTouched)
            {
                if (_rightGripTouch) //button went from untouched to touched
                {
                    if (OnRightGripTouchStart != null)
                    {
                        OnRightGripTouchStart();
                    }
                }
                else
                {
                    if (OnRightGripTouchEnd != null)
                    {
                        OnRightGripTouchEnd();
                    }
                }

                RightGripTouched = !RightGripTouched;

            }

        }

        //Check if the grip buttons have been pressed with a force greater than or equal to the respective properties set in the inspector
        private void CheckGripsPress()
        {
            float _leftGripForce = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch);
            float _rightGripForce = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch);

            //Check for grips "touched"
            CheckGripsTouch(_leftGripForce, _rightGripForce);

            //Check for grips pressed
            if (_leftGripForce >= LeftGripPress && !LeftGripPressed)
            {
                if (OnLeftGripPress != null)
                {
                    OnLeftGripPress();
                }

                LeftGripPressed = true;
            }

            if (_rightGripForce >= RightGripPress && !RightGripPressed)
            {
                if (OnRightGripPress != null)
                {
                    OnRightGripPress();
                }

                RightGripPressed = true;
            }

            //access force currently being applied to grip buttons
            if (OnLeftGripMonitor != null) OnLeftGripMonitor(_leftGripForce);
            if (OnRightGripMonitor != null) OnRightGripMonitor(_rightGripForce);

        }

        //Check if grip buttons have been released
        private void CheckGripsRelease()
        {
            if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) <= LeftGripRelease && LeftGripPressed)
            {
                if (OnLeftGripRelease != null)
                {
                    OnLeftGripRelease();
                }

                LeftGripPressed = false;
            }

            if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) <= RightGripRelease && RightGripPressed)
            {
                if (OnRightGripRelease != null)
                {
                    OnRightGripRelease();
                }

                RightGripPressed = false;
            }
        }

        //Check if the trigger buttons have been pressed with a force greater than or equal to the respective properties set in the inspector
        private void CheckTriggersPress()
        {
            float _leftTriggerForce = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
            float _rightTriggerForce = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);

            if (_leftTriggerForce >= LeftTriggerPress && !LeftTriggerPressed)
            {
                if (OnLeftTriggerPress != null)
                {
                    OnLeftTriggerPress();
                }

                LeftTriggerPressed = true;
            }

            if (_rightTriggerForce >= RightTriggerPress && !RightTriggerPressed)
            {
                if (OnRightTriggerPress != null)
                {
                    OnRightTriggerPress();
                }

                RightTriggerPressed = true;
            }

            //access force currently being applied to trigger buttons
            if (OnLeftTriggerMonitor != null) OnLeftTriggerMonitor(_leftTriggerForce);
            if (OnRightTriggerMonitor != null) OnRightTriggerMonitor(_rightTriggerForce);


        }

        //Check if trigger buttons have been released
        private void CheckTriggersRelease()
        {
            if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) <= LeftTriggerRelease && LeftTriggerPressed)
            {
                if (OnLeftTriggerRelease != null)
                {
                    OnLeftTriggerRelease();
                }

                LeftTriggerPressed = false;
            }

            if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) <= RightTriggerRelease && RightTriggerPressed)
            {
                if (OnRightTriggerRelease != null)
                {
                    OnRightTriggerRelease();
                }

                RightTriggerPressed = false;
            }
        }

        //Check if controllers are connected
        private bool CheckForControllers(bool currentValue)
        {
            bool controllersCheck = OVRInput.IsControllerConnected(OVRInput.Controller.LTouch) &&
                                    OVRInput.IsControllerConnected(OVRInput.Controller.RTouch);

            if (currentValue == controllersCheck)
            {
                return currentValue;
            }

            if (onHasController != null)
            {
                onHasController(controllersCheck);
            }

            return controllersCheck;

        }

        private void PlayerFound()
        {
            _inputActive = true;
        }

        private void PlayerLost()
        {
            _inputActive = false;
        }
    }
}
