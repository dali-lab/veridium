using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/* allows us to pass collided gameobject upstream through listener */
[System.Serializable]
public class GameObjectEvent : UnityEvent<GameObject>
{
}

public class PointerSelector : MonoBehaviour
{
    public GameObjectEvent onAtomSelect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /* whenever an atom is selected */
    public void OnTriggerEnter(Collider other) {
        Debug.Log("here");
        if (other.gameObject.CompareTag("atom"))
        {
            Debug.Log("collided");
            onAtomSelect.Invoke(other.gameObject);
        }
    }
}
