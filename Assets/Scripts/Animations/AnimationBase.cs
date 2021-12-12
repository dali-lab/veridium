using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SIB_Animation{
    public class AnimationBase : MonoBehaviour
    {

        /// <summary>
        /// Animation base is the parent class of all animations for lecture animation
        /// sequences. Other scripts can interact with these animations using Play(),
        /// Pause(), PlayFromStart(), Reset(), and by adding listeners to the 
        /// FinishedEvent. 
        /// </summary>

        public bool playOnStart, indefiniteDuration;            // Whether this animation should begin playing immediately, and whether it should ever stop
        public float duration = 2;                              // Total length of the animation
        public UnityEvent FinishedEvent {get; private set;}     // Event invoked when the animation is finished
        public float elapsedTime {get; private set;}            // Total time since the animation has started
        protected float elapsedTimePercent;                     // Time as a fraction of duration
        public bool playing {get; private set;}                 // Whether this animation is actively playing
        private bool begunPlaying;                              // Used to determine whether the animation should reset before playing


        void Awake(){

            // Initialize the event if it hasn't been
            if (FinishedEvent == null) FinishedEvent = new UnityEvent();

        }

        // Start is called before the first frame update
        protected virtual void Start()
        {

            // Begin playing immediately if playing on start
            if (playOnStart) Play();
            
        }

        // Update is called on each frame
        void Update()
        {

            // Increment animation time and update the animation if it is playing
            if (playing){
                
                elapsedTime += Time.deltaTime;

                elapsedTimePercent = elapsedTime/duration;

                UpdateAnim();

            }

            // If the animation has run its course, stop it
            if (!indefiniteDuration && elapsedTime >= duration) FinishedPlaying();

        }

        // Call this function to play the animation. Will play from start if the animation is alrady completed
        public virtual void Play(){

            // Start the animation over if it is already at its end
            if(!indefiniteDuration && elapsedTime >= duration) {

                elapsedTime = 0;
                elapsedTimePercent = 0;

            }

            playing = true;

            if (!begunPlaying) begunPlaying = true;

        }

        // Update function for the animation, will only be called if playing is true or scrubbing
        protected virtual void UpdateAnim(){

        }

        // Pauses the animation. Use this, never set playing to false any other way
        public virtual void Pause(){

            playing = false;

        }

        // Resets the animation before playing
        public virtual void PlayFromStart(){

            Reset();
            Play();

        }

        public virtual void PlayFromEnd(){

            Play();
            Pause();

            elapsedTime = duration;
            elapsedTimePercent = 1;

            UpdateAnim();

        }

        // Resets the animation. Overriden by individual animations
        public void Reset(){

            elapsedTime = 0;
            elapsedTimePercent = 0;
            FinishedEvent = new UnityEvent();

            // Animations collect information when they start playing. Resetting before then can break things
            if (begunPlaying) ResetChild();

            begunPlaying = false;

        }

        // This is a separate function so that it can be canceled in the child by Reset
        protected virtual void ResetChild(){

        }

        // Internal function for ending an animation
        private void FinishedPlaying(){

            Pause();

            FinishedEvent.Invoke();

        }

        // Scrubs the animation backward or forward depending on the sign of time delta
        public void Scrub(float newTime){
                
            elapsedTime = newTime;

            elapsedTimePercent = elapsedTime/duration;

            UpdateAnim();

            // If the animation has run its course, stop it
            if (!indefiniteDuration && (elapsedTime >= duration || elapsedTime < 0)) FinishedPlaying();

        }

    }
}