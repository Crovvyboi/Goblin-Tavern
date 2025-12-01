using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ServiceManager : MonoBehaviour
{
    // Servicemanagers handles everything to do with the service, such as: starting/stopping service, keeping track of stats for that service
    public static ServiceManager instance;

    public ServiceStats stats;

    public static float serviceTimer;
    private float serviceTimerStart = 30f;

    public int goldMadeInService = 0;
    public int customersServed = 0;

    public List<List<CustomerStats>> customerGroupPool;
    public List<GameObject> generatedCustomers;

    public List<MenuItem> definitiveMenu;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void OnInteract()
    {
        switch (TavernManager.state)
        {
            case TavernState.OverworldDay:
                StartService();
                break;
            case TavernState.Service:
                InitiateFinalCall();
                break;
            case TavernState.ServiceFinalCall:
                // Warn player for reputation penalty if they continue

                ShowOverview();
                break;
            default:
                break;
        }

    }
    private void FixedUpdate()
    {
        switch (TavernManager.state)
        {
            case TavernState.Service:
                Timer();
                break;
            case TavernState.ServiceFinalCall:
                CheckIfFinalCallIsFinished();
                break;
            default:
                break;
        }
    }

    public void StartService()
    {
        // If tavern contains either a meal & drink station or a bar
        if (GameObject.FindGameObjectsWithTag("MealStation").Length > 0 && GameObject.FindGameObjectsWithTag("DrinkStation").Length > 0 || GameObject.FindGameObjectsWithTag("Bar").Length > 0)
        {
            serviceTimer = serviceTimerStart;

            // Lock in set menu
            definitiveMenu = TavernManager.instance.tavernMenu;
            definitiveMenu.AddRange(TavernManager.instance.menuItems.Where(x => x.standardInMenu));

            // Generate customer pool
            generatedCustomers = new List<GameObject>();
            customerGroupPool = new List<List<CustomerStats>>();
            customerGroupPool = CustomerGenerator.instance.GenerateAllCustomers();

            // Reset stats
            stats = new ServiceStats();

            // Close door
            TavernManager.instance.tavernDoor.SetActive(false);

            // Activate playerservicemanager
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerServiceManager>().enabled = true;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerServiceManager>().OnServiceStart();

            // Switch tavern state
            TavernManager.state = TavernState.Service;

            // Switch UI elements
            PlayerUIManager.instance.StartService();
            PlayerServiceUI.instance.StartService();
        }
        else
        {
            // Warn player



        }
        
    }

    public void InitiateFinalCall()
    {
        // Switch tavern state
        TavernManager.state = TavernState.ServiceFinalCall;

        // Show final call in UI
        PlayerServiceUI.instance.InitiateFinalCall();

        // Send all customers without order to exit (on delay)
        foreach (GameObject customer in generatedCustomers)
        {
            if (customer != null)
            {
                CustomerBase customerBase = customer.GetComponent<CustomerBase>();
                GameObject tableobject = customerBase.customerStats.assignedTable;
                if (tableobject != null)
                {
                    if (!PlayerServiceManager.instance.takenOrders.ContainsKey(tableobject.GetComponent<Table>()))
                    {
                        customerBase.OnFinalCall();

                        tableobject.GetComponent<Table>().OnFinalCall();
                    }                    
                }
                else
                {
                    // Check if customer has placed order at bar
                    if (customer.GetComponent<CustomerBehaviourB>().currentOrder.Count == 0)
                    {
                        customerBase.OnFinalCall();
                    }
                }
            }
        }
    }

    public void CheckIfFinalCallIsFinished()
    {
        // Check if any bar orders are open
        if (PlayerServiceManager.instance.takenOrders.Count == 0 && Bar.instance.barOrders.Count == 0)
        {
            ShowOverview();
        }
    }

    public void ShowOverview()
    {
        // Lock player movement & interactions
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().enabled = false;

        // Switch tavern state
        TavernManager.state = TavernState.ServiceOverview;

        // Add stats to overall stats
        stats.AddToGlobalStats();

        // Show service overview in UI
        PlayerServiceUI.instance.ShowServiceOverview();

        Debug.Log(TavernManager.state);
    }

    public void StopServiceHook()
    {
        StartCoroutine(StopService());
    }
    public IEnumerator StopService()
    {
        yield return Fader.instance.FadeIn(FaderType.Endday);

        TavernManager.instance.EndDay();

        foreach (GameObject item in generatedCustomers)
        {
            Destroy(item);
        }

        PlayerUIManager.instance.StopService();
        PlayerServiceUI.instance.StopService();

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerServiceManager>().enabled = false;

        yield return new WaitForSeconds(2f);
        yield return Fader.instance.FadeOut();

        TavernManager.instance.tavernDoor.SetActive(true);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().enabled = true;

        Debug.Log("Stopping service");
    }

    public void Timer()
    {
        serviceTimer -= Time.deltaTime;
        if (serviceTimer <= 0f)
        {
            // Initiate final call
            InitiateFinalCall();
        }
    }
}
