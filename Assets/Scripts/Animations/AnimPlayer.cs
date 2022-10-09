using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Veridium_Animation
{
    public class AnimPlayer : AnimationManager
    {
        public bool playOnStart;
        [HideInInspector] public AnimationScript currentAnimation;
        public AnimationScript animationType;
        [SerializeReference] public AnimationBase animScript;

        void Awake()
        {
            if (animScript != null) animScript.OnValidate(this);
        }

        // Start is called before the first frame update
        void Start()
        {

            if (playOnStart) animScript.Play();

        }

        // Update is called once per frame
        void Update()
        {

            animScript.Update();

        }

        // Resets the animation before playing
        public void PlayFromStart()
        {

            animScript.PlayFromStart();

        }

        public void Play()
        {

            animScript.Play();

        }

        public void Pause()
        {

            animScript.Pause();

        }

        public void Reset()
        {

            animScript.Reset();

        }

        public void OnValidate()
        {
            if (currentAnimation != animationType)
            {
                SetAnimation();
                currentAnimation = animationType;
            }

        }

        public static AnimationBase CreateAnimation(AnimationScript type)
        {
            switch (type)
            {
                case AnimationScript.Add_Atoms:
                    return new AddAtoms();
                    break;
                case AnimationScript.Fade:
                    return new Fade();
                    break;
                case AnimationScript.Glow:
                    return new Glow();
                    break;
                case AnimationScript.Glow_Pulse:
                    return new GlowPulse();
                    break;
                case AnimationScript.Move_To:
                    return new MoveTo();
                    break;
                case AnimationScript.Play_On_Atoms:
                    return new PlayOnAtoms();
                    break;
                case AnimationScript.Spin_Up:
                    return new SpinUp();
                    break;
                case AnimationScript.Play_On_Object:
                    return new PlayOnObject();
                    break;
                default:
                    return null;
                    break;
            }
        }

        public static AwaitUserBase CreateAwait(AwaitType type)
        {
            switch (type)
            {
                case AwaitType.Await_Continue:
                    return new AwaitContinue();
                    break;
                case AwaitType.Await_Grab:
                    return new AwaitGrab();
                    break;
                case AwaitType.Await_Insert:
                    return new AwaitInsert();
                    break;
                case AwaitType.Await_Release:
                    return new AwaitRelease();
                    break;
                case AwaitType.Await_Any:
                    return new AwaitAny();
                    break;
                case AwaitType.Await_Sequence:
                    return new AwaitSequence();
                    break;
                default:
                    return null;
                    break;
            }
        }

        public void SetAnimation()
        {
            animScript = CreateAnimation(animationType);
            animScript.OnValidate(this);
        }
    }

#if UNITY_EDITOR
        [CustomEditor(typeof(AnimPlayer))]
        public class AnimPlayerEditor : Editor{

            AnimPlayer player;

            private void OnEnable(){
                player = (AnimPlayer)target;
            }

            public override void OnInspectorGUI(){

                // Draw the default inspector
                DrawDefaultInspector();

                // Add a button to play the sequence
                if(GUILayout.Button("Reset Animation")){
                    player.SetAnimation();
                }
            }
        }

#endif
}
