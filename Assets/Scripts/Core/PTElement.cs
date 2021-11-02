using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sib;

public class PTElement : MonoBehaviour
{

    public string elementName;
    public GameObject home;
    private float unHeldTimer = 0f;
    public float maxUnHeldTime = 1f;
    private bool interacted = true;
    public CellType type = CellType.CUBIC;
    public CellVariation variation = CellVariation.SIMPLE;

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

            } else {

                unHeldTimer = 0f;
                gameObject.transform.position = home.transform.position;

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
