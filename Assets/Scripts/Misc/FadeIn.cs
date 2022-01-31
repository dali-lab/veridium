using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    // Start is called before the first frame update
    public Color fadeColor;
    public GameObject fadeScreen;
    public float fadeDuration;

    void Start()
    {
        StartCoroutine(FadeScreenIn());
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public IEnumerator FadeScreenIn()
    {
        float timer = 0;
        
        while (timer <= fadeDuration + .1)
        {
            Color colorUpdate = fadeColor;
            colorUpdate.a = Mathf.Lerp(1,0,timer/fadeDuration);
            fadeScreen.GetComponent<Renderer>().material.SetColor("_BaseColor", colorUpdate);

            timer += Time.deltaTime;
            yield return null;
        }

    }
}
