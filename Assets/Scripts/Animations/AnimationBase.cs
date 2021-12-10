using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SIB_Animation{
    public class AnimationBase : MonoBehaviour
    {

        public bool playOnStart;
        public float duration = 2;
        UnityEvent FinishedEvent;
        protected float elapsedTime = 0;
        public bool playing {get; private set;}

        // Start is called before the first frame update
        protected virtual void Start()
        {
            
            if (FinishedEvent == null){
                FinishedEvent = new UnityEvent();
            }

            if (playOnStart) Play();
            
        }

        protected virtual void Update()
        {

            if (playing){
                
                elapsedTime += Time.deltaTime;

                UpdateAnim();

            }

            if (elapsedTime >= duration) FinishedPlaying();

        }

        public virtual void Play(){

            if(elapsedTime >= duration) {

                PlayFromStart();

                return;

            }

            playing = true;

        }

        protected virtual void UpdateAnim(){

        }

        public virtual void Pause(){

            playing = false;

        }

        public virtual void PlayFromStart(){

            Reset();

            elapsedTime = 0;

            Play();

        }

        public virtual void Reset(){

        }

        private void FinishedPlaying(){

            Pause();

            FinishedEvent.Invoke();

        }
    }
}