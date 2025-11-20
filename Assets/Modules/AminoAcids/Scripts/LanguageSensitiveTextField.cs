using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Veridium
{
    public class LanguageSensitiveTextField : MonoBehaviour
    {
        public string englishText;
        public string germanText;

        void Start()
        {
            TMP_Text tmp = GetComponent<TMP_Text>();
            if (!tmp) return;

            tmp.text = Language.language switch
            {
                "English" => englishText,
                "German" => germanText,
                _ => englishText // Default to English if no match
            };
        }
    }
}