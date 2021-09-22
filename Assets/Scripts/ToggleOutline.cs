using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleOutline : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void toggleOutline(bool value)
    {
        GetComponent<Outline>().enabled = value;
    }
}
