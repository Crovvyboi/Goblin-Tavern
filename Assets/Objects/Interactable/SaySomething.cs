
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaySomething : MonoBehaviour
{
    public List<string> speechLines = new List<string>()
    {
        "I don't know what to say. But thanks for interacting with me!",
        "I'm a red, little cube! :3"
    };
    public GameObject speechbubble;
    public CanvasGroup speechbubbleGroup;
    public TextMeshProUGUI speechtext;

    public float fadeTimer;
    public bool isFading;

    private void Awake()
    {
        speechbubble.SetActive(true);
        speechbubbleGroup = speechbubble.GetComponent<CanvasGroup>();
        speechbubbleGroup.alpha = 0f;
    }

    private void FixedUpdate()
    {
        if (speechbubbleGroup.alpha == 1f && fadeTimer < 2f)
        {
            fadeTimer += Time.deltaTime;
            if (fadeTimer >= 2f)
            {
                isFading = true;
            }
        }
        if (isFading)
        {
            speechbubbleGroup.alpha -= 0.2f;
            if (speechbubbleGroup.alpha <= 0)
            {
                isFading = false;
            }
        }
    }

    public void SaySomethingWithBubble()
    {
        int random = Random.Range(0, speechLines.Count);
        speechbubbleGroup.alpha = 1f;
        fadeTimer = 0f;
        speechtext.text = speechLines[random];

    }

    public void SaySomethingWithBubble(string speechline)
    {
        speechbubbleGroup.alpha = 1f;
        fadeTimer = 0f;
        speechtext.text = speechline;

    }
}
