using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonInteractions : MonoBehaviour
{
    public MainMenuManager mainMenuManager;

    public GameObject closeGameWarning;

    private void Awake()
    {
        closeGameWarning.SetActive(false);
    }

    public void ToDevScene()
    {
        StartCoroutine(ToDevSceneAsync());
    }
    public IEnumerator ToDevSceneAsync()
    {
        yield return Fader.instance.FadeIn(FaderType.Tooltip);
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
    }

    public void NewGame()
    {

    }

    public void ContinueLastSave()
    {

    }

    public void OpenLoadGame()
    {

    }

    public void OpenSettings()
    {

    }

    public void OpenCloseGameWarning()
    {
        mainMenuManager.canInteract = false;

        closeGameWarning.SetActive(true);
    }

    public void CloseCloseGameWarning()
    {
        closeGameWarning.SetActive(false);

        mainMenuManager.canInteract = true;
    }

    public void ConfirmCloseGame()
    {
        #if UNITY_EDITOR
                Fader.instance.FadeOut();
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
