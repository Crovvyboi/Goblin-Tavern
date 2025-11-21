using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    public static Fader instance;

    public CanvasGroup fadingImage;
    public bool isFading;

    [Header("Tooltip")]
    public GameObject tooltipHolder;

    [Header("EndDay")]
    public GameObject enddayHolder;
    public TextMeshProUGUI enddayText;

    [Header("EndNight")]
    public GameObject endnightHolder;
    public TextMeshProUGUI endnightText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject.transform.parent.gameObject);
    }

    private void Start()
    {
        tooltipHolder.SetActive(false);
        enddayHolder.SetActive(false);
        endnightHolder.SetActive(false);

        fadingImage.alpha = 0f;
        this.gameObject.SetActive(false);
    }

    public IEnumerator FadeIn(FaderType faderType)
    {
        isFading = true;
        switch (faderType)
        {
            case FaderType.Tooltip:
                SetTooltip();
                break;
            case FaderType.Endday:
                SetEnddayMessage();
                break;
            case FaderType.Endnight:
                SetEndnightMessage();
                break;
            default:
                break;
        }

        this.gameObject.SetActive(true);

        //Color color = fadingImage.color;
        float transparency = 0f;
        //fadingImage.color = new Color(color.r, color.g, color.b, transparency);

        while (fadingImage.alpha < 1f) 
        {
            transparency += 0.01f;
            fadingImage.alpha = transparency;
            yield return new WaitForSeconds(0.005f);
        }
    }

    public void SetDark()
    {
        isFading = true;

        fadingImage.alpha = 1f;
        this.gameObject.SetActive(true);
    }

    public IEnumerator FadeOut()
    {

        //Color color = fadingImage.color;
        float transparency = 1f;
        //fadingImage.color = new Color(color.r, color.g, color.b, transparency);

        while (fadingImage.alpha > 0f)
        {
            transparency -= 0.01f;
            fadingImage.alpha = transparency;
            yield return new WaitForSeconds(0.005f);
        }

        this.gameObject.SetActive(false);

        tooltipHolder.SetActive(false);
        enddayHolder.SetActive(false);
        endnightHolder.SetActive(false);

        isFading = false;
    }

    public void SetTooltip()
    {
        tooltipHolder.SetActive(true);

    }

    public void SetEnddayMessage()
    {
        enddayHolder.SetActive(true);

        if (TavernManager.state == TavernState.ServiceOverview)
        {
            enddayText.text = "Another hard day's work!";
        }
        else if(TavernManager.state == TavernState.OverworldDay)
        {
            enddayText.text = "You deserve a break too :)";
        }
    }

    public void SetEndnightMessage()
    {
        endnightHolder.SetActive(true);
    }
    public IEnumerator SetNightText()
    {
        yield return new WaitForSeconds(1);

        endnightText.text = $"Year {GlobalStats.instance.year} - {GlobalStats.instance.season.ToString()}\nDay {GlobalStats.instance.day}";
    }
}

public enum FaderType
{
    Generic,
    Tooltip,
    Endday,
    Endnight
}
