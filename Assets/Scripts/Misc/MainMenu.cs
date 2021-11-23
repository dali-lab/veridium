using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private GameObject directRightHandInteractor;
    [SerializeField] private GameObject raycastRightHandInteractor;
    [SerializeField] private GameObject mainMenuTipCanvas;

    private bool isMenuOn = false;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false); // disable the menu in the beginning
        mainMenuTipCanvas.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Toggles the MainMenu on and off
    public void toggleMainMenu()
    {
        mainMenuTipCanvas.SetActive(false);
        isMenuOn = !isMenuOn;
        if (isMenuOn)
        {
            gameObject.SetActive(true);
            directRightHandInteractor.SetActive(false);
            raycastRightHandInteractor.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
            raycastRightHandInteractor.SetActive(false);
            directRightHandInteractor.SetActive(true);
        }
    }

    public void toggleMainMenu(bool val)
    {
        isMenuOn = val;
        gameObject.SetActive(val);
    }
}
