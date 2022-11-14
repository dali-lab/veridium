using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginningAudio : MonoBehaviour
{
    [SerializeField] float delay;

    AudioSource audio => GetComponent<AudioSource>();
    IEnumerator WaitThenPlayAudio()
    {
        yield return new WaitForSeconds(delay);
        audio.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitThenPlayAudio());
    }

    public void StopAudio()
    {
        audio.Stop();
    }
}
