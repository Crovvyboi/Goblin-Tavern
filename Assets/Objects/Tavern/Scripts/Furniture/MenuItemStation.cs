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
}
