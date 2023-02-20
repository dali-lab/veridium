using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class ButtonChangeScene : MonoBehaviour
{

    [SerializeField] private float threshold = 0.1f;
    [SerializeField] private float deadZone = 0.025f;

    private bool isPressed;
    private Vector3 startPos;
    // private ConfigurableJoint joint;

    public UnityEvent onPressed;

    void Start()
    {
        startPos = transform.localPosition;
        // joint = GetComponent<ConfigurableJoint>();
    }

    void Update()
    {
        // if (!isPressed && GetValue() + threshold >= 1){
        //     isPressed = true;
        //     onPressed.Invoke();
        // }

        // if (isPressed && ! (GetValue() + threshold >= 1))
        // {
        //     isPressed = false;
        // }

    }

    // private float GetValue()
    // {
    //     var value = Vector3.Distance(startPos, transform.localPosition) / joint.linearLimit.limit;
    //     if (Math.Abs(value) < deadZone){
    //         value = 0;
    //     }
    //     return Mathf.Clamp(value, -1f, 1f);
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "pointer" && !isPressed) 
        {
            Debug.Log("TRIGGERED WITH POINTER");
            isPressed = true;
            onPressed.Invoke();
            StartCoroutine(ButtonPressAnim());
        }
    }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.tag == "pointer")
    //     {
    //         isPressed = false;
    //     }
    // }

    IEnumerator ButtonPressAnim()
    {
        transform.Translate(0f, -0.01f, 0f);
        yield return new WaitForSeconds(0.5f);
        transform.localPosition = startPos;
        isPressed = false;
    }


    //IEnumerator enterMainSceneRoutine()
}
