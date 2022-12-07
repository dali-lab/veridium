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
                case AnimationScript.Anim_AddAtoms:
                    return new Anim_AddAtoms();
                    break;
                case AnimationScript.Anim_Fade:
                    return new Anim_Fade();
                    break;
                case AnimationScript.Anim_Glow:
                    return new Anim_Glow();
                    break;
                case AnimationScript.Anim_GlowPulse:
                    return new Anim_GlowPulse();
                    break;
                case AnimationScript.Anim_MoveTo:
                    return new Anim_MoveTo();
                    break;
                case AnimationScript.Anim_PlayOnAtoms:
                    return new Anim_PlayOnAtoms();
                    break;
                case AnimationScript.Anim_SpinUp:
                    return new Anim_SpinUp();
                    break;
                case AnimationScript.Anim_PlayOnObject:
                    return new Anim_PlayOnObject();
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
                    return new Await_Continue();
                    break;
                case AwaitType.Await_Grab:
                    return new Await_Grab();
                    break;
                case AwaitType.Await_Insert:
                    return new Await_Insert();
                    break;
                case AwaitType.Await_Release:
                    return new Await_Release();
                    break;
                case AwaitType.Await_Any:
                    return new Await_Any();
                    break;
                case AwaitType.Await_Sequence:
                    return new Await_Sequence();
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
