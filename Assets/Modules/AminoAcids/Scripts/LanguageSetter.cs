using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Veridium.Modules.AminoAcids
{
    public class LanguageSetter : MonoBehaviour
    {
        void Awake()
        {
            Language.language = "German";
        }
    }
}