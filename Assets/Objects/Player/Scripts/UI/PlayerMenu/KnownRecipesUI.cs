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

    [Header("Recipe overview")]
    public GameObject recipeOverviewObject;
    public GameObject contentParent;
    public GameObject gridItemPrefab;
    public List<GameObject> gridItems = new List<GameObject>();

    [Header("Recipe details")]
    public GameObject recipeDetailsObject;
    public GameObject menuItemDetails;
    public GameObject menuItemLikeDislike;

    [Header("Known recipe")]
    public GameObject knownRecipeObject;
    public GameObject knownRecipeImage;
    public GameObject knownRecipeTitleText;
    public GameObject knownRecipeDescriptionText;
    public GameObject knownRecipeStarRatingObject;
    public GameObject knownRecipeGoldText;
    public GameObject knownRecipeLikedSpecies;
    public GameObject knownRecipeDislikedSpecies;
    public GameObject knownRecipeLikedClass;
    public GameObject knownRecipeDislikedClass;

    public GameObject addRemoveMenuButton;
    public GameObject favoritedButton;

    [Header("Unknown recipe")]
    public GameObject unknownRecipeName;
    public GameObject unknownRecipeObject;
    public GameObject favoritedRecipeButton;
    public GameObject unkownRecipeHints;

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
        List<MenuItem> items = TavernManager.instance.menuItems.Where(x => x.recipeKnown || x.recipe.hints.Where(y => y.knowHint).ToList().Count > 0).ToList();

        // Make tile in grid for each recipe
        foreach (MenuItem item in items)
        {
           AddMenuItemToGrid(item);

        }


        // Select first known recipe
        if (items.Any(x => x.isFavorited) && items.First(x => x.isFavorited) != null)
        {
            selectedMenuItem = items.First(x => x.isFavorited);
        }
        else 
        {
            selectedMenuItem = items[0];
        }
        OnShow(selectedMenuItem);


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

    public void AddMenuItemToGrid(MenuItem item)
    {
        GameObject newGridItem = GameObject.Instantiate(gridItemPrefab);
        gridItems.Add(newGridItem);
        newGridItem.transform.SetParent(contentParent.transform, false);

        newGridItem.GetComponent<GridItem>().menuItem = item;
        newGridItem.GetComponent<Button>().onClick.AddListener(() => OnShow(item));

        if (item.isNew)
        {
            newGridItem.GetComponent<Outline>().enabled = true;
        }
        else
        {
            newGridItem.GetComponent<Outline>().enabled = false;
        }

        if (!item.recipeKnown)
        {
            // If player does not know recipe, set as unkown tile & decrease opacity
            if (item.recipeKnown)
            {
                newGridItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.itemName;
            }
            else
            {
                newGridItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "? ? ?";
            }
            newGridItem.GetComponent<CanvasGroup>().alpha = 0.5f;

            newGridItem.transform.GetChild(2).gameObject.SetActive(false);

            if (item.isFavorited)
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
            if (item.icon)
            {
                newGridItem.transform.GetChild(0).GetComponent<RawImage>().texture = item.icon;
            }
            newGridItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.gridTitle;
            newGridItem.GetComponent<CanvasGroup>().alpha = 1f;

            if (item.inMenu || item.standardInMenu)
            {
                newGridItem.transform.GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                newGridItem.transform.GetChild(2).gameObject.SetActive(false);
            }
            if (item.isFavorited)
            {
                newGridItem.transform.GetChild(3).gameObject.SetActive(true);
            }
            else
            {
                newGridItem.transform.GetChild(3).gameObject.SetActive(false);
            }
        }
    }

    public void SwitchToDetails()
    {
        menuItemLikeDislike.SetActive(false);
        menuItemDetails.SetActive(true);
    }
    public void SwitchToLikedDisliked()
    {
        menuItemDetails.SetActive(false);
        menuItemLikeDislike.SetActive(true);
    }

    public void UpdateNew(MenuItem item)
    {
        gridItems.First(x => x.GetComponent<GridItem>().menuItem == item).GetComponent<Outline>().enabled = false;
    }

    public void OnShow(MenuItem item)
    {
        selectedMenuItem = item;

        if (item.isNew)
        {
            item.isNew = false;
            UpdateNew(item);
            if (CurrentMenuUI.instance != null)
            {
                CurrentMenuUI.instance.UpdateNew(item);
            }
        }

        // Show selected recipe
        if (item.recipeKnown)
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

    public void ShowKnown(MenuItem item)
    {
        IconHandler iconHandler = new IconHandler();

        unknownRecipeObject.SetActive(false);
        knownRecipeObject.SetActive(true);

        // Set Image

        // Set title
        knownRecipeTitleText.GetComponent<TextMeshProUGUI>().text = item.itemName;

        // Set description
        knownRecipeDescriptionText.GetComponent<TextMeshProUGUI>().text = item.itemDescription;

        // Set star rating
        SetStarRating(item);

        // Set gold
        knownRecipeGoldText.GetComponent<TextMeshProUGUI>().text = item.cost.ToString();

        // Set (dis)liked
        SetLikedDisliked(item);


        // Change addtomenu button according if it's on the menu or not
        addRemoveMenuButton.GetComponent<Button>().onClick.RemoveAllListeners();
        addRemoveMenuButton.GetComponent<Button>().onClick.AddListener(() => AddRemoveToMenu(item));
        UpdateMenuButton(item);

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
                knownRecipeStarRatingObject.transform.GetChild(i).GetComponent<RawImage>().texture = IconHandler.instance.fullStar;
            }
            else
            {
                // Empty star
                knownRecipeStarRatingObject.transform.GetChild(i).GetComponent<RawImage>().texture = IconHandler.instance.emptyStar;
            }
        }
    }

    public void SetLikedDisliked(MenuItem item)
    {
        // Clear (dis)liked
        for (int i = knownRecipeLikedSpecies.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(knownRecipeLikedSpecies.transform.GetChild(i).gameObject);
        }
        for (int i = knownRecipeDislikedSpecies.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(knownRecipeDislikedSpecies.transform.GetChild(i).gameObject);
        }
        for (int i = knownRecipeLikedClass.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(knownRecipeLikedClass.transform.GetChild(i).gameObject);
        }
        for (int i = knownRecipeDislikedClass.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(knownRecipeDislikedClass.transform.GetChild(i).gameObject);
        }

        // Set (dis)liked
        if (item.preferredByAllSpecies || item.dislikedByAllSpecies)
        {
            // Item is either liked or disliked by all species
            Texture2D allIcon = IconHandler.instance.anyIcon;
            if (item.preferredByAllSpecies)
            {
                IconHandler.instance.MakeIcon(allIcon, knownRecipeLikedSpecies);
            }
            else if (item.dislikedByAllSpecies)
            {
                IconHandler.instance.MakeIcon(allIcon, knownRecipeDislikedSpecies);
            }
        }
        else
        {
            foreach (Species species in item.preferredBySpecies)
            {
                Texture2D icon = IconHandler.instance.GetIconOnSpecies(species);
                IconHandler.instance.MakeIcon(icon, knownRecipeLikedSpecies);
            }

            foreach (Species species in item.dislikedBySpecies)
            {
                Texture2D icon = IconHandler.instance.GetIconOnSpecies(species);
                IconHandler.instance.MakeIcon(icon, knownRecipeDislikedSpecies);
            }
        }

        if (item.preferredByAllClass || item.dislikedByAllClass)
        {
            // Item is either liked or disliked by all class
            Texture2D allIcon = IconHandler.instance.anyIcon;
            if (item.preferredByAllClass)
            {
                IconHandler.instance.MakeIcon(allIcon, knownRecipeLikedClass);
            }
            else if (item.dislikedByAllSpecies)
            {
                IconHandler.instance.MakeIcon(allIcon, knownRecipeDislikedClass);
            }
        }
        else
        {
            foreach (Class iconclass in item.preferredByClass)
            {
                Texture2D icon = IconHandler.instance.GetIconOnClass(iconclass);
                IconHandler.instance.MakeIcon(icon, knownRecipeLikedClass);
            }

            foreach (Class iconclass in item.dislikedByClass)
            {
                Texture2D icon = IconHandler.instance.GetIconOnClass(iconclass);
                IconHandler.instance.MakeIcon(icon, knownRecipeDislikedClass);
            }
        }
    }

    public void ShowUnknown(MenuItem item)
    {
        knownRecipeObject.SetActive(false);
        unknownRecipeObject.SetActive(true);

        favoritedRecipeButton.GetComponent<Button>().onClick.RemoveAllListeners();
        favoritedRecipeButton.GetComponent<Button>().onClick.AddListener(() => FavoriteMenuItem(item));
        UpdateFavoriteButton(item);

        if (item.recipeKnown)
        {
            unknownRecipeName.GetComponent<TextMeshProUGUI>().text = item.itemName;
        }

        string hintString = "";
        if (item.recipe != null)
        {
            

            foreach (RecipeHint hint in item.recipe.hints)
            {
                if (hint.knowHint)
                {
                    hintString += $" - {hint.hintText}\n";
                }
                else
                {
                    hintString += $" - ? ? ?\n";
                }
            }
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
        UpdateGridItem(item);
    }
    public void RemoveFromMenu(MenuItem item)
    {
        TavernManager.instance.RemoveFromMenu(item);
        UpdateGridItem(item);
    }
    public void UpdateGridItem(MenuItem menuItem)
    {
        GameObject gridobject = gridItems.First(x => x.GetComponent<GridItem>().menuItem.itemName == menuItem.itemName);
        if (gridobject != null)
        {
            if (menuItem.inMenu || menuItem.standardInMenu)
            {
                gridobject.transform.GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                gridobject.transform.GetChild(2).gameObject.SetActive(false);
            }

            if (menuItem.isFavorited)
            {
                gridobject.transform.GetChild(3).gameObject.SetActive(true);
            }
            else
            {
                gridobject.transform.GetChild(3).gameObject.SetActive(false);
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
        if (selectedMenuItem.recipeKnown && !selectedMenuItem.standardInMenu)
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

    public void FavoriteMenuItem(MenuItem item)
    {
        if (item.isFavorited)
        {
            item.isFavorited = false;
        }
        else
        {
            item.isFavorited = true;
        }

        // Update button
        UpdateFavoriteButton(item);
        UpdateGridItem(item);
        if (CurrentMenuUI.instance != null)
        {
            CurrentMenuUI.instance.UpdateGridItem(item);
        }
    }
    public void UpdateFavoriteButton(MenuItem item)
    {
        
        if (item.isFavorited)
        {
            Rect rect = new Rect(0,0, IconHandler.instance.fullStar.width, IconHandler.instance.fullStar.height);
            if (item.recipeKnown)
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
            if (item.recipeKnown)
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
        bool isHideUndiscovered = hideUndiscoveredToggle.isOn;

        List<GameObject> filteredSpeciesLiked = speciesObjectList.Where(x => x.GetComponent<FilterScrollButton>().state == FilterScrollState.Like).ToList();
        List<GameObject> filteredSpeciesDisliked = speciesObjectList.Where(x => x.GetComponent<FilterScrollButton>().state == FilterScrollState.Dislike).ToList();
        List<GameObject> filteredClassLiked = classObjectList.Where(x => x.GetComponent<FilterScrollButton>().state == FilterScrollState.Like).ToList();
        List<GameObject> filteredClassDisliked = classObjectList.Where(x => x.GetComponent<FilterScrollButton>().state == FilterScrollState.Dislike).ToList();

        // Foreach through entries, disable gameobject if outside of filter. No need to refresh items
        foreach (GameObject gridObject in gridItems)
        {
            GridItem gridItem = gridObject.GetComponent<GridItem>();

            if (trimmedString == string.Empty || gridItem.menuItem.itemName.Trim().ToLower().Contains(trimmedString))
            {
                if (gridItem.menuItem.recipeKnown && isUndiscovered)
                {
                    gridObject.SetActive(false);
                }
                else if (!gridItem.menuItem.recipeKnown && isHideUndiscovered)
                {
                    gridObject.SetActive(false);
                }
                else if ( !gridItem.menuItem.isFavorited && isFavored)
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

                if (filteredSpeciesLiked.Count > 0 && gridItem.menuItem.preferredBySpecies.Any(x => filteredSpeciesLiked.Any(y => y.GetComponent<FilterScrollButton>().filterSpecies == x)) || filteredSpeciesLiked.Count > 0 && gridItem.menuItem.preferredByAllSpecies)
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
