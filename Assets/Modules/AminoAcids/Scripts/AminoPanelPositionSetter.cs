using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AminoPanelPositionSetter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [ContextMenu("Set Position")]
    public void SetPosition()
    {
        transform.localPosition = transform.GetChild(0).localPosition;
        foreach (Transform child in transform)
        {
            child.localPosition = Vector3.zero;
        }
    }
}
