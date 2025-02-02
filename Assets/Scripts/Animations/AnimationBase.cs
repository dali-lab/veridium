using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Veridium.Animation{
    public class AnimationBase : MonoBehaviour
    {

        /// <summary>
        /// Animation base is the parent class of all animations for lecture animation
        /// sequences. Other scripts can interact with these animations using Play(),
        /// Pause(), PlayFromStart(), Reset()
        /// </summary>

        public bool playOnStart, indefiniteDuration;            // Whether this animation should begin playing immediately, and whether it should ever stop
        public float duration = 2;                              // Total length of the animation
        public float elapsedTime {get; private set;}            // Total time since the animation has started
        protected float elapsedTimePercent;                     // Time as a fraction of duration
        public bool playing {get; private set;}                 // Whether this animation is actively playing
        private bool begunPlaying;                              // Used to determine whether the animation should reset before playing
        public bool awaitingAction {get; protected set;}        // Don't set this directly, should only be used in the AwaitUserBase class
        public AnimSequence animSequence;     // A reference to the anim sequence that controls this animation. Should be null if the animation is independent
        [HideInInspector] public bool selfDestruct;


        // Start is called before the first frame update
        protected virtual void Start()
        {

            // Begin playing immediately if playing on start
            if (playOnStart) Play();
            
        }

        // Update is called on each frame
        protected virtual void Update()
        {

            // Increment animation time and update the animation if it is playing
            if (playing){

                UpdateAnim();
                
                elapsedTime += Time.deltaTime;

                if (duration != 0) elapsedTimePercent = elapsedTime/duration;

            }

            // If the animation has run its course, stop it
            if (!indefiniteDuration && elapsedTime >= duration) {
                Pause();
                End();
            }

        }

        // Call this function to play the animation. Will play from start if the animation is already completed
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

        public virtual void End(){

            if(selfDestruct) Destroy(this);

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

            // Animations collect information when they start playing. Resetting before then can break things
            if (begunPlaying) ResetChild();

            begunPlaying = false;

        }

        // This is a separate function so that it can be canceled in the child by Reset
        protected virtual void ResetChild(){

        }

        // Sets the time of the animation to the new time parameter
        public void Scrub(float newTime){
                
            elapsedTime = newTime;

            elapsedTimePercent = elapsedTime/duration;

            UpdateAnim();

            // If the animation has run its course, stop it
            if (!indefiniteDuration && (elapsedTime >= duration || elapsedTime < 0)) Pause();

            if (elapsedTime <= 0) Reset();

        }

    }
}