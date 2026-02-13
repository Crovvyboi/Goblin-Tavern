using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentMenuUI : MonoBehaviour
{
    public static CurrentMenuUI instance;

    public GameObject gridItemPrefab;
    public List<GameObject> gridItems = new List<GameObject>();
    public GameObject contentParent;

    [Header("Menu")]
    public GameObject menuItemGrid;
    public GameObject mealsInMenuContainer;
    public GameObject drinksInMenuContainer;
    public GameObject alwaysInMenuContainer;
    public GameObject menuItemPrefab;

    public List<GameObject> menuListItems = new List<GameObject>();

    [Header("Filter")]
    public GameObject filterObject;
    public TMP_InputField searchField;
    public Toggle isFavoredToggle;
    public Toggle isOnMenuToggle;

    public GameObject filterScrollPrefab;
    public GameObject speciesFilter;
    public List<GameObject> speciesObjectList = new List<GameObject>();
    public GameObject classFilter;
    public List<GameObject> classObjectList = new List<GameObject>();

    public void ApplyFilter()
    {
        string trimmedString = searchField.text.Trim();
        if (string.IsNullOrEmpty(trimmedString))
        {
            trimmedString = string.Empty;
        }
        bool isFavored = isFavoredToggle.isOn;
        bool isOnMenu = isOnMenuToggle.isOn;

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
                if (!gridItem.menuItem.isFavorited && isFavored)
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

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        // Load in known recipes
        List<MenuItem> items = TavernManager.instance.menuItems.Where(x => x.recipeKnown && !x.standardInMenu).ToList();

        // Make tile in grid for each recipe
        foreach (MenuItem item in items)
        {
            GameObject newGridItem = GameObject.Instantiate(gridItemPrefab);
            gridItems.Add(newGridItem);
            newGridItem.transform.SetParent(contentParent.transform, false);

            newGridItem.GetComponent<GridItem>().menuItem = item;

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

                newGridItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "? ? ?";
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
                newGridItem.GetComponent<Button>().onClick.AddListener(() => AddRemoveToMenu(item));

                // Set icon & title
                if (item.icon)
                {
                    newGridItem.transform.GetChild(0).GetComponent<RawImage>().texture = item.icon;
                }
                newGridItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.gridTitle;
                newGridItem.GetComponent<CanvasGroup>().alpha = 1f;

                if (item.inMenu)
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

        // Add known species & classes to filter
        foreach (Species filterSpecies in Enum.GetValues(typeof(Species)))
        {
            GameObject newObject = GameObject.Instantiate(filterScrollPrefab);
            speciesObjectList.Add(newObject);
            newObject.transform.SetParent(speciesFilter.transform, false);

            FilterScrollButton filterScrollButton = newObject.GetComponent<FilterScrollButton>();
            filterScrollButton.filterType = FilterScrollType.Species;
            filterScrollButton.filterSpecies = filterSpecies;

            newObject.GetComponent<Button>().onClick.AddListener(() => filterScrollButton.ChangeButtonStateCurrentMenu());
        }

        foreach (Class filterClass in Enum.GetValues(typeof(Class)))
        {
            GameObject newObject = GameObject.Instantiate(filterScrollPrefab);
            classObjectList.Add(newObject);
            newObject.transform.SetParent(classFilter.transform, false);

            FilterScrollButton filterScrollButton = newObject.GetComponent<FilterScrollButton>();
            filterScrollButton.filterType = FilterScrollType.Class;
            filterScrollButton.filterClass = filterClass;

            newObject.GetComponent<Button>().onClick.AddListener(() => filterScrollButton.ChangeButtonStateCurrentMenu());
        }

        // Reset filters
        ResetFilters();

        // Load in current menu
        LoadCurrentMenu();
    }

    public void UpdateMenuList()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(menuItemGrid.GetComponent<RectTransform>());
    }

    public void LoadCurrentMenu()
    {
        List<MenuItem> mealMenuItems = TavernManager.instance.tavernMenu.Where(x => x.menuItemType == MenuItemType.Meal && !x.standardInMenu).ToList();
        List<MenuItem> drinkMenuItems = TavernManager.instance.tavernMenu.Where(x => x.menuItemType == MenuItemType.Drink && !x.standardInMenu).ToList();
        List<MenuItem> alwaysOnMenu = TavernManager.instance.menuItems.Where(x => x.standardInMenu).ToList();

        // Add meal menu items
        foreach (MenuItem item in mealMenuItems)
        {
            AddListMenuItem(mealsInMenuContainer, item);
        }

        // Add drink menu items
        foreach (MenuItem item in drinkMenuItems)
        {
            AddListMenuItem(drinksInMenuContainer, item);
        }

        // Add permanent menu items
        foreach (MenuItem item in alwaysOnMenu)
        {
            AddListMenuItem(alwaysInMenuContainer, item);
        }
    }

    public void AddListMenuItem(GameObject parent, MenuItem menuItem)
    {
        TavernManager.instance.RemoveFromMenu(menuItem);

        GameObject newObject = GameObject.Instantiate(menuItemPrefab);
        menuListItems.Add(newObject);
        newObject.transform.SetParent(parent.transform, false);

        newObject.GetComponent<GridItem>().menuItem = menuItem;
        if (!menuItem.standardInMenu)
        {
            newObject.GetComponent<Button>().onClick.AddListener(() => RemoveListMenuItem(newObject, menuItem));
        }

        newObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = menuItem.itemName;
        newObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = menuItem.cost.ToString();

        UpdateMenuList();

        menuItem.inMenu = true;

        UpdateGridItem(menuItem);
        if (KnownRecipesUI.instance != null)
        {
            KnownRecipesUI.instance.UpdateGridItem(menuItem.recipe);
        }
       
    }
    public void AddListMenuItem(MenuItem menuItem)
    {
        TavernManager.instance.AddToMenu(menuItem);

        GameObject parent;
        switch (menuItem.menuItemType)
        {
            case MenuItemType.Meal:
                parent = mealsInMenuContainer;
                break;
            case MenuItemType.Drink:
                parent = drinksInMenuContainer;
                break;
            default:
                parent = alwaysInMenuContainer;
                break;
        }

        GameObject newObject = GameObject.Instantiate(menuItemPrefab);
        menuListItems.Add(newObject);
        newObject.transform.SetParent(parent.transform, false);

        newObject.GetComponent<GridItem>().menuItem = menuItem;
        if (!menuItem.standardInMenu)
        {
            newObject.GetComponent<Button>().onClick.AddListener(() => RemoveListMenuItem(newObject, menuItem));
        }
        else
        {
            newObject.GetComponent<Button>().enabled = false;
        }

        newObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = menuItem.itemName;
        newObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = menuItem.cost.ToString();

        UpdateMenuList();

        menuItem.inMenu = true;

        UpdateGridItem(menuItem);
        if (KnownRecipesUI.instance != null)
        {
            KnownRecipesUI.instance.UpdateGridItem(menuItem.recipe);
        }
    }

    public void RemoveListMenuItem(GameObject menuobject, MenuItem menuItem)
    {
        if (TavernManager.state == TavernState.OverworldNight || TavernManager.state == TavernState.OverworldDay)
        {
            TavernManager.instance.RemoveFromMenu(menuItem);

            menuListItems.Remove(menuobject);
            GameObject.Destroy(menuobject);

            menuItem.inMenu = false;

            UpdateMenuList();
            UpdateGridItem(menuItem);
            if (KnownRecipesUI.instance != null)
            {
                KnownRecipesUI.instance.UpdateGridItem(menuItem.recipe);
            }
        }
    }

    public void RemoveListMenuItem(MenuItem menuItem)
    {
        if (TavernManager.state == TavernState.OverworldNight || TavernManager.state == TavernState.OverworldDay)
        {
            TavernManager.instance.RemoveFromMenu(menuItem);

            GameObject menuobject = menuListItems.First(x => x.GetComponent<GridItem>().menuItem == menuItem);

            menuListItems.Remove(menuobject);
            GameObject.Destroy(menuobject);

            menuItem.inMenu = false;

            UpdateMenuList();
            UpdateGridItem(menuItem);
            if (KnownRecipesUI.instance != null)
            {
                KnownRecipesUI.instance.UpdateGridItem(menuItem.recipe);
            }
        }
    }

    public void UpdateGridItem(MenuItem menuItem)
    {
        if (!menuItem.standardInMenu)
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
        }
        
    }

    public void AddRemoveToMenu(MenuItem item)
    {
        if (item.isNew)
        {
            item.isNew = false;
            UpdateNew(item);
            if (KnownRecipesUI.instance != null)
            {
                KnownRecipesUI.instance.UpdateNew(item.recipe);
            }
        }

        if (TavernManager.state == TavernState.OverworldNight || TavernManager.state == TavernState.OverworldDay)
        {
            if (item.inMenu)
            {
                RemoveListMenuItem(item);
            }
            else
            {
                AddListMenuItem(item);
            }

            UpdateMenuList();
        }
    }

    public void UpdateNew(MenuItem item)
    {
        gridItems.First(x => x.GetComponent<GridItem>().menuItem == item).GetComponent<Outline>().enabled = false;
    }
}
