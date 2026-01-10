using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class KnownRecipesUI : MonoBehaviour
{
    public static KnownRecipesUI instance;
    public MenuItem selectedMenuItem;
    public InventoryItem selectedIngredient;

    [Header("Recipe overview")]
    public GameObject recipeOverviewObject;
    public GameObject contentParent;
    public GameObject gridItemPrefab;
    public List<GameObject> gridItems = new List<GameObject>();

    [Header("Recipe details")]
    public GameObject recipeDetailsObject;
    public GameObject menuItemDetails;
    public GameObject menuItemLikeDislike;
    public GameObject menuItemIngredients;
    public GameObject ingredientDetails;
    public GameObject ingredientIngredients;

    [Header("Known menuitem")]
    public GameObject knownMenuItemObject;
    public GameObject knownMenuItemDescriptionText;
    public GameObject knownMenuItemStarRatingObject;
    public GameObject knownMenuItemGoldText;
    public GameObject knownMenuItemLikedSpecies;
    public GameObject knownMenuItemDislikedSpecies;
    public GameObject knownMenuItemLikedClass;
    public GameObject knownMenuItemDislikedClass;
    public GameObject knownMenuItemIngredients;
    public GameObject addRemoveMenuButton;

    [Header("Known ingredient")]
    public GameObject knownIngredientObject;
    public GameObject knownIngredientDescriptionText;
    public GameObject knownIngredientCostText;
    #region Ingredients
    public GameObject knownIngredientBase;
    public GameObject knownIngredientLiquid;
    public GameObject knownIngredientGem;
    public GameObject knownIngredientTemp;
    public GameObject knownIngredientIngredients;
    #endregion

    [Header("Known general")]
    public GameObject knownRecipeTitleText;
    public GameObject favoritedButton;
    public GameObject knownRecipeImage;

    [Header("Unknown recipe")]
    public GameObject unknownRecipeName;
    public GameObject unknownRecipeObject;
    public GameObject favoritedRecipeButton;
    public GameObject unkownRecipeHints;
    public GameObject missingHintsText;

    [Header("Filter")]
    public GameObject filterObject;
    public TMP_InputField searchField;
    public Toggle isFavoredToggle;
    public Toggle isOnMenuToggle;
    public Toggle isUndiscoveredToggle;

    public GameObject filterScrollPrefab;
    public GameObject speciesFilter;
    public List<GameObject> speciesObjectList = new List<GameObject>();
    public GameObject classFilter;
    public List<GameObject> classObjectList = new List<GameObject>();

    public Toggle hideUndiscoveredToggle;

    


    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        // Load in recipes
        List<Recipe> items = TavernManager.instance.recipes.Where(x => x.resultMenuItem && x.resultMenuItem.recipeKnown || x.resultIngredient && x.resultIngredient.type == InventoryItemType.Ingredient && x.resultIngredient.recipeKnown || x.CheckHints()).ToList();

        // Make tile in grid for each recipe
        foreach (Recipe item in items)
        {
           AddToGrid(item);

        }


        // Select first known recipe
        if (items[0].resultMenuItem)
        {
            selectedIngredient = null;
            selectedMenuItem = items[0].resultMenuItem;
        }
        else if (items[0].resultIngredient)
        {
            selectedMenuItem = null;
            selectedIngredient = items[0].resultIngredient;
        }
        OnShow(items[0]);


        // Add known species & classes to filter
        foreach (Species filterSpecies in Enum.GetValues(typeof(Species)))
        {
            GameObject newObject = GameObject.Instantiate(filterScrollPrefab);
            speciesObjectList.Add(newObject);
            newObject.transform.SetParent(speciesFilter.transform, false);

            Texture2D icon = IconHandler.instance.GetIconOnSpecies(filterSpecies);
            Rect rect = new Rect(0,0, icon.width, icon.height);
            newObject.GetComponent<Image>().sprite = Sprite.Create(icon, rect, new Vector2(0,0), 512);

            FilterScrollButton filterScrollButton = newObject.GetComponent<FilterScrollButton>();
            filterScrollButton.filterType = FilterScrollType.Species;
            filterScrollButton.filterSpecies = filterSpecies;

            newObject.GetComponent<Button>().onClick.AddListener(() => filterScrollButton.ChangeButtonStateKnownRecipes());
        }

        foreach (Class filterClass in Enum.GetValues(typeof(Class)))
        {
            GameObject newObject = GameObject.Instantiate(filterScrollPrefab);
            classObjectList.Add(newObject);
            newObject.transform.SetParent(classFilter.transform, false);

            Texture2D icon = IconHandler.instance.GetIconOnClass(filterClass);
            Rect rect = new Rect(0, 0, icon.width, icon.height);
            newObject.GetComponent<Image>().sprite = Sprite.Create(icon, rect, new Vector2(0, 0), 512);

            FilterScrollButton filterScrollButton = newObject.GetComponent<FilterScrollButton>();
            filterScrollButton.filterType = FilterScrollType.Class;
            filterScrollButton.filterClass = filterClass;

            newObject.GetComponent<Button>().onClick.AddListener(() => filterScrollButton.ChangeButtonStateKnownRecipes());
        }

        // Reset filters
        ResetFilters();
    }

    public void AddToGrid(Recipe item)
    {
        // Check if griditem of recipe already exists
        if (gridItems.Any(x => x.GetComponent<GridItem>().menuItem && item.resultMenuItem == x.GetComponent<GridItem>().menuItem || x.GetComponent<GridItem>().inventoryItem && item.resultIngredient == x.GetComponent<GridItem>().inventoryItem))
        {
            // Adjust griditem to revealed recipe
            GameObject gridItem = gridItems.First(x => x.GetComponent<GridItem>().menuItem && item.resultMenuItem == x.GetComponent<GridItem>().menuItem || x.GetComponent<GridItem>().inventoryItem && item.resultIngredient == x.GetComponent<GridItem>().inventoryItem);

            if (item.resultMenuItem && item.resultMenuItem.isNew || item.resultIngredient && item.resultIngredient.isNew)
            {
                gridItem.GetComponent<Outline>().enabled = true;
            }
            else
            {
                gridItem.GetComponent<Outline>().enabled = false;
            }

            if (item.resultMenuItem)
            {
                gridItem.transform.GetChild(0).GetComponent<RawImage>().texture = item.resultMenuItem.icon;
                gridItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.resultMenuItem.gridTitle;

                if (item.resultMenuItem.inMenu || item.resultMenuItem.standardInMenu)
                {
                    gridItem.transform.GetChild(2).gameObject.SetActive(true);
                }
                else
                {
                    gridItem.transform.GetChild(2).gameObject.SetActive(false);
                }
                if (item.resultMenuItem.isFavorited)
                {
                    gridItem.transform.GetChild(3).gameObject.SetActive(true);
                }
                else
                {
                    gridItem.transform.GetChild(3).gameObject.SetActive(false);
                }

                if (selectedMenuItem == item.resultMenuItem)
                {
                    OnShow(item);
                }
            }
            else if (item.resultIngredient)
            {
                gridItem.transform.GetChild(0).GetComponent<RawImage>().texture = item.resultIngredient.hotbarIcon;
                gridItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.resultIngredient.gridName;

                gridItem.transform.GetChild(2).gameObject.SetActive(false);
                if (item.resultIngredient.isFavorited)
                {
                    gridItem.transform.GetChild(3).gameObject.SetActive(true);
                }
                else
                {
                    gridItem.transform.GetChild(3).gameObject.SetActive(false);
                }

                if (selectedIngredient == item.resultIngredient)
                {
                    OnShow(item);
                }
            }

            gridItem.GetComponent<CanvasGroup>().alpha = 1f;
        }
        else
        {
            GameObject newGridItem = GameObject.Instantiate(gridItemPrefab);
            gridItems.Add(newGridItem);
            newGridItem.transform.SetParent(contentParent.transform, false);

            if (item.resultMenuItem)
            {
                newGridItem.GetComponent<GridItem>().menuItem = item.resultMenuItem;
            }
            else if (item.resultIngredient)
            {
                newGridItem.GetComponent<GridItem>().inventoryItem = item.resultIngredient;
            }

            newGridItem.GetComponent<Button>().onClick.AddListener(() => OnShow(item));

            if (item.resultMenuItem && item.resultMenuItem.isNew || item.resultIngredient && item.resultIngredient.isNew)
            {
                newGridItem.GetComponent<Outline>().enabled = true;
            }
            else
            {
                newGridItem.GetComponent<Outline>().enabled = false;
            }

            if (item.resultMenuItem && !item.resultMenuItem.recipeKnown || item.resultIngredient && !item.resultIngredient.recipeKnown)
            {
                // If player does not know recipe, set as unkown tile & decrease opacity
                newGridItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "? ? ?";
            
                newGridItem.GetComponent<CanvasGroup>().alpha = 0.5f;

                newGridItem.transform.GetChild(2).gameObject.SetActive(false);

                if (item.resultMenuItem && item.resultMenuItem.isFavorited || item.resultIngredient && item.resultIngredient.isFavorited)
                {
                    newGridItem.transform.GetChild(3).gameObject.SetActive(true);
                }
                else
                {
                    newGridItem.transform.GetChild(3).gameObject.SetActive(false);
                }
            }
            else
            {
                // Set icon & title
                if (item.resultMenuItem)
                {
                    newGridItem.transform.GetChild(0).GetComponent<RawImage>().texture = item.resultMenuItem.icon;
                    newGridItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.resultMenuItem.gridTitle;

                    if (item.resultMenuItem.inMenu || item.resultMenuItem.standardInMenu)
                    {
                        newGridItem.transform.GetChild(2).gameObject.SetActive(true);
                    }
                    else
                    {
                        newGridItem.transform.GetChild(2).gameObject.SetActive(false);
                    }
                    if (item.resultMenuItem.isFavorited)
                    {
                        newGridItem.transform.GetChild(3).gameObject.SetActive(true);
                    }
                    else
                    {
                        newGridItem.transform.GetChild(3).gameObject.SetActive(false);
                    }
                }
                else if (item.resultIngredient)
                {
                    newGridItem.transform.GetChild(0).GetComponent<RawImage>().texture = item.resultIngredient.hotbarIcon;
                    newGridItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.resultIngredient.gridName;

                    newGridItem.transform.GetChild(2).gameObject.SetActive(false);
                    if (item.resultIngredient.isFavorited)
                    {
                        newGridItem.transform.GetChild(3).gameObject.SetActive(true);
                    }
                    else
                    {
                        newGridItem.transform.GetChild(3).gameObject.SetActive(false);
                    }
                }
            
                newGridItem.GetComponent<CanvasGroup>().alpha = 1f;
            }
        }
    }

    public void SwitchToDetails()
    {
        if (selectedMenuItem != null)
        {
            menuItemLikeDislike.SetActive(false);
            menuItemDetails.SetActive(true);
            menuItemIngredients.SetActive(false);
        }
        else if (selectedIngredient != null)
        {
            ingredientDetails.SetActive(true);
            ingredientIngredients.SetActive(false);
        }
        
    }
    public void SwitchToLikedDisliked()
    {
        menuItemDetails.SetActive(false);
        menuItemLikeDislike.SetActive(true);
    }

    public void SwitchToIngredients()
    {
        if (selectedMenuItem != null)
        {
            menuItemLikeDislike.SetActive(false);
            menuItemDetails.SetActive(false);
            menuItemIngredients.SetActive(true);
        }
        else if (selectedIngredient != null)
        {
            ingredientDetails.SetActive(false);
            ingredientIngredients.SetActive(true);
        }
    }

    public void UpdateNew(Recipe item)
    {
        if (item.resultMenuItem)
        {
            gridItems.First(x => x.GetComponent<GridItem>().menuItem == item.resultMenuItem).GetComponent<Outline>().enabled = false;
        }
        else if (item.resultIngredient)
        {
            gridItems.First(x => x.GetComponent<GridItem>().inventoryItem == item.resultIngredient).GetComponent<Outline>().enabled = false;
        }

    }

    public void OnShow(Recipe item)
    {
        if (item.resultMenuItem)
        {
            selectedIngredient = null;
            selectedMenuItem = item.resultMenuItem;

            if (item.resultMenuItem.isNew)
            {
                item.resultMenuItem.isNew = false;
                UpdateNew(item);
                if (CurrentMenuUI.instance != null)
                {
                    CurrentMenuUI.instance.UpdateNew(item.resultMenuItem);
                }
            }           
        }
        else if (item.resultIngredient)
        {
            selectedMenuItem = null;
            selectedIngredient = item.resultIngredient;

            if (item.resultIngredient.isNew)
            {
                item.resultIngredient.isNew = false;
                UpdateNew(item);

            }
        }

        // Show selected recipe
        if (item.resultMenuItem && item.resultMenuItem.recipeKnown || item.resultIngredient && item.resultIngredient.recipeKnown)
        {
            // Show as known
            ShowKnown(item);
        }
        else
        {
            // Show as unknown
            ShowUnknown(item);
        }

    }

    public void ShowKnown(Recipe item)
    {
        IconHandler iconHandler = new IconHandler();

        unknownRecipeObject.SetActive(false);
        knownMenuItemObject.SetActive(true);

        if (item.resultMenuItem)
        {
            // Enable known menuitem interface
            knownIngredientObject.SetActive(false);
            knownMenuItemObject.SetActive(true);

            // Set Image

            // Set title
            knownRecipeTitleText.GetComponent<TextMeshProUGUI>().text = item.resultMenuItem.itemName;

            // Set description
            knownMenuItemDescriptionText.GetComponent<TextMeshProUGUI>().text = item.resultMenuItem.itemDescription;

            // Set star rating
            SetStarRating(item.resultMenuItem);

            // Set gold
            knownMenuItemGoldText.GetComponent<TextMeshProUGUI>().text = item.resultMenuItem.cost.ToString();

            // Set (dis)liked
            SetLikedDisliked(item.resultMenuItem);


            // Change addtomenu button according if it's on the menu or not
            addRemoveMenuButton.GetComponent<Button>().onClick.RemoveAllListeners();
            addRemoveMenuButton.GetComponent<Button>().onClick.AddListener(() => AddRemoveToMenu(item.resultMenuItem));
            UpdateMenuButton(item.resultMenuItem);
        }
        else if (item.resultIngredient)
        {
            // Enable known ingredient interface
            knownIngredientObject.SetActive(true);
            knownMenuItemObject.SetActive(false);

            // Set title
            knownRecipeTitleText.GetComponent<TextMeshProUGUI>().text = item.resultIngredient.inventoryItemName;

            // Set description
            knownIngredientDescriptionText.GetComponent<TextMeshProUGUI>().text = item.resultIngredient.description;

            // Set cost
            knownIngredientCostText.GetComponent<TextMeshProUGUI>().text = item.resultIngredient.cost.ToString();
        }

        // Set ingredients
        SetIngredients(item);

        // Change state of favorited button
        favoritedButton.GetComponent<Button>().onClick.RemoveAllListeners();
        favoritedButton.GetComponent<Button>().onClick.AddListener(() => FavoriteMenuItem(item));
        UpdateFavoriteButton(item);

    }

    public void SetStarRating(MenuItem item)
    {
        for (int i = 0; i < 5; i++)
        {
            if (i <= item.starrating - 1)
            {
                // Full star
                knownMenuItemStarRatingObject.transform.GetChild(i).GetComponent<RawImage>().texture = IconHandler.instance.fullStar;
            }
            else
            {
                // Empty star
                knownMenuItemStarRatingObject.transform.GetChild(i).GetComponent<RawImage>().texture = IconHandler.instance.emptyStar;
            }
        }
    }

    public void SetLikedDisliked(MenuItem item)
    {
        // Clear (dis)liked
        for (int i = knownMenuItemLikedSpecies.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(knownMenuItemLikedSpecies.transform.GetChild(i).gameObject);
        }
        for (int i = knownMenuItemDislikedSpecies.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(knownMenuItemDislikedSpecies.transform.GetChild(i).gameObject);
        }
        for (int i = knownMenuItemLikedClass.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(knownMenuItemLikedClass.transform.GetChild(i).gameObject);
        }
        for (int i = knownMenuItemDislikedClass.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(knownMenuItemDislikedClass.transform.GetChild(i).gameObject);
        }

        // Set (dis)liked
        if (item.preferredByAllSpecies || item.dislikedByAllSpecies)
        {
            // Item is either liked or disliked by all species
            Texture2D allIcon = IconHandler.instance.anyIcon;
            if (item.preferredByAllSpecies)
            {
                IconHandler.instance.MakeIcon(allIcon, knownMenuItemLikedSpecies);
            }
            else if (item.dislikedByAllSpecies)
            {
                IconHandler.instance.MakeIcon(allIcon, knownMenuItemDislikedSpecies);
            }
        }
        else
        {
            foreach (Species species in item.preferredBySpecies)
            {
                Texture2D icon = IconHandler.instance.GetIconOnSpecies(species);
                IconHandler.instance.MakeIcon(icon, knownMenuItemLikedSpecies);
            }

            foreach (Species species in item.dislikedBySpecies)
            {
                Texture2D icon = IconHandler.instance.GetIconOnSpecies(species);
                IconHandler.instance.MakeIcon(icon, knownMenuItemDislikedSpecies);
            }
        }

        if (item.preferredByAllClass || item.dislikedByAllClass)
        {
            // Item is either liked or disliked by all class
            Texture2D allIcon = IconHandler.instance.anyIcon;
            if (item.preferredByAllClass)
            {
                IconHandler.instance.MakeIcon(allIcon, knownMenuItemLikedClass);
            }
            else if (item.dislikedByAllSpecies)
            {
                IconHandler.instance.MakeIcon(allIcon, knownMenuItemDislikedClass);
            }
        }
        else
        {
            foreach (Class iconclass in item.preferredByClass)
            {
                Texture2D icon = IconHandler.instance.GetIconOnClass(iconclass);
                IconHandler.instance.MakeIcon(icon, knownMenuItemLikedClass);
            }

            foreach (Class iconclass in item.dislikedByClass)
            {
                Texture2D icon = IconHandler.instance.GetIconOnClass(iconclass);
                IconHandler.instance.MakeIcon(icon, knownMenuItemDislikedClass);
            }
        }
    }

    public void SetIngredients(Recipe item)
    {
        if (item.resultMenuItem)
        {
            string ingredientString = "";
            List<IngredientTotal> ingredients = item.GetTotalCost(new List<IngredientTotal>());
            if (ingredients.Count > 0)
            {
                foreach (IngredientTotal ingredient in ingredients)
                {
                    if (ingredient.inventoryItem)
                    {
                        if (ingredient.amount > 1)
                        {
                            ingredientString += $" - {ingredient.inventoryItem.inventoryItemName} ({ingredient.amount}) \n";
                        }
                        else
                        {
                            ingredientString += $" - {ingredient.inventoryItem.inventoryItemName} \n";
                        }

                    }
                    else
                    {
                        if (ingredient.amount > 1)
                        {
                            ingredientString += $" - Any {ingredient.ingredientType.ToString()} ({ingredient.amount}) \n";
                        }
                        else
                        {
                            ingredientString += $" - Any {ingredient.ingredientType.ToString()} \n";
                        }

                    }
                }
            }
            knownMenuItemIngredients.GetComponent<TextMeshProUGUI>().text = ingredientString;
        }
        else if (item.resultIngredient)
        {
            if (item.menuItemBase)
            {
                knownIngredientBase.GetComponent<TextMeshProUGUI>().text = item.menuItemBase.itemName;
            }
            else knownIngredientBase.GetComponent<TextMeshProUGUI>().text = "/";
            knownIngredientLiquid.GetComponent<TextMeshProUGUI>().text = item.recipeLiquid.ToString();
            knownIngredientGem.GetComponent<TextMeshProUGUI>().text = item.recipeGem.ToString();
            knownIngredientTemp.GetComponent<TextMeshProUGUI>().text = item.recipeTemp.ToString();

            string ingredientString = "";
            List<IngredientTotal> ingredients = item.GetTotalCost(new List<IngredientTotal>());
            if (ingredients.Count > 0)
            {
                foreach (IngredientTotal ingredient in ingredients)
                {
                    if (ingredient.inventoryItem)
                    {
                        if (ingredient.amount > 1)
                        {
                            ingredientString += $" - {ingredient.inventoryItem.inventoryItemName} ({ingredient.amount}) \n";
                        }
                        else
                        {
                            ingredientString += $" - {ingredient.inventoryItem.inventoryItemName} \n";
                        }

                    }
                    else
                    {
                        if (ingredient.amount > 1)
                        {
                            ingredientString += $" - Any {ingredient.ingredientType.ToString()} ({ingredient.amount}) \n";
                        }
                        else
                        {
                            ingredientString += $" - Any {ingredient.ingredientType.ToString()} \n";
                        }

                    }
                }
            }
            knownIngredientIngredients.GetComponent<TextMeshProUGUI>().text = ingredientString;
        }
    }

    public void ShowUnknown(Recipe item)
    {
        knownMenuItemObject.SetActive(false);
        unknownRecipeObject.SetActive(true);

        favoritedRecipeButton.GetComponent<Button>().onClick.RemoveAllListeners();
        favoritedRecipeButton.GetComponent<Button>().onClick.AddListener(() => FavoriteMenuItem(item));
        UpdateFavoriteButton(item);

        if (item.resultMenuItem && item.resultMenuItem.recipeKnown)
        {
            unknownRecipeName.GetComponent<TextMeshProUGUI>().text = item.resultMenuItem.itemName;
        }
        else if (item.resultIngredient && item.resultIngredient.recipeKnown)
        {
            unknownRecipeName.GetComponent<TextMeshProUGUI>().text = item.resultIngredient.inventoryItemName;
        }

        string hintString = "";
        int missingHintCount = 0;
        if (item != null)
        {
            if (item.menuItemBase != null)
            {
                if (item.menuItemBaseHint)
                {
                    hintString += item.menuItemBaseText;
                }
                else
                {
                    hintString += " ... ";
                    missingHintCount++;
                }
            }
            if (item.recipeLiquid != LiquidSetting.None)
            {
                if (item.liquidHint)
                {
                    hintString += item.liquidText;
                }
                else
                {
                    hintString += " ... ";
                    missingHintCount++;
                }
            }
            if (item.ingredientHints.Any(x => x.knowHint))
            {
                foreach (IngredientHint hint in item.ingredientHints)
                {
                    if (hint.knowHint)
                    {
                        hintString += hint.ingredientText;
                    }
                    else
                    {
                        hintString += " ... ";
                        missingHintCount++;
                    }
                }
            }
            else
            {
                missingHintCount += item.ingredientHints.Count;
            }
            if (item.recipeTemp != TempSetting.None)
            {
                if (item.tempHint)
                {
                    hintString += item.tempText;
                }
                else
                {
                    hintString += " ... ";
                    missingHintCount++;
                }
            }
            if (item.recipeGem != GemSetting.None)
            {
                if (item.gemHint)
                {
                    hintString += item.gemText;
                }
                else
                {
                    hintString += " ... ";
                    missingHintCount++;
                }
            }

        }

        if (missingHintCount != 0)
        {
            missingHintsText.GetComponent<TextMeshProUGUI>().text = $"Missing {missingHintCount} hints.";
        }
        else
        {
            missingHintsText.GetComponent<TextMeshProUGUI>().text = $"All hints gathered. Discover this recipe at the recipe station.";
        }

        unkownRecipeHints.GetComponent<TextMeshProUGUI>().text = hintString;
    }

    public void AddRemoveToMenu(MenuItem item)
    {
        if (TavernManager.state == TavernState.OverworldNight || TavernManager.state == TavernState.OverworldDay)
        {
            if (!item.inMenu)
            {
                AddToMenu(item);
                if (CurrentMenuUI.instance != null)
                {
                    CurrentMenuUI.instance.AddListMenuItem(item);
                }
            }
            else
            {
                RemoveFromMenu(item);
                if (CurrentMenuUI.instance != null)
                {
                    CurrentMenuUI.instance.RemoveListMenuItem(item);
                }
            }
            // Update button
            UpdateMenuButton(item);
        }
    }
    public void AddToMenu(MenuItem item)
    {
        TavernManager.instance.AddToMenu(item);
        UpdateGridItem(item.recipe);
    }
    public void RemoveFromMenu(MenuItem item)
    {
        TavernManager.instance.RemoveFromMenu(item);
        UpdateGridItem(item.recipe);
    }
    public void UpdateGridItem(Recipe item)
    {
        if (item.resultMenuItem)
        {
            GameObject gridobject = gridItems.First(x => x.GetComponent<GridItem>().menuItem == item.resultMenuItem);
            if (gridobject != null)
            {
                if (item.resultMenuItem.inMenu || item.resultMenuItem.standardInMenu)
                {
                    gridobject.transform.GetChild(2).gameObject.SetActive(true);
                }
                else
                {
                    gridobject.transform.GetChild(2).gameObject.SetActive(false);
                }

                if (item.resultMenuItem.isFavorited)
                {
                    gridobject.transform.GetChild(3).gameObject.SetActive(true);
                }
                else
                {
                    gridobject.transform.GetChild(3).gameObject.SetActive(false);
                }
            }
        }
        else if (item.resultIngredient)
        {
            GameObject gridobject = gridItems.First(x => x.GetComponent<GridItem>().inventoryItem == item.resultIngredient);
            if (gridobject != null)
            {
                gridobject.transform.GetChild(2).gameObject.SetActive(false);
                if (item.resultIngredient.isFavorited)
                {
                    gridobject.transform.GetChild(3).gameObject.SetActive(true);
                }
                else
                {
                    gridobject.transform.GetChild(3).gameObject.SetActive(false);
                }
            }
        }

        ApplyFilter();
    }

    public void UpdateMenuButton(MenuItem item)
    {
        if (selectedMenuItem == item)
        {
            if (item.inMenu && !item.standardInMenu)
            {
                addRemoveMenuButton.GetComponent<Button>().enabled = true;
                addRemoveMenuButton.GetComponentInChildren<TextMeshProUGUI>().text = "Remove from menu";
            }
            else
            {
                if (item.standardInMenu)
                {
                    addRemoveMenuButton.GetComponent<Button>().enabled = false;
                    addRemoveMenuButton.GetComponentInChildren<TextMeshProUGUI>().text = "Always in menu";
                }
                else
                {
                    if (TavernManager.instance.CanAddItem(item))
                    {
                        addRemoveMenuButton.GetComponent<Button>().enabled = true;
                    }
                    else
                    {
                        addRemoveMenuButton.GetComponent<Button>().enabled = false;
                    }
                    addRemoveMenuButton.GetComponentInChildren<TextMeshProUGUI>().text = "Add to menu";
                }
            
            }
        }
    }
    public void UpdateAddRemoveButton()
    {
        if (selectedMenuItem != null && selectedMenuItem.recipeKnown && !selectedMenuItem.standardInMenu)
        {
            if (selectedMenuItem.inMenu)
            {
                addRemoveMenuButton.GetComponent<Button>().enabled = true;
                addRemoveMenuButton.GetComponentInChildren<TextMeshProUGUI>().text = "Remove from menu";
            }
            else if (!selectedMenuItem.inMenu)
            {
                addRemoveMenuButton.GetComponent<Button>().enabled = true;
                addRemoveMenuButton.GetComponentInChildren<TextMeshProUGUI>().text = "Add to menu";
            }
        }
        
    }

    public void FavoriteMenuItem(Recipe item)
    {
        if(item.resultMenuItem)
        {
            if (item.resultMenuItem.isFavorited)
            {
                item.resultMenuItem.isFavorited = false;
            }
            else
            {
                item.resultMenuItem.isFavorited = true;
            }

            // Update button
            UpdateFavoriteButton(item);
            UpdateGridItem(item);
            if (CurrentMenuUI.instance != null)
            {
                CurrentMenuUI.instance.UpdateGridItem(item.resultMenuItem);
            }
        }
        else if (item.resultIngredient)
        {
            if (item.resultIngredient.isFavorited)
            {
                item.resultIngredient.isFavorited = false;
            }
            else
            {
                item.resultIngredient.isFavorited = true;
            }

            // Update button
            UpdateFavoriteButton(item);
            UpdateGridItem(item);
        }
        
    }
    public void UpdateFavoriteButton(Recipe item)
    {
        
        if (item.resultMenuItem && item.resultMenuItem.isFavorited || item.resultIngredient && item.resultIngredient.isFavorited)
        {
            Rect rect = new Rect(0,0, IconHandler.instance.fullStar.width, IconHandler.instance.fullStar.height);
            if (item.resultMenuItem && item.resultMenuItem.recipeKnown || item.resultIngredient && item.resultIngredient.recipeKnown)
            {
                favoritedButton.GetComponent<Image>().sprite = Sprite.Create(IconHandler.instance.fullStar, rect, new Vector2(0,0), 512);
            }
            else
            {
                favoritedRecipeButton.GetComponent<Image>().sprite = Sprite.Create(IconHandler.instance.fullStar, rect, new Vector2(0, 0), 512);
            }
        }
        else
        {
            Rect rect = new Rect(0, 0, IconHandler.instance.emptyStar.width, IconHandler.instance.emptyStar.height);
            if (item.resultMenuItem && item.resultMenuItem.recipeKnown || item.resultIngredient && item.resultIngredient.recipeKnown)
            {
                favoritedButton.GetComponent<Image>().sprite = Sprite.Create(IconHandler.instance.emptyStar, rect, new Vector2(0, 0), 512);
            }
            else
            {
                favoritedRecipeButton.GetComponent<Image>().sprite = Sprite.Create(IconHandler.instance.emptyStar, rect, new Vector2(0, 0), 512);
            }
        }
    }

    #region Filters
    public void ApplyFilter()
    {
        string trimmedString = searchField.text.Trim();
        if (string.IsNullOrEmpty(trimmedString))
        {
            trimmedString = string.Empty;
        }
        bool isFavored = isFavoredToggle.isOn;
        bool isOnMenu = isOnMenuToggle.isOn;
        bool isUndiscovered = isUndiscoveredToggle.isOn;
        bool isHideUndiscovered = false;
        if (hideUndiscoveredToggle != null)
        {
            isHideUndiscovered = hideUndiscoveredToggle.isOn;
        }

        List<GameObject> filteredSpeciesLiked = speciesObjectList.Where(x => x.GetComponent<FilterScrollButton>().state == FilterScrollState.Like).ToList();
        List<GameObject> filteredSpeciesDisliked = speciesObjectList.Where(x => x.GetComponent<FilterScrollButton>().state == FilterScrollState.Dislike).ToList();
        List<GameObject> filteredClassLiked = classObjectList.Where(x => x.GetComponent<FilterScrollButton>().state == FilterScrollState.Like).ToList();
        List<GameObject> filteredClassDisliked = classObjectList.Where(x => x.GetComponent<FilterScrollButton>().state == FilterScrollState.Dislike).ToList();

        // Foreach through entries, disable gameobject if outside of filter. No need to refresh items
        foreach (GameObject gridObject in gridItems)
        {
            GridItem gridItem = gridObject.GetComponent<GridItem>();

            if (trimmedString == string.Empty || gridItem.menuItem && gridItem.menuItem.itemName.Trim().ToLower().Contains(trimmedString) || gridItem.inventoryItem && gridItem.inventoryItem.inventoryItemName.Trim().ToLower().Contains(trimmedString))
            {
                if (gridItem.menuItem && gridItem.menuItem.itemName.Trim().ToLower().Contains(trimmedString))
                {
                    if (gridItem.menuItem.recipeKnown && isUndiscovered)
                    {
                        gridObject.SetActive(false);
                    }
                    else if (!gridItem.menuItem.recipeKnown && isHideUndiscovered)
                    {
                        gridObject.SetActive(false);
                    }
                    else if (!gridItem.menuItem.isFavorited && isFavored)
                    {
                        gridObject.SetActive(false);
                    }
                    else if (!gridItem.menuItem.inMenu && isOnMenu && !gridItem.menuItem.standardInMenu)
                    {
                        gridObject.SetActive(false);
                    }
                    else
                    {
                        gridObject.SetActive(true);
                    }
                }
                else if (gridItem.inventoryItem && gridItem.inventoryItem.inventoryItemName.Trim().ToLower().Contains(trimmedString))
                {
                    if (gridItem.inventoryItem.recipeKnown && isUndiscovered)
                    {
                        gridObject.SetActive(false);
                    }
                    else if (!gridItem.inventoryItem.recipeKnown && isHideUndiscovered)
                    {
                        gridObject.SetActive(false);
                    }
                    else if (!gridItem.inventoryItem.isFavorited && isFavored)
                    {
                        gridObject.SetActive(false);
                    }
                    else if (isOnMenu)
                    {
                        gridObject.SetActive(false);
                    }
                    else
                    {
                        gridObject.SetActive(true);
                    }
                }

            }
            else
            {
                gridObject.SetActive(false);
            }
        }

        // Filter species & class
        if (filteredSpeciesLiked.Count > 0 || filteredClassLiked.Count > 0 || filteredSpeciesDisliked.Count > 0 || filteredClassDisliked.Count > 0)
        {

            foreach (GameObject gridObject in gridItems.Where(x => x.gameObject.activeSelf == true).ToList())
            {
                GridItem gridItem = gridObject.GetComponent<GridItem>();

                if (gridItem.inventoryItem)
                {
                    gridObject.SetActive(false);
                }
                else if (gridItem.menuItem)
                {
                    if (!gridItem.menuItem.recipeKnown)
                    {
                        gridObject.SetActive(false);
                    }
                    else if (filteredSpeciesLiked.Count > 0 && gridItem.menuItem.preferredBySpecies.Any(x => filteredSpeciesLiked.Any(y => y.GetComponent<FilterScrollButton>().filterSpecies == x)) || filteredSpeciesLiked.Count > 0 && gridItem.menuItem.preferredByAllSpecies)
                    {
                        gridObject.SetActive(true);
                    }
                    else if (filteredClassLiked.Count > 0 && gridItem.menuItem.preferredByClass.Any(x => filteredClassLiked.Any(y => y.GetComponent<FilterScrollButton>().filterClass == x)) || filteredClassLiked.Count > 0 && gridItem.menuItem.preferredByAllClass)
                    {
                        gridObject.SetActive(true);

                    }
                    else if (filteredSpeciesDisliked.Count > 0 && gridItem.menuItem.dislikedBySpecies.Any(x => filteredSpeciesDisliked.Any(y => y.GetComponent<FilterScrollButton>().filterSpecies == x)) || filteredSpeciesDisliked.Count > 0 && gridItem.menuItem.dislikedByAllSpecies)
                    {
                        gridObject.SetActive(true);
                    }
                    else if (filteredClassDisliked.Count > 0 && gridItem.menuItem.dislikedByClass.Any(x => filteredClassDisliked.Any(y => y.GetComponent<FilterScrollButton>().filterClass == x)) || filteredClassDisliked.Count > 0 && gridItem.menuItem.dislikedByAllClass)
                    {
                        gridObject.SetActive(true);
                    }
                    else
                    {
                        gridObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void ResetFilters()
    {
        searchField.text = "";
        isFavoredToggle.isOn = false;
        isOnMenuToggle.isOn = false;
        isUndiscoveredToggle.isOn = false;

        foreach (GameObject item in speciesObjectList)
        {
            item.GetComponent<FilterScrollButton>().TurnOff();
        }
        foreach (GameObject item in classObjectList)
        {
            item.GetComponent<FilterScrollButton>().TurnOff();
        }

        ApplyFilter();
    }
    #endregion

}
