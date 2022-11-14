using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class FadeScreen : MonoBehaviour
{

    public GameObject fadeScreen;
    public float fadeDuration;
    public Color fadeColor;

    public IEnumerator fadeRoutine(string scene)
    {
        float timer = 0;

        while (timer <= fadeDuration + .1)
        {
            Color colorUpdate = fadeColor;
            colorUpdate.a = Mathf.Lerp(0, 1, timer / fadeDuration);
            fadeScreen.GetComponent<Renderer>().material.SetColor("_BaseColor", colorUpdate);

            timer += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene(scene);

    }

    public void fadeThenLoadScene(string scene)
    {
        StartCoroutine(fadeRoutine(scene));
    }

    //IEnumerator enterMainSceneRoutine()
}
