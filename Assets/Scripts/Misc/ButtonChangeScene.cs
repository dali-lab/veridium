using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class ButtonChangeScene : MonoBehaviour
{

    [SerializeField] private float threshold = 0.1f;
    [SerializeField] private float deadZone = 0.025f;

    private bool isPressed;
    private Vector3 startPos;
    private ConfigurableJoint joint;

    public GameObject fadeScreen;
    public float fadeDuration;
    public Color fadeColor;

    void Start()
    {
        startPos = transform.localPosition;
        joint = GetComponent<ConfigurableJoint>();
    }

    void Update()
    {
        if (!isPressed && GetValue() + threshold >= 1){
            Pressed();
        }
        else if (isPressed){
            StartCoroutine(fadeRoutine());
        }

    }

    private float GetValue()
    {
        var value = Vector3.Distance(startPos, transform.localPosition) / joint.linearLimit.limit;
        if (Math.Abs(value) < deadZone){
            value = 0;
        }
        return Mathf.Clamp(value, -1f, 1f);
    }

    private void Pressed()
    {
        isPressed = true;
    }

    public void enterMainScene()
    {
        SceneManager.LoadScene(1);
    }

    public IEnumerator fadeRoutine()
    {
        float timer = 0;
        
        while (timer <= fadeDuration + .1)
        {
            Color colorUpdate = fadeColor;
            colorUpdate.a = Mathf.Lerp(0,1,timer/fadeDuration);
            fadeScreen.GetComponent<Renderer>().material.SetColor("_BaseColor", colorUpdate);

            timer += Time.deltaTime;
            yield return null;
        }

        enterMainScene();

    }

    //IEnumerator enterMainSceneRoutine()
}
