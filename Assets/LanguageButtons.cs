using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageButtons : MonoBehaviour
{
    [SerializeField] GameObject englishButton;
    [SerializeField] GameObject germanButton;

    [SerializeField] float offset; // how far buttons go down when pressed

    // Start is called before the first frame update
    void Start()
    {
        englishButton.transform.position -= englishButton.transform.up * offset;
    }

    public void SelectEnglish()
    {
        if (Language.language == "English") return;

        Debug.Log("ENGLISH");
        Language.language = "English";
        englishButton.transform.position -= englishButton.transform.up * offset;
        germanButton.transform.position += germanButton.transform.up * offset;

    }
    public void SelectGerman()
    {
        if (Language.language == "German") return;

        Debug.Log("GERMAN");
        Language.language = "German";
        englishButton.transform.position += englishButton.transform.up * offset;
        germanButton.transform.position -= germanButton.transform.up * offset;

    }
}
