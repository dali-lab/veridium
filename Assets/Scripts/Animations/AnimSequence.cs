using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Veridium_Animation
{

    public class AnimSequence : AnimationManager
    {

        private int currentIndex = 0;                                   // The index in the list of the animation that is currently playing
        public bool playOnStart;                                        // Whether the animation sequence should play immediately
        public bool playing { get; private set; }                         // Whether this animation sequence is currently playing or not
        private bool audioHasFinished;                                  // Used to record the end of audio for sequence progression
        public List<AnimSegment> segments;
        public AudioSource audioSource;                                 // The audio source for this animation's audio
        [HideInInspector]
        public List<AnimationBase> playingAnims;                        // A list of currently playing animations
        public string sequenceState;                                    // For debug purposes
        public float segmentTime { get; private set; }                    // Time since the beginning of the segment. Equal to audio time if audio has not finished


        void OnValidate()
        {
            for (int j = 0; j < segments.Count; j++)
            {
                AnimSegment segment = segments[j];
                for (int i = 0; i < segment.animations.Count; i++)
                {
                    SegmentAnimPlayer player = segment.animations[i];
                    if (player == null || i > 0 && segments[j].animations[i].animType == segments[j].animations[i - 1].animType)
                    {
                        segment.animations[i] = new SegmentAnimPlayer();
                        segments[j] = segment;
                    }
                    if (player != null) player.OnValidate(this);
                }
            }
        }

        // Called before start
        void Awake()
        {

            if (audioSource == null) audioSource = GetComponent<AudioSource>();

            playingAnims = new List<AnimationBase>();

            // Find the real ending durations of all segments. These will be longer than audio
            // clip length when animations run longer than it.
            for (int i = 0; i < segments.Count; i++)
            {
                AnimSegment segment = segments[i];
                segment.realDuration = GetSegmentRealDuration(segment);
                segments[i] = segment;
            }
        }

        private float GetSegmentRealDuration(AnimSegment segment)
        {

            float latestTime = segment.audio.length;

            foreach (SegmentAnimPlayer anim in segment.animations)
            {
                if (anim.actionType == ActionType.AnimationScript)
                {
                    AnimScriptType animationType = anim.animType as AnimScriptType;
                    float end = anim.timing + animationType.animScript.duration;
                    if (end > latestTime) latestTime = end;
                }
            }

            return latestTime;

        }

        void Start()
        {

            if (playOnStart) PlaySequence();

        }

        // Called every frame
        void Update()
        {

            // Record the time since the beginning of the audio, even if it has finished
            if (audioHasFinished)
            {
                if (playing) segmentTime += Time.deltaTime;
            }
            else
            {
                segmentTime = audioSource.time;
            }

            sequenceState += "playing: " + playing.ToString() + "\n";
            sequenceState += "current audio: " + audioSource.clip.ToString() + "\n";

            if (playing) UpdateAnimations();

            sequenceState += "audioHasFinished: " + audioHasFinished.ToString() + "\n";
            GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>().text = sequenceState;
            sequenceState = "";
        }

        // Pure function that returns whether the sequence is not being blocked from moving to the next segment
        public bool CanMoveOn()
        {
            bool animsBlockingMove = false;

            // Find animations playing or waiting for input
            foreach (AnimationBase anim in playingAnims)
            {
                if (!anim.indefiniteDuration || anim.awaitingAction || anim.elapsedTime < anim.duration) animsBlockingMove = true;
            }

            // Find if there are animations that have not yet played
            foreach (SegmentAnimPlayer anim in segments[currentIndex].animations)
            {
                if (anim.timing >= segmentTime) animsBlockingMove = true;
            }

            return !animsBlockingMove;
        }

        private void UpdateAnimations()
        {

            // Update all animations
            foreach (AnimationBase anim in playingAnims)
            {
                anim.Update();
            }

            // Find animations that should be playing
            foreach (SegmentAnimPlayer anim in segments[currentIndex].animations)
            {
                if (anim.ShouldExecute()) anim.Execute();
            }

            List<AnimationBase> animsToRemove = new List<AnimationBase>();
            // Find animations that are done playing
            foreach (AnimationBase anim in playingAnims)
            {

                if (!anim.playing || anim.elapsedTime < 0)
                {

                    anim.Pause();
                    anim.End();
                    animsToRemove.Add(anim);
                    Debug.Log("Removing animation " + anim.ToString());

                }
            }

            // Remove animations that are done playing
            foreach (AnimationBase anim in animsToRemove)
            {
                playingAnims.Remove(anim);
            }

            // Determine if everything is ready to move on to the next segment
            sequenceState += "segment time: " + string.Format("{0:0.0}", segmentTime) + "\n";
            sequenceState += "audio length: " + segments[currentIndex].audio.length.ToString() + "\n";
            sequenceState += "audio is playing: " + audioSource.isPlaying.ToString() + "\n";

            bool audioFinished = !audioSource.isPlaying && audioSource.time >= segments[currentIndex].audio.length;
            bool audioNearlyFinished = audioSource.isPlaying && audioSource.time >= segments[currentIndex].audio.length - 2 * Time.deltaTime;
            if (!audioHasFinished) audioHasFinished = audioFinished || audioNearlyFinished;

            // Determine whether there are animations blocking the transition to the next segment
            if (audioHasFinished && CanMoveOn()) PlayNextSegment();

            string animsString = "";
            foreach (AnimationBase anim in playingAnims)
            {
                animsString += anim.ToString() + ", ";
            }
            sequenceState += "animations playing: " + animsString + "\n";

        }

        // Play the sequence. This will start from where it last left off if paused
        public void PlaySequence()
        {

            playing = true;

            // If the sequence has already ended, start it over
            if (currentIndex >= segments.Count - 1) currentIndex = 0;

            PlaySegment(segments[currentIndex]);

        }

        private void PlaySegment(AnimSegment segment)
        {

            if (audioSource == null) return;

            audioSource.Stop();
            audioSource.clip = segment.audio;
            audioSource.Play();

            audioHasFinished = false;

        }

        // Plays the next animation in the list and adds the listener to it
        public void PlayNextSegment()
        {

            audioSource.Stop();

            foreach (AnimationBase anim in playingAnims)
            {
                anim.Pause();
                anim.End();
            }

            currentIndex++;
            audioSource.clip = segments[currentIndex].audio;
            audioSource.Play();
            audioHasFinished = false;

        }

        // Pauses the current animation
        public void PauseSequence()
        {

            playing = false;

            audioSource.Pause();

            foreach (AnimationBase anim in playingAnims)
            {
                anim.Pause();
            }

        }

        // Resets the entire sequence, and prompts each animation that has already begun playing to reset
        public void ResetSequence()
        {

            PauseSequence();

            // Reset all animations
            for (int i = 0; i < segments.Count; i++)
            {
                foreach (SegmentAnimPlayer anim in segments[i].animations)
                {
                    if (anim.actionType == ActionType.AnimationScript)
                    {
                        AnimationBase animScript = (anim.animType as AnimScriptType).animScript;
                        if (animScript != null) animScript.Reset();
                    }
                }
            }

            currentIndex = 0;
            audioHasFinished = false;

        }

        // Resets the sequence and plays from the start
        public void PlaySequenceFromStart()
        {

            ResetSequence();
            PlaySequence();

        }

    }
}