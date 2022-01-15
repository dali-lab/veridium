using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIB_Animation{
    public class AnimSequence : MonoBehaviour
    {
        
        /// <summary>
        /// Holds a list of animations and plays them in order. Can await user input
        /// before proceeding. This should be used to implement entire lecture
        /// animation sequences
        /// </summary>

        private int currentIndex = 0;                                   // The index in the list of the animation that is currently playing
        public bool playOnStart;                                        // Whether the animation sequence should play immediately
        public bool playing {get; private set;}                         // Whether this animation sequence is currently playing or not
        private bool audioHasFinished;                                  // Used to record the end of audio for sequence progression
        public List<AnimSegment> segments;                              // A list of segments that contain an audio clip and a set of animations
        public AudioSource audioSource;                                 // The audio source for this animation's audio
        private List<AnimationBase> playingAnims;                       // A list of currently playing animations
        public string sequenceState;                                    // For debug purposes
        private float segmentTime;                                      // Time since the beginning of the segment. Equal to audio time if audio has not finished


        // A segment of a lecture that lasts as long as the audio clip. Can have any number of animations associated
        [System.Serializable]
        public struct AnimSegment {
            public AudioClip audio;                         // Each segment has one audio clip. This should be used for lecture audio
            public List<AnimPlayer> animations;             // List of animations set in the inspector.
            [HideInInspector] public float realDuration;    // The real duration of the segment. Longer than audio clip length if animations run over time
        }

        // A single animation on a segment that specifies the animation to play and the time into the clip it should start playing
        [System.Serializable]
        public struct AnimPlayer {
            public AnimationBase animation;                 // Animation to play at this time. Set in inspector.
            public float timing;                            // Start time in seconds of the animation from the beginning of the segment
        }

        // Called before start
        void Awake(){

            if(audioSource == null) audioSource = GetComponent<AudioSource>();

            playingAnims = new List<AnimationBase>();

            // Find the real ending durations of all segments. These will be longer than audio
            // clip length when animations run longer than it.
            for(int i = 0; i < segments.Count; i++)
            {
                AnimSegment segment = segments[i];
                segment.realDuration = GetSegmentRealDuration(segment);
                segments[i] = segment;

                // Give each animation a reference to the anim sequence
                foreach (AnimPlayer anim in segment.animations)
                {
                    anim.animation.animSequence = this;
                }
            }

        }

        void Start(){

            if(playOnStart) PlaySequence();

        }

        // Called every frame
        void Update(){

            // Record the time since the beginning of the audio, even if it has finished
            if (audioHasFinished){
                if(playing) segmentTime += Time.deltaTime;
            } else {
                segmentTime = audioSource.time;
            }

            sequenceState += "playing: " + playing.ToString() + "\n";
            sequenceState += "current audio: " + audioSource.clip.ToString() + "\n";

            if(playing) UpdateAnimations();

            sequenceState += "audioHasFinished: " + audioHasFinished.ToString() + "\n";
            //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = sequenceState;
            sequenceState = "";

        }

        private void UpdateAnimations(){

            // Find animations that should be playing
            foreach (AnimPlayer anim in segments[currentIndex].animations){

                bool afterStart = anim.timing < segmentTime;
                bool beforeEnd = anim.animation.duration + anim.timing > segmentTime;
                bool endless = anim.animation.indefiniteDuration;

                if(!playingAnims.Contains(anim.animation) && (afterStart && (beforeEnd || endless))){

                    playingAnims.Add(anim.animation);
                    anim.animation.Play();

                }
            }

            // Find animations that are done playing
            foreach (AnimationBase anim in playingAnims){

                if(!anim.playing || anim.elapsedTime < 0){

                    anim.Pause();
                    playingAnims.Remove(anim);

                }

            }

            // Determine if everything is ready to move on to the next segment
            sequenceState += "segment time: " + string.Format("{0:0.0}", segmentTime) + "\n";
            sequenceState += "audio length: " + segments[currentIndex].audio.length.ToString() + "\n";
            sequenceState += "audio is playing: " + audioSource.isPlaying.ToString() + "\n";

            bool audioFinished = !audioSource.isPlaying && audioSource.time >= segments[currentIndex].audio.length;
            bool audioNearlyFinished  = audioSource.isPlaying && audioSource.time >= segments[currentIndex].audio.length - 2 * Time.deltaTime;
            if (!audioHasFinished) audioHasFinished = audioFinished || audioNearlyFinished;

            // Determine whether there are animations blocking the transition to the next segment
            if (audioHasFinished){

                bool animsBlockingMove = false;

                // Find animations playing or waiting for input
                foreach (AnimationBase anim in playingAnims){
                    if (!anim.indefiniteDuration || anim.awaitingAction || anim.elapsedTime < anim.duration) animsBlockingMove = true;
                }

                // Find if there are animations that have not yet played
                foreach (AnimPlayer anim in segments[currentIndex].animations){
                    if (anim.timing + 2f >= segmentTime) animsBlockingMove = true;
                }

                if (!animsBlockingMove) PlayNextSegment();

            }

            string animsString = "";
            foreach (AnimationBase anim in playingAnims){
                animsString += anim.ToString() + ", ";
            }
            sequenceState += "animations playing: " + animsString + "\n";

        }

        // Play the sequence. This will start from where it last left off if paused
        public void PlaySequence(){

            playing = true;

            // If the sequence has already ended, start it over
            if(currentIndex >= segments.Count - 1) currentIndex = 0;

            PlaySegment(segments[currentIndex]);

        }

        private void PlaySegment(AnimSegment segment){

            if (audioSource == null) return;

            audioSource.Stop();
            audioSource.clip = segment.audio;
            audioSource.Play();

            audioHasFinished = false;

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

            PauseSequence();

            // Reset all animations
            for (int i = 0; i < segments.Count; i++)
            {
                foreach (AnimPlayer anim in segments[i].animations){
                    anim.animation.Reset();
                }
            }

            currentIndex = 0;
            audioHasFinished = false;

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
            audioHasFinished = false;

        }

        public void ScrubSequence(float timeDelta){

            float newTime = audioSource.time + timeDelta;

            // Skip backward if reaching the beginning of the segment
            if(newTime < 0){

                audioSource.Stop();

                foreach (AnimationBase anim in playingAnims){
                    anim.Pause();
                    anim.Reset();
                }

                currentIndex--;

                PlaySegment(segments[currentIndex]);

                newTime += segments[currentIndex].realDuration;

            }

            audioSource.time = newTime;

            // Record the time since the beginning of the audio, even if it has finished
            if (audioHasFinished){
                if(playing) segmentTime += timeDelta;
            } else {
                segmentTime = audioSource.time;
            }

            // Find animations that should be playing
            foreach (AnimPlayer anim in segments[currentIndex].animations){

                if(!playingAnims.Contains(anim.animation) && anim.timing < audioSource.time && anim.animation.duration + anim.timing > audioSource.time){

                    playingAnims.Add(anim.animation);
                    anim.animation.Play();

                }
            }

            // Scrub animations
            foreach(AnimationBase anim in playingAnims){

                float startTime = 0;

                foreach(AnimPlayer timing in segments[currentIndex].animations){
                    if(timing.animation == anim) startTime = timing.timing;
                }

                anim.Scrub(newTime - startTime);
            }

            UpdateAnimations();

            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = sequenceState;

        }

        private float GetSegmentRealDuration(AnimSegment segment){

            float latestTime = segment.audio.length;

            foreach (AnimPlayer anim in segment.animations)
            {
                float end = anim.timing + anim.animation.duration;
                if (end > latestTime) latestTime = end;
            }

            return latestTime;

        }
    }
}
