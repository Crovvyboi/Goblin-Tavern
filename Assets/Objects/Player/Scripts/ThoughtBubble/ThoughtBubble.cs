using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ThoughtBubble : MonoBehaviour
{
    public static ThoughtBubble instance;

    public CanvasGroup canvasGroup;
    public TextMeshProUGUI thoughtText;

    public bool isFading;
    public float fadeTimer;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        canvasGroup.alpha = 0f;
        isFading = false;
        fadeTimer = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (canvasGroup.alpha == 1f && fadeTimer < 3f)
        {
            fadeTimer += Time.deltaTime;
            if (fadeTimer >= 3f)
            {
                isFading = true;   
            }
        }
        if (isFading)
        {
            canvasGroup.alpha -= 0.2f;
            if (canvasGroup.alpha <= 0f)
            {
                isFading = false;
            }
        }
    }

    public void Think(string text)
    {
        canvasGroup.alpha = 1f;
        isFading = false;
        fadeTimer = 0f;

        thoughtText.text = text;
    }
}
