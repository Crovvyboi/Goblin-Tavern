using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuItemStation : MonoBehaviour
{
    public static MenuItemStation instance;

    private PlayerControls playerControls;

    public bool isShowingMenu;
    public GameObject uiObject;

    [Header("Side panels")]
    public GameObject popupBackground;
    public GameObject ingredientPicker;
    public GameObject liquidPicker;
    public GameObject gemPicker;

    private void Start()
    {
        
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        playerControls = new PlayerControls();

        isShowingMenu = false;
        uiObject.SetActive(false);

        ingredientPicker.GetComponent<MenuItemStationSidebar>().SetPlayerControls(playerControls);
        gemPicker.GetComponent<MenuItemStationSidebar>().SetPlayerControls(playerControls);
        liquidPicker.GetComponent<MenuItemStationSidebar>().SetPlayerControls(playerControls);
    }

    public void OnEnable()
    {
        playerControls.Enable();

        playerControls.General.PlayerMenu.performed += ExitMenu;
    }

    public void OnDisable()
    {
        playerControls.Disable();
    }

    public void ExitMenu(InputAction.CallbackContext input)
    {
        if (input.performed && isShowingMenu)
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        if (TavernManager.state == TavernState.OverworldDay && !Fader.instance.isFading || TavernManager.state == TavernState.OverworldNight && !Fader.instance.isFading)
        {
            if (isShowingMenu)
            {
                CloseMenu();
            }
            else
            {
                ShowMenu();
            }
        }
    }

    public void ShowMenu()
    {
        uiObject.SetActive(true);

        // Disable other UI & Menus
        PlayerUIManager.instance.hotbar.SetActive(false);
        PlayerUIManager.instance.playerControls.Disable();

        // Disable movement & interact
        PlayerMovement.instance.enabled = false;
        InteractionSender.instance.enabled = false;

        // Reset Menu
        ResetMenu();

        isShowingMenu = true;
    }

    public void CloseMenu()
    {
        isShowingMenu = false;

        uiObject.SetActive(false);

        // Enable other UI & Menus
        PlayerUIManager.instance.hotbar.SetActive(true);
        PlayerUIManager.instance.playerControls.Enable();

        // Enable movement & interact
        PlayerMovement.instance.enabled = true;
        InteractionSender.instance.enabled = true;

    }

    public void ResetMenu()
    {
        // Close popups
        popupBackground.SetActive(false);
        ingredientPicker.SetActive(false);
        gemPicker.SetActive(false);
        liquidPicker.SetActive(false);


    }

    public void ToggleSideMenu(string menuName)
    {
        switch (menuName)
        {
            case "Ingredient":
                playerControls.General.PlayerMenu.performed -= ExitMenu;

                popupBackground.SetActive(true);
                ingredientPicker.SetActive(true);
                gemPicker.SetActive(false);
                liquidPicker.SetActive(false);
                break;
            case "Gem":
                playerControls.General.PlayerMenu.performed -= ExitMenu;

                popupBackground.SetActive(true);
                ingredientPicker.SetActive(false);
                gemPicker.SetActive(true);
                liquidPicker.SetActive(false);
                break;
            case "Liquid":
                playerControls.General.PlayerMenu.performed -= ExitMenu;

                popupBackground.SetActive(true);
                ingredientPicker.SetActive(false);
                gemPicker.SetActive(false);
                liquidPicker.SetActive(true);
                break;
            default:
                break;
        }
    }
    public void OnCloseSidebar()
    {
        playerControls.General.PlayerMenu.performed += ExitMenu;
    }
}
