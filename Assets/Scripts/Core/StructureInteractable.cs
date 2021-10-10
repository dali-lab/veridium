using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StructureInteractable : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interacted(){
        (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Interacted Structure";
    }
    
}
