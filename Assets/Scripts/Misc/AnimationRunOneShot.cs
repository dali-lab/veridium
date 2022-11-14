using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationRunOneShot : MonoBehaviour
{

    public Animator sphereAnim;

    void SetParamFalse(string paramName)
    {
        sphereAnim.SetBool(paramName, false);
    }


}
