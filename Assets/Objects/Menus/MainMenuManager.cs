using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public MainMenuState mainMenuState;
    public bool canInteract
    {
        get {
            return _canInteract;
        } 
        set
        {
            _canInteract = value;
            if (_canInteract)
            {
                for (int i = 0; i < buttonHolder.transform.childCount; i++)
                {
                    buttonHolder.transform.GetChild(i).gameObject.GetComponent<Button>().interactable = true;
                }
            }
            else
            {
                for (int i = 0; i < buttonHolder.transform.childCount; i++)
                {
                    buttonHolder.transform.GetChild(i).gameObject.GetComponent<Button>().interactable = false;
                }
            }
        }
    }
    private bool _canInteract;

    public GameObject startScreen;
    public GameObject startScreenText;
    public GameObject buttonScreen;
    public GameObject buttonHolder;

    // Start is called before the first frame update
    void Awake()
    {
        buttonScreen.SetActive(false);

        canInteract = false;
    }

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    private void FixedUpdate()
    {
        if (Input.anyKeyDown)
        {
            if (canInteract)
            {
                switch (mainMenuState)
                {
                    case MainMenuState.StartScreen:
                        StartCoroutine(InteractStartScreen());
                        break;
                }
            }
        }
        
    }

    public IEnumerator LoadScene()
    {
        yield return Fader.instance.FadeOut();

        mainMenuState = MainMenuState.StartScreen;
        canInteract = true;
    }

    public IEnumerator InteractStartScreen()
    {
        canInteract = false;
        startScreenText.SetActive(false);

        yield return PlayStartScreenAnimation();

        mainMenuState = MainMenuState.ButtonScreen;
        canInteract = true;
    }

    public IEnumerator PlayStartScreenAnimation()
    {
        yield return null;

        startScreen.SetActive(false);
        buttonScreen.SetActive(true);
    }

}

public enum MainMenuState
{
    StartScreen,
    ButtonScreen
}


