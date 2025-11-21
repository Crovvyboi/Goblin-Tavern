using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SplashScreen : MonoBehaviour
{
    public VideoPlayer player;

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(PlayHandler());   
    }

    public IEnumerator PlayHandler()
    {
        player.Play();
        yield return PlayVideo();
    }

    public IEnumerator PlayVideo()
    {
        while (player.isPlaying)
        {
            yield return null;
        }
        yield return SwitchToMainMenu();
    }
    public IEnumerator SwitchToMainMenu()
    {
        yield return Fader.instance.FadeIn(FaderType.Generic);
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }
}
