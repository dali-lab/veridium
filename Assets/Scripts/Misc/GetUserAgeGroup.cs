using System.Collections;
using System.Collections.Generic;
using Oculus.Platform;
using UnityEngine;

public class GetUserAgeGroup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #if !UNITY_EDITOR
        Core.AsyncInitialize();
        UserAgeCategory.Get();
        #endif
    }
    
}
