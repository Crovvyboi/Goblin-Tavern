using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ServiceStats
{
    public Dictionary<MenuItem, int> menuitemsServed;
    public int mealsServed;
    public int drinksServed;

    public List<CustomerStats> customersVisited;

    public int goldMade;
    public int avgReputationGained;
    public int seatsFilled;


    public ServiceStats()
    {
        menuitemsServed = new Dictionary<MenuItem, int>();
        mealsServed = 0;
        drinksServed = 0;

        customersVisited = new List<CustomerStats>();

        goldMade = 0;
        avgReputationGained = 0;
        seatsFilled = 0;
    }

    public void AddToGlobalStats()
    {
        TavernStatManager.instance.ModifyAfterService(this);
        GlobalStats.instance.AddServiceStatsToGlobal(this);
    }


    public void AddToServedMenuItems(List<MenuItem> menuItems)
    {
        foreach (MenuItem menuItem in menuItems)
        {
            if (menuitemsServed.ContainsKey(menuItem))
            {
                menuitemsServed[menuItem]++;
            }
            else
            {
                menuitemsServed.Add(menuItem, 1);
            }

            if (menuItem.type == MenuItemType.Meal)
            {
                mealsServed++;
            }
            else
            {
                drinksServed++;
            }
        }
    }
    public int GetServedMeals()
    {
        return mealsServed;
    }
    public int GetServedDrinks()
    {
        return drinksServed;
    }
    public KeyValuePair<MenuItem, int> GetMostServedMeal()
    {
        IEnumerable<KeyValuePair<MenuItem, int>> meals = menuitemsServed.Where(x => x.Key.type == MenuItemType.Meal);
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
        IEnumerable<KeyValuePair<MenuItem, int>> drinks = menuitemsServed.Where(x => x.Key.type == MenuItemType.Drink);
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


    public void AddToServedCustomer(CustomerStats customerStats)
    {
        customersVisited.Add(customerStats);
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


    public void AddGold(int gold)
    {
        goldMade += gold;
    }
    public int GetGold()
    {
        return goldMade;
    }

    public void AddFilledSeat()
    {
        seatsFilled++;
    }
    public int GetFilledSeat()
    {
        return seatsFilled;
    }

    public void AddReputation()
    {
        
    }
    public int GetReputation()
    {
        return avgReputationGained;
    }
}
