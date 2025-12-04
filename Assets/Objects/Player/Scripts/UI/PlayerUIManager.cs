using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance;

    public PlayerControls playerControls;
    public PlayerServiceUI _serviceUIManager;

    public bool canInteract;

    [Header("PauseMenu")]
    public GameObject pausemenuObject;
    public bool pausePressed;
    public static bool showingPause;
    public GameObject closeGameWarning;

    [Header("PlayerMenu")]
    public static bool showingPlayerMenu;
    public MenuState menuState;

    public GameObject playermenuObject;
    public GameObject tavernSummaryObject;
    public GameObject inventoryObject;
    public GameObject hotbar;
    public GameObject knownRecipesObject;
    public GameObject currentMenuObject;


    void Start()
    {

    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        playerControls = new PlayerControls();

        playermenuObject.SetActive(true);
        OpenInventoryMenu();
        OpenTavernSummaryMenu();
        playermenuObject.SetActive(false);
        hotbar.SetActive(true);

        showingPause = false;
        canInteract = true;


        StartCoroutine(Fader.instance.FadeOut());
    }

    private void OnEnable()
    {
        playerControls.Enable();

        playerControls.General.Pause.started += PauseGame;
        playerControls.General.PlayerMenu.started += PlayerMenu;
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    #region Pausemenu
    void PauseGame(InputAction.CallbackContext input)
    {

        if (input.started && !Fader.instance.isFading)
        {
            Debug.Log("pressed");
            if (!showingPause)
            {
                showingPause = true;
                ShowPauseMenu();
            }
            else
            {
                ResumeGame();
            }
        }

    }

    public void ResumeGame()
    {
        showingPause = false;
        HidePauseMenu();
    }

    public void ShowPauseMenu()
    {
        Debug.Log("Showing");

        Time.timeScale = 0;

        PlayerMovement.instance.enabled = false;
        InteractionSender.instance.enabled = false;

        pausemenuObject.SetActive(true);
    }

    public void HidePauseMenu()
    {
        Debug.Log("Hiding");

        pausemenuObject.SetActive(false);

        PlayerMovement.instance.enabled = true;
        InteractionSender.instance.enabled = true;

        Time.timeScale = 1;
    }

    public void QuitToMainMenu()
    {

    }

    public void OpenCloseGameWarning()
    {
        canInteract = false;

        closeGameWarning.SetActive(true);
    }

    public void CloseCloseGameWarning()
    {
        closeGameWarning.SetActive(false);

        canInteract = true;
    }

    public void ConfirmCloseGame()
    {
    #if UNITY_EDITOR
            Fader.instance.FadeOut();
            UnityEditor.EditorApplication.isPlaying = false;
    #else
                Application.Quit();
    #endif
    }
    #endregion

    #region Playermenu
    public void StartService()
    {
        hotbar.SetActive(false);
    }

    public void StopService()
    {
        hotbar.SetActive(true);
    }


    public void PlayerMenu(InputAction.CallbackContext input)
    {
        if (input.started && canInteract && !Fader.instance.isFading && !MenuItemStation.instance.isShowingMenu)
        {
            if (showingPlayerMenu)
            {
                showingPlayerMenu = false;
                playermenuObject.SetActive(false);

                PlayerMovement.instance.enabled = true;
                InteractionSender.instance.enabled = true;

                Hotbar.instance.SelectSlot();
                if (TavernManager.state == TavernState.OverworldNight || TavernManager.state == TavernState.OverworldDay)
                {
                    hotbar.SetActive(true);
                }
                else
                {
                    hotbar.SetActive(false);
                }
            }
            else
            {
                showingPlayerMenu = true;
                playermenuObject.SetActive(true);

                PlayerMovement.instance.enabled = false;
                InteractionSender.instance.enabled = false;

                Hotbar.instance.DeselectSlot();
                switch (menuState)
                {
                    case MenuState.TavernSummary:
                        OpenTavernSummaryMenu();
                        break;
                    case MenuState.Inventory:
                        OpenInventoryMenu();
                        break;
                    case MenuState.Recipes:
                        OpenRecipeMenu();
                        break;
                    case MenuState.MenuSettings:
                        OpenMenuMenu();
                        break;
                    default:
                        break;
                }
            }
        }
    }
    
    public void OpenTavernSummaryMenu()
    {
        menuState = MenuState.TavernSummary;

        tavernSummaryObject.SetActive(true);

        inventoryObject.SetActive(false);
        hotbar.SetActive(false);
        currentMenuObject.SetActive(false);
        knownRecipesObject.SetActive(false);
    }
    public void OpenInventoryMenu()
    {
        menuState = MenuState.Inventory;

        inventoryObject.SetActive(true);
        hotbar.SetActive(true);

        tavernSummaryObject.SetActive(false);
        currentMenuObject.SetActive(false);
        knownRecipesObject.SetActive(false);
    }

    public void OpenRecipeMenu()
    {
        menuState = MenuState.Recipes;

        currentMenuObject.SetActive(false);

        inventoryObject.SetActive(false);
        hotbar.SetActive(false);
        tavernSummaryObject.SetActive(false);
        knownRecipesObject.SetActive(true);

        if (KnownRecipesUI.instance != null)
        {
            KnownRecipesUI.instance.UpdateAddRemoveButton();
        }
    }

    public void OpenMenuMenu()
    {
        menuState = MenuState.MenuSettings;

        knownRecipesObject.SetActive(false);

        inventoryObject.SetActive(false);
        hotbar.SetActive(false);
        tavernSummaryObject.SetActive(false);
        currentMenuObject.SetActive(true);

        if (CurrentMenuUI.instance != null)
        {
            CurrentMenuUI.instance.UpdateMenuList();
        }
    }

    #endregion

}

public enum MenuState
{
    TavernSummary,
    Inventory,
    Recipes,
    MenuSettings
}
