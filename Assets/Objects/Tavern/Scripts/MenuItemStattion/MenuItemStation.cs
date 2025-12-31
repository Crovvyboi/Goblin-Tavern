using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class MenuItemStation : MonoBehaviour
{
    public static MenuItemStation instance;

    private PlayerControls playerControls;

    public bool isShowingMenu;
    public GameObject uiObject;

    [Header("Selected")]
    public int selectedIngredientSlot;
    public Dictionary<int, InventoryItem> selectedIngredients = new Dictionary<int, InventoryItem>();
    public MenuItem selectedMenuItemBase;
    public LiquidSetting selectedLiquid;
    public GemSetting selectedGem;
    public TempSetting selectedTemp;

    [Header("Side panels")]
    public GameObject popupBackground;
    public GameObject menuitembasePicker;
    public GameObject ingredientPicker;
    public GameObject liquidPicker;
    public GameObject gemPicker;

    [Header("Ingredients")]
    public List<GameObject> ingredientSlots = new List<GameObject>();
    public GameObject ingredientGrid;
    public GameObject ingredientGridPrefab;

    [Header("MenuItemBase")]
    public GameObject menuItemBaseSlot;
    public GameObject menuItemBaseGrid;
    public GameObject menuItemBaseGridPrefab;

    [Header("Liquid")]
    public GameObject liquidSlot;
    public GameObject liquidGrid;
    public List<GameObject> liquidGameObjectList = new List<GameObject>();

    [Header("Gem")]
    public GameObject gemSlot;
    public GameObject gemGrid;
    public List<GameObject> gemGameObjectList = new List<GameObject>();

    [Header("Temperature")]
    public GameObject temperatureSlider;

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

        menuitembasePicker.GetComponent<MenuItemStationSidebar>().SetPlayerControls(playerControls);
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
        menuitembasePicker.SetActive(false);
        ingredientPicker.SetActive(false);
        gemPicker.SetActive(false);
        liquidPicker.SetActive(false);

        // Ingredients
        selectedIngredients = new Dictionary<int, InventoryItem>();
        foreach (GameObject item in ingredientSlots)
        {
            item.GetComponent<RawImage>().texture = null;
            item.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }

        // Menu item base
        selectedMenuItemBase = null;
        menuItemBaseSlot.GetComponent<RawImage>().texture = null;
        menuItemBaseSlot.GetComponentInChildren<TextMeshProUGUI>().text = "";

        // Liquid
        selectedLiquid = LiquidSetting.None;
        liquidSlot.GetComponent<RawImage>().texture = null;
        liquidSlot.GetComponentInChildren<TextMeshProUGUI>().text = "";
        if (TavernMilestones.instance.CheckIfAnyLiquidIsUnlocked())
        {
            liquidSlot.GetComponent<Button>().interactable = true;
        }
        else
        {
            liquidSlot.GetComponent<Button>().interactable = false;
        }

        // Gem
        selectedGem = GemSetting.None;
        gemSlot.GetComponent<RawImage>().texture = null;
        gemSlot.GetComponentInChildren<TextMeshProUGUI>().text = "";
        if (TavernMilestones.instance.CheckIfAnyGemIsUnlocked())
        {
            gemSlot.GetComponent<Button>().interactable = true;
        }
        else
        {
            gemSlot.GetComponent<Button>().interactable = false;
        }

        // Temperature
        selectedTemp = TempSetting.None;
        temperatureSlider.GetComponent<Slider>().value = 1;
        temperatureSlider.GetComponentInChildren<TextMeshProUGUI>().text = TempSetting.None.ToString();
        if (!TavernMilestones.instance.negativeTempMilestone)
        {
            temperatureSlider.GetComponent<Slider>().minValue = 1;
        }
        else
        {
            temperatureSlider.GetComponent<Slider>().minValue = 0;
        }
        if (!TavernMilestones.instance.highTempMilestone)
        {
            temperatureSlider.GetComponent<Slider>().maxValue = 3;
        }
        else
        {
            temperatureSlider.GetComponent<Slider>().maxValue = 4;
        }
    }

    public void ToggleIngredientMenu(int index)
    {
        playerControls.General.PlayerMenu.performed -= ExitMenu;

        popupBackground.SetActive(true);
        menuitembasePicker.SetActive(false);
        ingredientPicker.SetActive(true);
        gemPicker.SetActive(false);
        liquidPicker.SetActive(false);

        ShowIngredients(index);
    }

    public void ToggleSideMenu(string menuName)
    {
        switch (menuName)
        {
            case "Gem":
                playerControls.General.PlayerMenu.performed -= ExitMenu;

                popupBackground.SetActive(true);
                menuitembasePicker.SetActive(false);
                ingredientPicker.SetActive(false);
                gemPicker.SetActive(true);
                liquidPicker.SetActive(false);

                ShowGems();
                break;
            case "Liquid":
                playerControls.General.PlayerMenu.performed -= ExitMenu;

                popupBackground.SetActive(true);
                menuitembasePicker.SetActive(false);
                ingredientPicker.SetActive(false);
                gemPicker.SetActive(false);
                liquidPicker.SetActive(true);

                ShowLiquids();
                break;
            case "MenuItem":
                playerControls.General.PlayerMenu.performed -= ExitMenu;

                popupBackground.SetActive(true);
                menuitembasePicker.SetActive(true);
                ingredientPicker.SetActive(false);
                gemPicker.SetActive(false);
                liquidPicker.SetActive(false);

                ShowMenuItemBases();
                break;
            default:
                break;
        }
    }

    #region Ingredients
    public void ShowIngredients(int index)
    {
        selectedIngredientSlot = index;

        // -- Inventory ------
        List<GameObject> entries = new List<GameObject>();
        List<GameObject> itemsInventory = new List<GameObject>();
        List<GameObject> containersInventory = new List<GameObject>();
        List<TavernStorageObject> storageInventory = TavernStorage.instance.tavernInventory.Where(x => x.item.type == InventoryItemType.Ingredient && !selectedIngredients.ContainsValue(x.item)).ToList();

        // Get all ingredients from inventory
        itemsInventory = PlayerInventory.instance.itemsInInventory.Where(x => x.GetComponent<InventoryItemHolder>().item.type == InventoryItemType.Ingredient && x.GetComponent<InventoryItemContainer>() == null && !selectedIngredients.ContainsValue(x.GetComponent<InventoryItemHolder>().item)).ToList();
        containersInventory = PlayerInventory.instance.itemsInInventory.Where(x => x.GetComponent<InventoryItemHolder>().item.type == InventoryItemType.Ingredient && x.GetComponent<InventoryItemContainer>() != null && !selectedIngredients.ContainsValue(x.GetComponent<InventoryItemHolder>().item)).ToList();

        // Convert items to storageobjects
        List<TavernStorageObject> itemsInventoryConverted = new List<TavernStorageObject>();
        foreach (TavernStorageObject item in storageInventory)
        {
            itemsInventoryConverted.Add(new TavernStorageObject(item.item, item.amount));
        }

        // for items
        foreach (GameObject item in itemsInventory)
        {
            if (itemsInventoryConverted.Any(x => x.item == item.GetComponent<InventoryItemHolder>().item))
            {
                itemsInventoryConverted.First(x => x.item == item.GetComponent<InventoryItemHolder>().item).IncreaseByOne();
            }
            else
            {
                itemsInventoryConverted.Add(new TavernStorageObject(item.GetComponent<InventoryItemHolder>().item));
            }
        }
        // for containers
        foreach (GameObject item in containersInventory)
        {
            foreach (InventoryItem inventoryItem in item.GetComponent<InventoryItemContainer>().itemsInContainer)
            {
                if (itemsInventoryConverted.Any(x => x.item == inventoryItem))
                {
                    itemsInventoryConverted.First(x => x.item == inventoryItem).IncreaseByOne();
                }
                else
                {
                    itemsInventoryConverted.Add(new TavernStorageObject(inventoryItem));
                }
            }
        }

        // Check for each existing entry if ingredient is still in inventory
        // If not, destroy entry
        // else increase amount of selector
        List<GameObject> markForDestroy = new List<GameObject>();
        foreach (Transform item in ingredientGrid.transform)
        {
            GameObject itemGO = item.gameObject;
            if (!itemsInventoryConverted.Any(x => x.item == itemGO.GetComponent<IngredientSelector>().ingredient))
            {
                markForDestroy.Add(itemGO);
            }
            else
            {
                itemGO.GetComponent<IngredientSelector>().amount = itemsInventoryConverted.First(x => x.item == itemGO.GetComponent<IngredientSelector>().ingredient).amount;
                itemsInventoryConverted.Remove(itemsInventoryConverted.First(x => x.item == itemGO.GetComponent<IngredientSelector>().ingredient));
                entries.Add(itemGO);
            }
        }
        if (markForDestroy.Count > 0)
        {
            for (int i = markForDestroy.Count - 1; 0 <= i; i--)
            {
                Destroy(markForDestroy[i]);
            }
        }

        // For each remaining item, make new entry
        foreach (TavernStorageObject item in itemsInventoryConverted)
        {
            GameObject newGameObject = GameObject.Instantiate(ingredientGridPrefab);
            newGameObject.transform.SetParent(ingredientGrid.transform, false);
            newGameObject.transform.SetAsLastSibling();

            newGameObject.GetComponent<IngredientSelector>().ingredient = item.item;
            newGameObject.GetComponent<IngredientSelector>().amount = item.amount;

            entries.Add(newGameObject);
        }

        // Finalize amounts
        foreach (GameObject item in entries)
        {
            item.GetComponent<IngredientSelector>().FinalizeSelector();
        }

        // Add none slot
        GameObject newGameObject2 = GameObject.Instantiate(ingredientGridPrefab);
        newGameObject2.transform.SetParent(ingredientGrid.transform, false);
        newGameObject2.transform.SetAsFirstSibling();

        newGameObject2.GetComponent<IngredientSelector>().ingredientName.text = "None";
        newGameObject2.GetComponent<IngredientSelector>().amountText.text = "";
        newGameObject2.GetComponent<IngredientSelector>().icon.gameObject.SetActive(false);

        entries.Add(newGameObject2);

    }

    public void SelectIngredient(InventoryItem ingredient)
    {
        if (ingredient == null)
        {
            selectedIngredients.Remove(selectedIngredientSlot);

            // update text & icon
            ingredientSlots[selectedIngredientSlot].GetComponent<RawImage>().texture = null;
            ingredientSlots[selectedIngredientSlot].GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
        else
        {
            if (selectedIngredients.ContainsKey(selectedIngredientSlot))
            {
                selectedIngredients.Remove(selectedIngredientSlot);
            }
            selectedIngredients.Add(selectedIngredientSlot, ingredient);

            // update text & icon
            ingredientSlots[selectedIngredientSlot].GetComponent<RawImage>().texture = ingredient.hotbarIcon;
            ingredientSlots[selectedIngredientSlot].GetComponentInChildren<TextMeshProUGUI>().text = ingredient.inventoryItemName;
        }


        popupBackground.SetActive(false);
        ingredientPicker.SetActive(false);

        selectedIngredientSlot = 0;

        CheckCombination();
    }
    #endregion

    #region MenuItemBase
    public void ShowMenuItemBases()
    {
        // Get known menuitems
        List<MenuItem> knownMenuItems = TavernManager.instance.menuItems.Where(x => x.recipeKnown && selectedMenuItemBase != x && !x.standardInMenu).ToList();

        // Get existing entries
        List<GameObject> entries = new List<GameObject>();
        List<GameObject> markForDestroy = new List<GameObject>();
        foreach (Transform item in menuItemBaseGrid.transform)
        {
            if (item.gameObject.GetComponent<MenuItemBaseSelector>().baseMenuItem == selectedMenuItemBase)
            {
                markForDestroy.Add(item.gameObject);
            }
            else
            {
                entries.Add(item.gameObject);
            }   
        }
        if (markForDestroy.Count > 0)
        {
            for (int i = markForDestroy.Count - 1; 0 <= i; i--)
            {
                Destroy(markForDestroy[i]);
            }
        }

        // If menuitem doesnt have an entry, make one
        foreach (MenuItem item in knownMenuItems)
        {
            if (!entries.Any(x => x.GetComponent<MenuItemBaseSelector>().baseMenuItem == item))
            {
                GameObject newGameObject = GameObject.Instantiate(menuItemBaseGridPrefab);
                newGameObject.transform.SetParent(menuItemBaseGrid.transform, false);
                newGameObject.transform.SetAsFirstSibling();

                newGameObject.GetComponent<MenuItemBaseSelector>().baseMenuItem = item;
                if (item.icon != null)
                {
                    newGameObject.GetComponent<MenuItemBaseSelector>().icon.texture = item.icon;
                }
                newGameObject.GetComponent<MenuItemBaseSelector>().menuItemName.text = item.itemName;

                entries.Add(newGameObject);
            }
        }

        // Add none slot
        if (!entries.Any(x => x.GetComponent<MenuItemBaseSelector>().baseMenuItem == null))
        {
            GameObject newGameObject2 = GameObject.Instantiate(menuItemBaseGridPrefab);
            newGameObject2.transform.SetParent(menuItemBaseGrid.transform, false);
            newGameObject2.transform.SetAsFirstSibling();

            newGameObject2.GetComponent<MenuItemBaseSelector>().icon.gameObject.SetActive(false);
            newGameObject2.GetComponent<MenuItemBaseSelector>().menuItemName.text = "None";

            entries.Add(newGameObject2);
        }
        else
        {
            entries.First(x => x.GetComponent<MenuItemBaseSelector>().baseMenuItem == null).transform.SetAsFirstSibling();
        }

    }

    public void SelectMenuItemBase(MenuItem menuItem)
    {
        if (menuItem == null)
        {
            selectedMenuItemBase = null;
            menuItemBaseSlot.GetComponent<RawImage>().texture = null;
            menuItemBaseSlot.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
        else
        {
            selectedMenuItemBase = menuItem;
            menuItemBaseSlot.GetComponent<RawImage>().texture = menuItem.icon;
            menuItemBaseSlot.GetComponentInChildren<TextMeshProUGUI>().text = menuItem.itemName;
        }

        popupBackground.SetActive(false);
        menuitembasePicker.SetActive(false);

        CheckCombination();
    }


    #endregion

    #region Liquid
    public void ShowLiquids()
    {
        // Get all entries
        liquidGameObjectList = new List<GameObject>();
        foreach (Transform item in liquidGrid.transform)
        {
            liquidGameObjectList.Add(item.gameObject);

            // Check if there's a selected liquid
            // if yes, disable selected liquid entry and enable others
            // if no, disable none option and enable all
            if (selectedLiquid == item.gameObject.GetComponent<LiquidSelector>().liquidName && item.gameObject.GetComponent<LiquidSelector>().liquidName == LiquidSetting.None)
            {
                item.gameObject.SetActive(true);
            }
            else if (selectedLiquid == item.gameObject.GetComponent<LiquidSelector>().liquidName || !TavernMilestones.instance.CheckIfLiquidMilestoneIsAchieved(item.gameObject.GetComponent<LiquidSelector>().liquidName))
            {
                item.gameObject.SetActive(false);
            }
            else
            {
                item.gameObject.SetActive(true);
            }
        }

        
    }

    public void SelectLiquid(int liquid)
    {
        if ((LiquidSetting)liquid != LiquidSetting.None)
        {
            LiquidSelector liquidObject = liquidGameObjectList.First(x => x.GetComponent<LiquidSelector>().liquidName == (LiquidSetting)liquid).GetComponent<LiquidSelector>();

            liquidSlot.GetComponent<RawImage>().texture = liquidObject.icon;
            liquidSlot.GetComponentInChildren<TextMeshProUGUI>().text = liquidObject.liquidName.ToString();
            
            selectedLiquid = (LiquidSetting)liquid;
        }
        else
        {
            liquidSlot.GetComponent<RawImage>().texture = null;
            liquidSlot.GetComponentInChildren<TextMeshProUGUI>().text = "";

            selectedLiquid = LiquidSetting.None;
        }


        popupBackground.SetActive(false);
        liquidPicker.SetActive(false);


        CheckCombination();
    }
    #endregion

    #region Gem
    public void ShowGems()
    {
        gemGameObjectList = new List<GameObject>();
        foreach (Transform item in gemGrid.transform)
        {
            gemGameObjectList.Add(item.gameObject);

            if (selectedGem == item.gameObject.GetComponent<GemSelector>().gemName && item.gameObject.GetComponent<GemSelector>().gemName == GemSetting.None)
            {
                item.gameObject.SetActive(true);
            }
            else if (selectedGem == item.gameObject.GetComponent<GemSelector>().gemName || !TavernMilestones.instance.CheckIfGemMilestoneIsAchieved(item.gameObject.GetComponent<GemSelector>().gemName))
            {
                item.gameObject.SetActive(false);
            }
            else
            {
                item.gameObject.SetActive(true);
            }
        }
    }

    public void SelectGem(int gem)
    {
        if ((GemSetting)gem != GemSetting.None)
        {
            GemSelector gemObject = gemGameObjectList.First(x => x.GetComponent<GemSelector>().gemName == (GemSetting)gem).GetComponent<GemSelector>();

            gemSlot.GetComponent<RawImage>().texture = gemObject.icon;
            gemSlot.GetComponentInChildren<TextMeshProUGUI>().text = gemObject.gemName.ToString();

            selectedGem = (GemSetting)gem;
        }
        else
        {
            gemSlot.GetComponent<RawImage>().texture = null;
            gemSlot.GetComponentInChildren<TextMeshProUGUI>().text = "";

            selectedGem = GemSetting.None;
        }

        popupBackground.SetActive(false);
        gemPicker.SetActive(false);

        CheckCombination();
    }
    #endregion

    #region Temperature
    public void SetTemperature()
    {
        TempSetting tempsetting = (TempSetting)temperatureSlider.GetComponent<Slider>().value;

        selectedTemp = tempsetting;
        temperatureSlider.GetComponentInChildren<TextMeshProUGUI>().text = tempsetting.ToString();


        CheckCombination();
    }
    #endregion

    public void OnCloseSidebar()
    {
        playerControls.General.PlayerMenu.performed += ExitMenu;
    }

    public void CheckCombination()
    {
        List<InventoryItem> ingredients = new List<InventoryItem>();
        ingredients.AddRange(selectedIngredients.Values.ToList());

        List<Recipe> combinationValid = TavernManager.instance.recipes.Where(x =>
            x.menuItemBase == selectedMenuItemBase &&
            x.recipeLiquid == selectedLiquid &&
            x.recipeGem == selectedGem &&
            x.recipeTemp == selectedTemp &&
            x.CheckIngredients(ingredients)
        ).ToList();
        Debug.Log(combinationValid);


        // Show result if combination is valid && discovered
        if (combinationValid.Count == 1)
        {
            if (combinationValid[0].resultMenuItem != null && combinationValid[0].resultMenuItem.recipeKnown)
            {
                
            }
            else if (combinationValid[0].resultIngredient != null && combinationValid[0].resultIngredient.ingredientKnown)
            {

            }
        }
    }

    public void MakeCombination()
    {
        List<InventoryItem> ingredients = new List<InventoryItem>();
        ingredients.AddRange(selectedIngredients.Values.ToList());

        List<Recipe> combinationValid = TavernManager.instance.recipes.Where(x =>
            x.menuItemBase == selectedMenuItemBase &&
            x.recipeLiquid == selectedLiquid &&
            x.recipeGem == selectedGem &&
            x.recipeTemp == selectedTemp &&
            x.CheckIngredients(ingredients)
        ).ToList();

        if (combinationValid.Count == 0)
        {
            Debug.Log("No recpies found. Grant Gruel");

            ConsumeItems(ingredients);
            FailRecipe();
        }
        else if (combinationValid.Count == 1)
        {
            Debug.Log($"Recipe found: {combinationValid[0].name}");

            Recipe recipe = combinationValid[0];

            if (recipe.resultMenuItem != null)
            {
                if (!recipe.resultMenuItem.recipeKnown)
                {
                    ConsumeItems(ingredients);
                    GrantRewards(recipe);

                    recipe.resultMenuItem.recipeKnown = true;

                    recipe.resultMenuItem.inMenu = false;
                    recipe.resultMenuItem.isNew = true;
                    if (KnownRecipesUI.instance != null)
                    {
                        KnownRecipesUI.instance.AddMenuItemToGrid(recipe.resultMenuItem);
                    }

                    DiscoveryPopup(recipe);
                }
            }
            else if (recipe.resultIngredient != null) 
            {
                ConsumeItems(ingredients);
                GrantRewards(recipe);

                if (!recipe.resultIngredient.ingredientKnown)
                {
                    recipe.resultIngredient.ingredientKnown = true;
                    DiscoveryPopup(recipe);
                }
            }
        }
        else
        {
            Debug.Log("Multiple recipes detected");
        }

        
    }

    public void FailRecipe()
    {

    }
    
    public void ConsumeItems(List<InventoryItem> ingredients)
    {
        foreach (InventoryItem item in ingredients)
        {

            // Check inventory first, then storage
            if (item.containerItem && PlayerInventory.instance.itemsInInventory.Any(x =>
                    x.GetComponent<InventoryItemContainer>().itemsInContainer.Contains(item)
                ) ||
                !item.containerItem && PlayerInventory.instance.itemsInInventory.Any(x => 
                    x.GetComponent<InventoryItemHolder>().item == item                    
                )
            )
            {
                // Take from inventory
                PlayerInventory.instance.TakeItem(item);
            }
            else
            {
                // Take from storage
                TavernStorage.instance.TakeItem(item);
            }
        }
    }

    public void GrantRewards(Recipe recipe)
    {
        
    }

    public void DiscoveryPopup(Recipe recipe)
    {
        


    }
}
