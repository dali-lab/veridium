using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable] 
public struct LanguageText
{
    public string language;
    public string text;
}

[RequireComponent(typeof(TextMeshPro))]
public class LocalizeText : MonoBehaviour
{
    [SerializeField] List<LanguageText> textVariants;

    void Start()
    {
        foreach (LanguageText textVariant in textVariants)
        {
            if (textVariant.language == Language.language)
            {
                GetComponent<TextMeshPro>().text = textVariant.text;
                return;
            }
        }

        Debug.LogWarning("No text variant found for language " + Language.language);
    }
}
