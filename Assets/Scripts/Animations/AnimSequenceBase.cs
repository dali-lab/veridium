using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIB_Animation{
    public class AnimSequenceBase : MonoBehaviour
    {
        
        /// <summary>
        /// Holds a list of animations and plays them in order. Can await user input
        /// before proceeding. This should be used to implement entire lecture
        /// animation sequences
        /// </summary>

        private int currentIndex = 0;                                   // The index in the list of the animation that is currently playing
        public bool playing {get; private set;}                         // Whether this animation sequence is currently playing or not
        public List<animSegment> segments;                              // A list of segments that contain an audio clip and a set of animations
        private AudioSource audioSource;
        private List<AnimationBase> playingAnims;


        // A segment of a lecture that lasts as long as the audio clip. Can have any number of animations associated
        [System.Serializable]
        public struct animSegment {
            public AudioClip audio;
            public List<animPlayer> animations;
        }

        // A single animation on a segment that specifies the animation to play and the time into the clip it should start playing
        [System.Serializable]
        public struct animPlayer {
            public AnimationBase animation;
            public float timing;
        }

        // Called before start
        void Awake(){

            audioSource = GetComponent<AudioSource>();

            playingAnims = new List<AnimationBase>();

        }

        // Called every frame
        void Update(){

            // Find animations that should be playing
            foreach (animPlayer anim in segments[currentIndex].animations){

                if(!playingAnims.Contains(anim.animation) && anim.timing < audioSource.time && anim.animation.duration + anim.timing > audioSource.time){

                    playingAnims.Add(anim.animation);
                    anim.animation.Play();

                }
            }

            // Find animations that are done playing
            foreach (AnimationBase anim in playingAnims){

                if(!anim.playing) playingAnims.Remove(anim);

            }

            // Determine if everything is ready to move on to the next segment
            if (!audioSource.isPlaying && audioSource.time >= segments[currentIndex].audio.length){

                bool animsBlockingMove = false;

                foreach (AnimationBase anim in playingAnims){
                    if (!anim.indefiniteDuration) animsBlockingMove = true;
                }

                if (!animsBlockingMove) PlayNextSegment();

            }
        }

        // Play the sequence. This will start from where it last left off if paused
        public void PlaySequence(){

            playing = true;

            // If the sequence has already ended, start it over
            if(currentIndex >= segments.Count - 1) currentIndex = 0;

            PlaySegment(segments[currentIndex]);

        }

        private void PlaySegment(animSegment segment){

            if (audioSource == null) return;

            audioSource.Stop();
            audioSource.clip = segment.audio;
            audioSource.Play();

        }

        // Pauses the current animation
        public void PauseSequence(){

            playing = false;

            audioSource.Pause();

            foreach (AnimationBase anim in playingAnims){
                anim.Pause();
            }

        }

        // Resets the entire sequence, and prompts each animation that has already begun playing to reset
        public void ResetSequence(){

            audioSource.Pause();

            // Reset all animations
            for (int i = 0; i < segments.Count; i++)
            {
                foreach (animPlayer anim in segments[i].animations){
                    anim.animation.Reset();
                }
            }

            currentIndex = 0;

        }

        // Resets the sequence and plays from the start
        public void PlaySequenceFromStart(){

            ResetSequence();
            PlaySequence();

        }

        // Plays the next animation in the list and adds the listener to it
        private void PlayNextSegment(){

            audioSource.Stop();

            foreach (AnimationBase anim in playingAnims){
                anim.Pause();
            }

            currentIndex ++;

            audioSource.clip = segments[currentIndex].audio;

            audioSource.Play();

        }

        public void ScrubSequence(float timeDelta){

            

        }
    }
}
