using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Veridium_Animation
{

    /// <summary> 
    /// This file contains all of the classes that make up the GUI for the animation system.
    /// The animation GUI system is built on nesting and provides a single interface for
    /// creating complex animations and lectures.
    /// </summary>

    // Basic options for animation scripts. Used to instantiate animations in the 
    // animation hierarchy. See readme for individual functionality
    [System.Serializable]
    public enum AnimationScript
    {
        Anim_AddAtoms,
        Anim_Fade,
        Anim_Glow,
        Anim_GlowPulse,
        Anim_MoveTo,
        Anim_SpinUp,
        Anim_PlayOnAtoms,
        Anim_PlayOnObject
    }

    // Basic options for awaitScripttypes. Used to instantiate awaits in the animation hierarchy.
    // See readme for individual functionality
    public enum AwaitType
    {
        Await_Continue,
        Await_Grab,
        Await_Insert,
        Await_Release,
        Await_Any,
        Await_Sequence
    }

    // Options for types of actions that an AnimSequence can perform. See readme for individual functionality
    public enum ActionType
    {
        AnimationScript,
        Await,
        UnityEvent,
        Animator,
        AnimPlayerReference
    }

    // Options for modifiable parameters for Unity Animators
    public enum AnimatorParameter
    {
        Bool,
        Float,
        Int,
        Trigger
    }

    // A segment of a lecture that lasts as long as the audio clip. Can have any number of animations associated
    [System.Serializable]
    public struct AnimSegment
    {
        public AudioClip audio;                                 // Each segment has one audio clip. This should be used for lecture audio
        public AudioClip audioEN;                               // Each segment has an ENGLISH audio clip
        public AudioClip audioDE;  
        public List<SegmentAnimPlayer> animations;              // List of animations set in the inspector.
        [HideInInspector] public float realDuration;            // The real duration of the segment. Longer than audio clip length if animations run over time
    }

    // Base class for the GUI element for input about a specific action type
    [System.Serializable]
    public class AnimType
    {
        public virtual void Execute() { }
        public virtual void Undo() { }
        [HideInInspector] public AnimationManager manager;
        public virtual void OnValidate(AnimationManager parent) 
        { 
            manager = parent;
        }
    }

    // GUI element for input about Awaits. Also provides an interface for execution
    [System.Serializable]
    public class AwaitUserType : AnimType
    {
        public AwaitType awaitType;
        [HideInInspector] public AwaitType currentType;
        [SerializeReference] public AwaitUserBase awaitScript;
        public override void OnValidate(AnimationManager parent)
        {
            base.OnValidate(parent);
            if (currentType != awaitType || awaitScript == null)
            {
                awaitScript = AnimPlayer.CreateAwait(awaitType);
                awaitScript.OnValidate(manager);
                currentType = awaitType;
            }
            else
            {
                if (awaitScript != null) awaitScript.OnValidate(manager);
            }
        }
        public override void Execute()
        {
            base.Execute();
            if (manager is AnimSequence)
            {
                AnimSequence sequence = manager as AnimSequence;
                sequence.playingAnims.Add(awaitScript);
            }
            awaitScript.Play();
        }
    }

    // GUI element for input about animation scripts. Also provides an interface for execution
    [System.Serializable]
    public class AnimScriptType : AnimType
    {
        public AnimationScript animationType;
        [HideInInspector] public AnimationScript currentAnimation;
        [SerializeReference] public AnimationBase animScript;
        public override void OnValidate(AnimationManager parent)
        {
            base.OnValidate(parent);
            if (currentAnimation != animationType || animScript == null)
            {
                animScript = AnimPlayer.CreateAnimation(animationType);
                animScript.OnValidate(manager);
                currentAnimation = animationType;
            }
            else
            {
                if (animScript != null) animScript.OnValidate(manager);
            }
        }
        public override void Execute()
        {
            base.Execute();

            if (manager is AnimSequence)
            {
                AnimSequence sequence = manager as AnimSequence;
                sequence.playingAnims.Add(animScript);
            }
            animScript.Play();
        }
    }

    // GUI element for input about Unity Events. Also provides an interface for exection and undo
    [System.Serializable]
    public class UnityEventType : AnimType
    {
        public UnityEvent onExecute;
        public UnityEvent onUndo;
        public override void Execute()
        {
            base.Execute();
            onExecute.Invoke();
        }
        public override void Undo()
        {
            base.Undo();
            onUndo.Invoke();
        }
    }

    // GUI element for input about Anim Player References. Also provides an interface for execution 
    // of the referenced animation
    [System.Serializable]
    public class AnimPlayerReferenceType : AnimType
    {
        public AnimPlayer animPlayer;
        public override void Execute()
        {
            base.Execute();
            if (manager is AnimSequence)
            {
                AnimSequence sequence = manager as AnimSequence;
                sequence.playingAnims.Add(animPlayer.animScript);
                animPlayer.Play();
            }
        }
    }

    // GUI element for input about modifying Unity Animator parameters. Also provides an interface 
    // for execution and undo
    [System.Serializable]
    public class AnimatorType : AnimType
    {
        public Animator animator;
        public AnimatorParameter parameterType;
        [HideInInspector] public AnimatorParameter currentParameter;
        public string parameterName;
        [SerializeReference]
        public AnimatorParameterType parameter;
        public AnimatorParameter undoParameterType;
        [HideInInspector] public AnimatorParameter currentUndoParameter;
        public string undoParameterName;
        [SerializeReference]
        public AnimatorParameterType undo;
        public override void OnValidate(AnimationManager parent)
        {
            base.OnValidate(parent);
            if (currentParameter != parameterType || parameter == null)
            {
                switch (parameterType)
                {
                    case AnimatorParameter.Bool:
                        parameter = new BoolType();
                        break;
                    case AnimatorParameter.Float:
                        parameter = new FloatType();
                        break;
                    case AnimatorParameter.Int:
                        parameter = new IntType();
                        break;
                    case AnimatorParameter.Trigger:
                        parameter = new TriggerType();
                        break;
                }
                currentParameter = parameterType;
            }
            if (currentUndoParameter != undoParameterType || undo == null)
            {
                switch (undoParameterType)
                {
                    case AnimatorParameter.Bool:
                        undo = new BoolType();
                        break;
                    case AnimatorParameter.Float:
                        undo = new FloatType();
                        break;
                    case AnimatorParameter.Int:
                        undo = new IntType();
                        break;
                    case AnimatorParameter.Trigger:
                        undo = new TriggerType();
                        break;
                }
                currentUndoParameter = undoParameterType;
            }
        }
        public override void Execute()
        {
            base.Execute();
            parameter.Execute(animator, parameterName);
        }
        public override void Undo()
        {
            base.Undo();
            undo.Execute(animator, undoParameterName);
        }
    }

    // A single animation on a segment that specifies the animation to play and the time into the clip it should start playing
    [System.Serializable]
    public class SegmentAnimPlayer
    {
        [HideInInspector] public AnimationManager manager;
        [HideInInspector] public bool executed;
        [HideInInspector] public ActionType currentActionType;
        public ActionType actionType;
        public float timing;
        [SerializeReference] public AnimType animType;

        public void OnValidate(AnimationManager parent)
        {
            manager = parent;

            if (currentActionType != actionType || animType == null)
            {
                switch (actionType)
                {
                    case ActionType.Await:
                        animType = new AwaitUserType();
                        break;
                    case ActionType.AnimationScript:
                        animType = new AnimScriptType();
                        break;
                    case ActionType.UnityEvent:
                        animType = new UnityEventType();
                        break;
                    case ActionType.Animator:
                        animType = new AnimatorType();
                        break;
                    case ActionType.AnimPlayerReference:
                        animType = new AnimPlayerReferenceType();
                        break;
                }
                currentActionType = actionType;
                animType.OnValidate(manager);
            }
            else
            {
                if (animType != null) animType.OnValidate(manager);
            }
        }
        public bool ShouldExecute()
        {
            if (manager is AnimSequence)
            {
                AnimSequence sequence = manager as AnimSequence;

                if (animType is AnimScriptType)
                {
                    AnimScriptType script = animType as AnimScriptType;
                    bool afterStart = timing < sequence.segmentTime;
                    bool beforeEnd = script.animScript.duration + timing > sequence.segmentTime;
                    bool endless = script.animScript.indefiniteDuration;

                    if (!sequence.playingAnims.Contains(script.animScript) && (afterStart && (beforeEnd || endless)))
                    {
                        return !executed;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return !executed && timing <= sequence.segmentTime;
                }
            }
            else
            {
                return !executed;
            }
        }
        public void Execute()
        {
            executed = true;
            animType.Execute();
        }
        public void Undo()
        {
            executed = false;
            animType.Undo();
        }
    }

    // Base class for GUI elements for input about modifying individual animator parameters
    [System.Serializable]
    public class AnimatorParameterType
    {
        public virtual void Execute(Animator animator, string parameterName) { }
    }

    // GUI element for modifying Int values in animators. Also provides an interface for execution
    [System.Serializable]
    public class IntType : AnimatorParameterType
    {
        public int value;
        public override void Execute(Animator animator, string parameterName)
        {
            base.Execute(animator, parameterName);
            animator.SetInteger(parameterName, value);
        }
    }

    // GUI element for modifying Float values in animators. Also provides an interface for execution
    [System.Serializable]
    public class FloatType : AnimatorParameterType
    {
        public float value;
        public override void Execute(Animator animator, string parameterName)
        {
            base.Execute(animator, parameterName);
            animator.SetFloat(parameterName, value);
        }
    }

    // GUI element for modifying Bool values in animators. Also provides an interface for execution
    [System.Serializable]
    public class BoolType : AnimatorParameterType
    {
        public bool value;
        public override void Execute(Animator animator, string parameterName)
        {
            base.Execute(animator, parameterName);
            animator.SetBool(parameterName, value);
        }
    }

    // Element for setting triggers in animators. Does not take any input. Also provides an interface for execution
    [System.Serializable]
    public class TriggerType : AnimatorParameterType
    {
        public override void Execute(Animator animator, string parameterName)
        {
            base.Execute(animator, parameterName);
            animator.SetTrigger(parameterName);
        }
    }
}