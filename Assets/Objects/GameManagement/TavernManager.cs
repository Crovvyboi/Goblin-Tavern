using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TavernManager : MonoBehaviour
{
    // Tavernmanager is the connecting framework between tavern scripts
    public static TavernManager instance;

    public static TavernState state;

    public GameObject furnitureContainer;
    public GameObject decorationContainer;
    public GameObject tavernDoor;

    [Header("Recipes")]
    public List<MenuItem> menuItems = new List<MenuItem>();
    public List<Recipe> recipes = new List<Recipe>();

    [Header("Menu")]
    public int maxMealOnMenu = 5;
    public int maxDrinkOnMenu = 5;
    public List<MenuItem> tavernMenu = new List<MenuItem>();

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        state = TavernState.OverworldDay;

        GetAllMenuItems();
        GetAllRecipes();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetAllMenuItems()
    {
        string[] assets = AssetDatabase.FindAssets("t:MenuItem", null);
        foreach (string asset in assets)
        {
            MenuItem item = (MenuItem)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(asset), typeof(MenuItem));
            if (tavernMenu.Contains(item))
            {
                item.inMenu = true;
            }
            menuItems.Add(item);
        }

        // Update menuItems according to save file (known & in menu)
    }

    public void GetAllRecipes()
    {
        string[] assets = AssetDatabase.FindAssets("t:Recipe", null);
        foreach (string asset in assets)
        {
            Recipe recipe = (Recipe)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(asset), typeof(Recipe));
            recipes.Add(recipe);
        }

        // Update according to save file
    }
    public bool CanAddItem(MenuItem item)
    {
        switch (item.menuItemType)
        {
            case MenuItemType.Meal:
                if (menuItems.Where(x => x.menuItemType == MenuItemType.Meal).Count() < maxMealOnMenu)
                {
                    return true;
                }
                break;
            case MenuItemType.Drink:
                if (menuItems.Where(x => x.menuItemType == MenuItemType.Drink).Count() < maxDrinkOnMenu)
                {
                    return true;
                }
                break;
            default:
                return false;
        }
        return false;
    }
    public void AddToMenu(MenuItem item)
    {
        if (!tavernMenu.Contains(item))
        {
            if (CanAddItem(item))
            {
                tavernMenu.Add(item);
                item.inMenu = true;
            }
        }
    }
    public void RemoveFromMenu(MenuItem item)
    {
        if (tavernMenu.Contains(item))
        {
            tavernMenu.Remove(item);
            item.inMenu = false;
        }
    }

    public void EndDay()
    {
        tavernMenu.RemoveAll(x => x.standardInMenu);

        // Switch to night phase
        state = TavernState.OverworldNight;
    }

    public void EndDayWithoutService()
    {
        // Apply rep penalty
        TavernStatManager.instance.OnDaySkipped();

        // Switch to night phase
        state = TavernState.OverworldNight;
    }

    public void EndNight()
    {
        // Add day to counter
        GlobalStats.instance.AdvanceDay();

        // Switch to day phase
        state = TavernState.OverworldDay;

    }

}
