using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium_Animation;

public class LanguageButtons : MonoBehaviour
{
    [SerializeField] GameObject englishButton;
    [SerializeField] GameObject germanButton;

    [SerializeField] float offset; // how far buttons go down when pressed

    [SerializeField] AnimSequence onboardingSequence;

    // Start is called before the first frame update
    void Start()
    {
        if (Language.language == "English") englishButton.transform.position -= englishButton.transform.up * offset;
        else if (Language.language == "German") germanButton.transform.position -= germanButton.transform.up * offset;
    }

    public void SelectEnglish()
    {
        if (Language.language == "English") return;

        Debug.Log("ENGLISH");
        Language.language = "English";
        englishButton.transform.position -= englishButton.transform.up * offset;
        germanButton.transform.position += germanButton.transform.up * offset;
        if (onboardingSequence != null) onboardingSequence.UpdateLanguage();

    }
    public void SelectGerman()
    {
        if (Language.language == "German") return;

        Debug.Log("GERMAN");
        Language.language = "German";
        englishButton.transform.position += englishButton.transform.up * offset;
        germanButton.transform.position -= germanButton.transform.up * offset;
        if (onboardingSequence != null) onboardingSequence.UpdateLanguage();


    }
}
