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
        playGame();
    }

    public void playGame()
    {
        SceneManager.LoadScene(1);
    }
}
