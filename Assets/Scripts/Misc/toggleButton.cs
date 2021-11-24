using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class toggleButton : MonoBehaviour
{
    private TextMeshProUGUI buttonText;
    // Start is called before the first frame update
    void Start()
    {
        buttonText = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator OnTriggerEnter(Collider other) {
        if (other.tag == "Player")
        {
            gameObject.GetComponent<Outline>().enabled = !gameObject.GetComponent<Outline>().enabled;
            // gameObject.GetComponent<Collider>().isTrigger = true;
            yield return new WaitForSeconds(0.5f);
            // gameObject.GetComponent<Collider>().isTrigger = false;
        }
    }
}
