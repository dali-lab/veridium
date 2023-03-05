using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginningAudio : MonoBehaviour
{
    [SerializeField] float delay;

    [SerializeField] AudioClip EN;
    [SerializeField] AudioClip DE;
    AudioSource audio => GetComponent<AudioSource>();
    IEnumerator WaitThenPlayAudio()
    {
        yield return new WaitForSeconds(delay);

        if (Language.language == "English") audio.clip = EN;
        else audio.clip = DE;

        audio.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitThenPlayAudio());
    }

    public void StopAudio()
    {
        StopAllCoroutines();
        audio.Stop();
    }
}
