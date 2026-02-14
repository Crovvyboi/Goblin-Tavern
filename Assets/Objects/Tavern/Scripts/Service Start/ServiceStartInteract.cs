using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ServiceStartInteract : MonoBehaviour
{
    public static ServiceStartInteract instance;

    public GameObject serviceInteractContainer;

    [Header("Start")]
    public GameObject serviceStartUI;
    public TextMeshProUGUI serviceStartWarningText;

    [Header("Final call")]
    public GameObject serviceFinalCallUI;

    [Header("Stop")]
    public GameObject serviceStopUI;



    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        serviceInteractContainer.SetActive(false);
        serviceStartUI.SetActive(false);
        serviceFinalCallUI.SetActive(false);
        serviceStopUI.SetActive(false);
    }

    public void ClosePopup()
    {
        PlayerMovement.instance.canMove = true;
        InteractionSender.instance.canInteract = true;
        PlayerUIManager.instance.canInteract = true;

        serviceInteractContainer.SetActive(false);
        serviceStartUI.SetActive(false);
        serviceFinalCallUI.SetActive(false);
        serviceStopUI.SetActive(false);
    }

    #region Start
    public void ShowStartPopup()
    {
        serviceInteractContainer.SetActive(true);
        serviceStartUI.SetActive(true);
        serviceFinalCallUI.SetActive(false);
        serviceStopUI.SetActive(false);

        PlayerMovement.instance.canMove = false;
        InteractionSender.instance.canInteract = false;
        PlayerUIManager.instance.canInteract = false;
        

        // Find active objects with tag
        List<GameObject> barObjects = GameObject.FindGameObjectsWithTag("Bar").Where(x => x.activeInHierarchy == true).ToList();
        List<GameObject> drinkStationObjects = GameObject.FindGameObjectsWithTag("DrinkStation").Where(x => x.activeSelf == true).ToList();
        List<GameObject> mealStationObjects = GameObject.FindGameObjectsWithTag("MealStation").Where(x => x.activeSelf == true).ToList();

        List<MenuItem> drinksOnMenu = TavernManager.instance.menuItems.Where(x => x.menuItemType == MenuItemType.Drink && !x.standardInMenu && x.inMenu).ToList();
        List<MenuItem> mealsOnMenu = TavernManager.instance.menuItems.Where(x => x.menuItemType == MenuItemType.Meal && !x.standardInMenu && x.inMenu).ToList();

        List<GameObject> activeTables = GameObject.FindGameObjectsWithTag("Table").Where(x => x.GetComponent<Table>() && x.GetComponent<Table>().GetChairCount() > 0).ToList();

        // Make text
        bool needToWarn = false;
        string warningText = "But, there are some issues:";
        if (drinksOnMenu.Count > 0)
        {
            if (barObjects.Count == 0 && drinkStationObjects.Count == 0 && activeTables.Count == 0)
            {
                needToWarn = true;
                warningText += "\n - There are drinks on the menu, but no way to serve them.";
            }
            else
            {
                if (barObjects.Count == 0)
                {
                    needToWarn = true;
                    warningText += "\n - There are drinks on the menu, but no bar.";
                }

                if (drinkStationObjects.Count == 0 && activeTables.Count > 0)
                {
                    needToWarn = true;
                    warningText += "\n - There are drinks on the menu, but no drink station.";
                }
                else if (drinkStationObjects.Count != 0 && activeTables.Count == 0)
                {
                    needToWarn = true;
                    warningText += "\n - There are drinks on the menu, but no reachable tables.";
                }
            }
        }
        else
        {
            needToWarn = true;
            warningText += "\n - There are no drinks on the menu.";
        }

        if (mealsOnMenu.Count > 0)
        {
            if (mealStationObjects.Count == 0 && activeTables.Count == 0)
            {
                needToWarn = true;
                warningText += "\n - There are meals on the menu, but no reachable tables.";
            }
            else if (mealStationObjects.Count == 0 && activeTables.Count > 0)
            {
                needToWarn = true;
                warningText += "\n - There are meals on the menu, but no meal station.";
            }
            else if (mealStationObjects.Count != 0 && activeTables.Count == 0)
            {
                needToWarn = true;
                warningText += "\n - There are meals on the menu, but no reachable tables.";
            }
        }
        else
        {
            needToWarn = true;
            warningText += "\n - There are no meals on the menu.";
        }

        // Set text
        if (needToWarn)
        {
            serviceStartWarningText.gameObject.SetActive(true);
            serviceStartWarningText.text = warningText;
        }
        else
        {
            serviceStartWarningText.gameObject.SetActive(false);
        }
        

    }

    public void OnStartConfirm()
    {
        ClosePopup();

        ServiceManager.instance.StartService();
    }

    public void OnStartCancel()
    {
        ClosePopup();
    }
    #endregion

    #region Final Call
    public void ShowFinalPopup()
    {
        serviceInteractContainer.SetActive(true);
        serviceFinalCallUI.SetActive(true);
        serviceStartUI.SetActive(false);
        serviceStopUI.SetActive(false);

        PlayerMovement.instance.canMove = false;
        InteractionSender.instance.canInteract = false;
        PlayerUIManager.instance.canInteract = false;
    }


    public void OnFinalConfirm()
    {
        ClosePopup();

        if (PlayerServiceManager.instance.HasOpenOrders())
        {
            ServiceManager.instance.InitiateFinalCall();
        }
        else
        {
            ServiceManager.instance.InitiateFinalCall();
            ServiceManager.instance.ShowOverview();
        }
            
    }

    public void OnFinalCancel()
    {
        ClosePopup();
    }
    #endregion

    #region End
    public void ShowEndPopup()
    {
        serviceInteractContainer.SetActive(true);
        serviceFinalCallUI.SetActive(false);
        serviceStartUI.SetActive(false);
        serviceStopUI.SetActive(true);

        PlayerMovement.instance.canMove = false;
        InteractionSender.instance.canInteract = false;
        PlayerUIManager.instance.canInteract = false;
    }

    public void OnEndConfirm()
    {
        ClosePopup();

        ServiceManager.instance.ShowOverview();
    }
    #endregion
}
