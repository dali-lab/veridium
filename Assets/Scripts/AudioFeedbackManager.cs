using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFeedbackManager : MonoBehaviour
{
    public List<AudioClip> rightPhrases;
    public List<AudioClip> wrongPhrases;
    public bool finishedAudio = true;

    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        finishedAudio = !audioSource.isPlaying;
    }

    public void PlayCorrectAudio()
    {
        audioSource.PlayOneShot(PickRandomClip(rightPhrases));
    }

    public void PlayWrongAudio()
    {
        audioSource.PlayOneShot(PickRandomClip(wrongPhrases));
    }

    // Pick a random audio clip from a list of audio clips
    private AudioClip PickRandomClip(List<AudioClip> clips)
    {
        int index = Random.Range(0, rightPhrases.Count);
        return clips[index];
    }
}
