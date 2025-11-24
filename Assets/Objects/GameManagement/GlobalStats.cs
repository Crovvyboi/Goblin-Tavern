using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalStats : MonoBehaviour
{
    public static GlobalStats instance;

    [Header("Playtime")]
    public float totalPlaytime;

    public int year;
    public Season season;
    public int day;

    [Header("Customers")]
    public List<CustomerStats> customersVisited;

    [Header("Menu items")]
    public Dictionary<MenuItem, int> menuitemsServed;
    public int mealsServed;
    public int drinksServed;

    [Header("Gold")]
    public int goldMade;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        customersVisited = new List<CustomerStats>();
        menuitemsServed = new Dictionary<MenuItem, int>();
        mealsServed = 0;
        drinksServed = 0;
        goldMade = 0;

        year = 1;
        season = Season.Spring;
        day = 1;
    }

    private void FixedUpdate()
    {
        totalPlaytime += Time.deltaTime;
    }

    public void SaveStats() 
    { 
    
    }

    public void LoadStats() 
    { 
    
    }

    public void AddServiceStatsToGlobal(ServiceStats serviceStats)
    {
        customersVisited.AddRange(serviceStats.customersVisited);

        foreach (KeyValuePair<MenuItem, int> item in menuitemsServed)
        {
            AddToServedMenuItems(item);
        }
        mealsServed += serviceStats.mealsServed;
        drinksServed += serviceStats.drinksServed;

        goldMade += serviceStats.goldMade;
    }


    public void AdvanceDay()
    {
        switch (season)
        {
            case Season.Spring:
                if (day + 1 > 30)
                {
                    season = Season.Summer;
                    day = 1;
                }
                else
                {
                    day++;
                }
                break;
            case Season.Summer:
                if (day + 1 > 31)
                {
                    season = Season.Autumn;
                    day = 1;
                }
                else
                {
                    day++;
                }
                break;
            case Season.Autumn:
                if (day + 1 > 30)
                {
                    season = Season.Winter;
                    day = 1;
                }
                else
                {
                    day++;
                }
                break;
            case Season.Winter:
                if (day + 1 > 31)
                {
                    year++;
                    season = Season.Spring;
                    day = 1;
                }
                else
                {
                    day++;
                }

                    break;
            default:
                break;
        }
    }

    public void AddToServedMenuItems(KeyValuePair<MenuItem, int> menuItems)
    {
        for (int i = 0; i < menuItems.Value; i++)
        {
            if (menuitemsServed.ContainsKey(menuItems.Key))
            {
                menuitemsServed[menuItems.Key]++;
            }
            else
            {
                menuitemsServed.Add(menuItems.Key, 1);
            }

            if (menuItems.Key.menuItemType == MenuItemType.Meal)
            {
                mealsServed++;
            }
            else
            {
                drinksServed++;
            }
        }
    }

    public KeyValuePair<MenuItem, int> GetMostServedMeal()
    {
        IEnumerable<KeyValuePair<MenuItem, int>> meals = menuitemsServed.Where(x => x.Key.menuItemType == MenuItemType.Meal);
        KeyValuePair<MenuItem, int>? keyValuePair = null;
        foreach (KeyValuePair<MenuItem, int> item in meals)
        {
            if (keyValuePair == null || keyValuePair.Value.Value < item.Value)
            {
                keyValuePair = item;
            }
        }

        if (keyValuePair == null)
        {
            return new KeyValuePair<MenuItem, int>(null, 0);
        }
        return (KeyValuePair<MenuItem, int>)keyValuePair;
    }
    public KeyValuePair<MenuItem, int> GetMostServedDrink()
    {
        IEnumerable<KeyValuePair<MenuItem, int>> drinks = menuitemsServed.Where(x => x.Key.menuItemType == MenuItemType.Drink);
        KeyValuePair<MenuItem, int>? keyValuePair = null;
        foreach (KeyValuePair<MenuItem, int> item in drinks)
        {
            if (keyValuePair == null || keyValuePair.Value.Value < item.Value)
            {
                keyValuePair = item;
            }
        }

        if (keyValuePair == null)
        {
            return new KeyValuePair<MenuItem, int>(null, 0);
        }
        return (KeyValuePair<MenuItem, int>)keyValuePair;
    }

    public KeyValuePair<Class, int> GetMostVisitedClass()
    {
        KeyValuePair<Class, int>? keyValuePair = null;

        Dictionary<Class, int> dictionary = new Dictionary<Class, int>();
        foreach (CustomerStats item in customersVisited)
        {
            if (dictionary.ContainsKey(item.customerClass))
            {
                dictionary[item.customerClass]++;
            }
            else
            {
                dictionary.Add(item.customerClass, 1);
            }

            if (keyValuePair == null || keyValuePair.Value.Value < dictionary[item.customerClass])
            {
                keyValuePair = new KeyValuePair<Class, int>(item.customerClass, dictionary[item.customerClass]);
            }
        }

        if (keyValuePair == null)
        {
            return new KeyValuePair<Class, int>(Class.Fighter, 0);
        }
        return (KeyValuePair<Class, int>)keyValuePair;
    }

    public KeyValuePair<Species, int> GetMostVisitedSpecies()
    {
        KeyValuePair<Species, int>? keyValuePair = null;

        Dictionary<Species, int> dictionary = new Dictionary<Species, int>();
        foreach (CustomerStats item in customersVisited)
        {
            if (dictionary.ContainsKey(item.customerSpecies))
            {
                dictionary[item.customerSpecies]++;
            }
            else
            {
                dictionary.Add(item.customerSpecies, 1);
            }

            if (keyValuePair == null || keyValuePair.Value.Value < dictionary[item.customerSpecies])
            {
                keyValuePair = new KeyValuePair<Species, int>(item.customerSpecies, dictionary[item.customerSpecies]);
            }
        }

        if (keyValuePair == null)
        {
            return new KeyValuePair<Species, int>(Species.Human, 0);
        }
        return (KeyValuePair<Species, int>)keyValuePair;
    }


    public string GetPlaytime()
    {
        TimeSpan time = TimeSpan.FromSeconds(totalPlaytime);
        return time.ToString("hh':'mm':'ss");
    }
}

public enum Season
{
    Spring,
    Summer,
    Autumn,
    Winter
}
