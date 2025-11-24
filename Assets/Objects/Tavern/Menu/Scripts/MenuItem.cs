using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MenuItem")]
public class MenuItem : ScriptableObject
{
    [Header("Typing")]
    public string itemName;
    public string itemDescription;
    public MenuItemType menuItemType;
    public int cost;
    public List<string> tags = new List<string>();
    public int starrating;

    [Header("Preferred by")]
    public bool preferredByAllSpecies;
    public bool preferredByAllClass;
    public List<Species> preferredBySpecies = new List<Species>();
    public List<Class> preferredByClass = new List<Class>();

    [Header("Disliked by")]
    public bool dislikedByAllSpecies;
    public bool dislikedByAllClass;
    public List<Species> dislikedBySpecies = new List<Species>();
    public List<Class> dislikedByClass = new List<Class>();

    [Header("Modifiers")]
    public int addToHunger;
    public int addToThirst;
    public int addToHappiness;
    public int addToRowdyness;
    public float alcoholPercent;

    [Header("GridSettings")]
    public bool isFavorited;
    public string gridTitle;
    public Texture2D icon;

    [Header("Recipe")]
    public bool knowRecipe;
    public List<MenuItemHint> recipeHints = new List<MenuItemHint>();

    [Header("Menu")]
    public bool standardInMenu;
    public bool inMenu;
}

public enum MenuItemType
{
    Meal,
    Drink
}

[Serializable]
public class MenuItemHint
{
    public string hintText;
    public bool knowHint;
}
