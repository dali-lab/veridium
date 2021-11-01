using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTElement : MonoBehaviour
{

    public string elementName;
    public GameObject home;
    private float unHeldTimer = 0f;
    public float maxUnHeldTime = 1f;
    private bool interacted = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(!interacted){

            if(unHeldTimer < maxUnHeldTime){

                unHeldTimer += Time.deltaTime;

                //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = unHeldTimer.ToString();

            } else {

                unHeldTimer = 0f;
                gameObject.transform.position = home.transform.position;
                (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "moving back home";

            }

        }
        
    }

    public void Interacted(){
        interacted = true;
    }

    public void UnInteracted(){
        interacted = false;
    }
}
