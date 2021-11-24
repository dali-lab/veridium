using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToggleMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private Material offMat;
    [SerializeField] private Material onMat;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private string offText;
    [SerializeField] private string onText;

    private bool isClicked = false;

    void Start()
    {
        if (menu)
        {
            menu.SetActive(false);
        }
        if (buttonText)
        {
            buttonText.text = offText;
        }
    }

    void OnEnable()
    {
        GetComponent<MeshRenderer>().material = isClicked ? onMat : offMat;
        if (buttonText)
        {
            buttonText.text = isClicked ? onText : offText;
        }
    }

    public void toggleMenu()
    {
        isClicked = !isClicked;
        if (menu)
        {
            menu.SetActive(isClicked ? true : false);
        }
        
        GetComponent<MeshRenderer>().material = isClicked ? onMat : offMat; // material toggle
        if (buttonText) // button text toggle
        {
            buttonText.text = isClicked ? onText : offText;
        }

    }
}
