using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour
{
    public GameObject popupFrame;
    public bool isAsking;

    // Start is called before the first frame update
    void Start()
    {
        popupFrame.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public void HideAsk()
    {
        if (isAsking)
        {
            popupFrame.SetActive(false);
            isAsking = false;
        }
        
    }

    public void OnInteract()
    {
        if (isAsking)
        {
            StartCoroutine(SkipPhase());
        }
        else
        {
            switch (TavernManager.state)
            {
                case TavernState.OverworldDay:
                    AskIfSkip();
                    break;
                case TavernState.OverworldNight:
                    AskIfSkip();
                    break;
                default:
                    break;
            }
        }   
    }

    public void AskIfSkip()
    {
        popupFrame.SetActive(true);
        isAsking = true;
        Debug.Log("Ask");
    }

    public IEnumerator SkipPhase()
    {
        HideAsk();
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().enabled = false;
        GameObject interactionarea =  GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<InteractionSender>().gameObject;
        interactionarea.SetActive(false);

        // Change phases
        switch (TavernManager.state)
        {
            case TavernState.OverworldDay:
                yield return Fader.instance.FadeIn(FaderType.Endday);
                TavernManager.instance.EndDayWithoutService();
                break;
            case TavernState.OverworldNight:
                yield return Fader.instance.FadeIn(FaderType.Endnight);
                TavernManager.instance.EndNight();
                yield return Fader.instance.SetNightText();
                break;
            default:
                break;
        }


        // Fade out
        yield return new WaitForSeconds(2f);
        yield return Fader.instance.FadeOut();

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().enabled = true;
        interactionarea.SetActive(true);
    }

    public void FastForwardTime()
    {

    }
}
