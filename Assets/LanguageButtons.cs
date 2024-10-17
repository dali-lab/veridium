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

    [SerializeField] Material normalMaterial;
    [SerializeField] Material selectedMaterial;

    // Start is called before the first frame update
    void Start()
    {
        if (Language.language == "English") {
            englishButton.transform.position -= englishButton.transform.up * offset;
            englishButton.GetComponent<MeshRenderer>().material = selectedMaterial;
        }
        else if (Language.language == "German") {
            germanButton.transform.position -= germanButton.transform.up * offset;
            germanButton.GetComponent<MeshRenderer>().material = selectedMaterial;
        }
    }

    public void SelectEnglish()
    {
        if (Language.language == "English") return;

        Debug.Log("ENGLISH");
        Language.language = "English";
        englishButton.transform.position -= englishButton.transform.up * offset;
        englishButton.GetComponent<MeshRenderer>().material = selectedMaterial;
        germanButton.transform.position += germanButton.transform.up * offset;
        germanButton.GetComponent<MeshRenderer>().material = normalMaterial;
        if (onboardingSequence != null) onboardingSequence.UpdateLanguage();

    }
    public void SelectGerman()
    {
        if (Language.language == "German") return;

        Debug.Log("GERMAN");
        Language.language = "German";
        englishButton.transform.position += englishButton.transform.up * offset;
        englishButton.GetComponent<MeshRenderer>().material = normalMaterial;
        germanButton.transform.position -= germanButton.transform.up * offset;
        germanButton.GetComponent<MeshRenderer>().material = selectedMaterial;
        if (onboardingSequence != null) onboardingSequence.UpdateLanguage();


    }
}
