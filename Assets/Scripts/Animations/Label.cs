using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Label : MonoBehaviour
{

    public string labelText = "Sample Text";
    public Vector2 offset = new Vector2(0.1f, 0.1f);
    public float paddingDMM = 16f;
    public float boxWidth = 0.1f;
    public GameObject textLabel, lineRenderer;
    public GameObject controlPoint1, controlPoint2, controlPoint3, controlPoint4;

    // Start is called before the first frame update
    void Start()
    {

        textLabel.GetComponent<RectTransform>().sizeDelta = new Vector2(boxWidth,0.025f);
        textLabel.transform.localPosition = new Vector3(offset.x,offset.y,0);
        textLabel.GetComponent<TMPro.TextMeshPro>().text = labelText;

        float curveWidth = offset.x - boxWidth/2;

        controlPoint1.transform.localPosition = Vector3.zero;
        controlPoint2.transform.localPosition = new Vector3(curveWidth*0.3f, 0, 0);
        controlPoint3.transform.localPosition = new Vector3(curveWidth*0.7f,offset.y,0);
        controlPoint4.transform.localPosition = new Vector3(curveWidth,offset.y,0);
    }

    // Update is called once per frame
    void Update()
    {
        var camera = FindObjectsOfType<Camera>()[0];

        transform.rotation = Quaternion.LookRotation(textLabel.transform.position - camera.transform.position, Vector3.up);

        transform.localScale = Vector3.one * (textLabel.transform.position - camera.transform.position).magnitude * 3;
    }

    float DMMtoWorldSpace(float DMM){

        return(DMM * (textLabel.transform.position - FindObjectsOfType<Camera>()[0].transform.position).magnitude / 1000);

    }
}
