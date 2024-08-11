using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour
{
    public string sceneName;
    
    public void LoadScene()
    {
        SceneManager.LoadScene("Main Menu");
    }
    /*
    public void LoadLab()
    {
        SceneManager.LoadScene("Element Strucures");
    }*/
}
