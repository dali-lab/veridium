using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToggleTransparency : MonoBehaviour
{

    [SerializeField] private float transparencyPercent = 0.0f;
    [SerializeField] private TextMeshProUGUI transparencyText;
    [SerializeField] private GameObject tooth;
    [SerializeField] private Material toothOriginalMat;
    [SerializeField] private Material toothTransparentMat;
    [SerializeField] private Material offMat;
    [SerializeField] private Material onMat;

    private List<GameObject> toothParts;
    private bool isClicked = false;

    // Start is called before the first frame update
    void Start()
    {
        toothParts = new List<GameObject>();

        Color tempColor = toothTransparentMat.color;
        tempColor.a = transparencyPercent;
        toothTransparentMat.color = tempColor;

        foreach (Transform child in tooth.transform.GetChild(1))
        {
            toothParts.Add(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        if (offMat && onMat)
            GetComponent<MeshRenderer>().material = offMat;
    }

    public void toggleTransparency()
    {
        isClicked = !isClicked;

        foreach (GameObject part in toothParts)
        {
            if (part.name != "Pulp")
            {
                // Color currColor = part.GetComponent<MeshRenderer>().material.color;
                // currColor.a = isClicked ? transparencyPercent : 1.0f;
                // part.GetComponent<MeshRenderer>().material.SetColor("_Color", currColor);
                part.GetComponent<MeshRenderer>().material = isClicked ? toothTransparentMat : toothOriginalMat;

            }
        }
        transparencyText.text = isClicked ? "On" : "Off";
        if (offMat && onMat)
            GetComponent<MeshRenderer>().material = isClicked ? onMat : offMat;
    }
}
