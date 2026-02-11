using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public static StaminaBar instance;
    public PlayerControls playerControls;

    public CanvasGroup canvasGroup;
    public Slider slider;
    public RawImage sliderImage;

    public bool isFading;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isFading)
        {
            canvasGroup.alpha -= 0.01f;
        }
    }

    public void SetSliderPercent(float percent)
    {
        if (isFading || canvasGroup.alpha != 1.0f)
        {
            isFading = false;
            canvasGroup.alpha = 1.0f;
        }

        slider.value = percent;
    }

    public void SetRed()
    {
        sliderImage.color = Color.red;
    }
    public void ImageReset()
    {
        sliderImage.color = new Color(0,1,1);
    }

    public void StartFadeout()
    {
        isFading = true;
    }
}
