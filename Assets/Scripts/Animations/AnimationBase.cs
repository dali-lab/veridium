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

        public bool playOnStart;                                // Whether this animation should begin playing immediately
        public float duration = 2;                              // Total length of the animation
        UnityEvent FinishedEvent;                               // Event invoked when the animation is finished
        protected float elapsedTime = 0, elapsedTimePercent;    // Total time since the animation has started and time as a fraction of duration
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
            if (elapsedTime >= duration) FinishedPlaying();

        }

        // Call this function to play the animation. Will play from start if the animation is alrady completed
        public virtual void Play(){

            // Start the animation over if it is already at its end
            if(elapsedTime >= duration) {

                PlayFromStart();

                return;

            }

            playing = true;

            if (!begunPlaying) begunPlaying = true;

        }

        // Update function for the animation, will only be called if playing is true
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

        // Resets the animation. Overriden by individual animations
        public virtual void Reset(){

            elapsedTime = 0;
            elapsedTimePercent = 0;

            // Animations collect information when they start playing. Resetting before then can break things
            if (!begunPlaying) return;

        }

        // Internal function for ending an animation
        private void FinishedPlaying(){

            Pause();

            FinishedEvent.Invoke();

        }

        // Exponential easing out
        protected float EaseOut(float x){

            // Function is only valid between 0 and 1
            if (x >= 0.99) return 1f;
            if (x < 0) return 0f;

            // 2/(1+e^-6x) - 1
            return (float) (2 / (1 + Mathf.Pow(2.71828182846f, -6f * x)) - 0.995f);

        }

        // Elastic easing out
        protected float EaseOutElastic(float x){

            // Function is only valid between 0 and 1
            if (x >= 0.99) return 1f;
            if (x < 0) return 0f;

            // 2pi/3
            float c4 = (2 * Mathf.PI) / 3;

            // 2^(-10x) * sin((10x - 3/4) 2pi/3) +1
            return Mathf.Pow(2f, -10f * x) * Mathf.Sin((x * 10f - 0.75f) * c4) + 1;

        }
    }
}