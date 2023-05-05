using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LanguageButton : MonoBehaviour
{
    [SerializeField] UnityEvent onPress;


    private void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals("pointer")) return;

        onPress.Invoke();
    }
}
