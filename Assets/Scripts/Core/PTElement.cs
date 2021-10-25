using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTElement : MonoBehaviour
{

    public string elementName;
    private GameObject home;
    private float unHeldTimer = 0f;
    public float maxUnHeldTime = 1f;
    private bool interacted;

    // Start is called before the first frame update
    void Start()
    {

        home = transform.parent.gameObject;
        
    }

    // Update is called once per frame
    void Update()
    {

        if(interacted && unHeldTimer < maxUnHeldTime){
            unHeldTimer += Time.deltaTime;

            if(unHeldTimer > maxUnHeldTime){
                transform.position = home.transform.position;
            }
        } else {
            unHeldTimer = 0f;
        }
        
    }

    public void Interacted(){
        interacted = true;
    }

    public void UnInteracted(){
        interacted = false;
    }
}
