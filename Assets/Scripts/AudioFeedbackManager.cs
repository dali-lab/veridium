using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFeedbackManager : MonoBehaviour
{
    public List<AudioClip> rightPhrasesEN;
    public List<AudioClip> wrongPhrasesEN;
    public List<AudioClip> rightPhrasesDE;
    public List<AudioClip> wrongPhrasesDE;
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
        if (Language.language == "English")
            audioSource.PlayOneShot(PickRandomClip(rightPhrasesEN));
        else if (Language.language == "German")
            audioSource.PlayOneShot(PickRandomClip(rightPhrasesDE));

    }

    public void PlayWrongAudio()
    {
        if (Language.language == "English")
            audioSource.PlayOneShot(PickRandomClip(wrongPhrasesEN));
        else if (Language.language == "German")
            audioSource.PlayOneShot(PickRandomClip(wrongPhrasesDE));
    }

    // Pick a random audio clip from a list of audio clips
    private AudioClip PickRandomClip(List<AudioClip> clips)
    {
        int index = Random.Range(0, clips.Count);
        return clips[index];
    }
}
