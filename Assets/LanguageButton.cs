using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LanguageButton : MonoBehaviour
{
    [SerializeField] GameObject tip;

    [SerializeField] UnityEvent onPress;


    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.gameObject.Equals(tip)) return;

        onPress.Invoke();
    }
}
