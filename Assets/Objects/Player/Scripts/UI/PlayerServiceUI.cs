using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerServiceUI : MonoBehaviour
{
    public static PlayerServiceUI instance;

    public GameObject serviceUIObject;
    public GameObject timerObject;

    public GameObject serviceOverviewObject;
    public ServiceOverviewUI serviceOverviewUI;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        serviceUIObject.SetActive(false);
        serviceOverviewObject.SetActive(false);

    }
    private void FixedUpdate()
    {
        switch (TavernManager.state)
        {
            case TavernState.Service:
                UpdateTimer();
                break;
            default:
                break;
        }
    }

    public void StartService()
    {
        serviceUIObject.gameObject.SetActive(true);
    }

    public void InitiateFinalCall()
    {
        timerObject.GetComponentInChildren<TextMeshProUGUI>().text = "Final call!";
    }

    public void ShowServiceOverview()
    {
        serviceUIObject.SetActive(false);
        serviceOverviewObject.SetActive(true);

        serviceOverviewUI.ShowOverviewUI();
    }

    public void StopService()
    {
        serviceOverviewObject.SetActive(false);
    }


    public void UpdateTimer()
    {
        float servicetimer = ServiceManager.serviceTimer;

        if (servicetimer > 0)
        {
            TimeSpan time = TimeSpan.FromSeconds(servicetimer);
            timerObject.GetComponentInChildren<TextMeshProUGUI>().text = time.ToString(@"mm\:ss");
        }
    }
}
