using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Recipe")]
public class Recipe : ScriptableObject
{
    public MenuItem menuItemBase;
    public LiquidSetting recipeLiquid;
    public TempSetting recipeTemp;
    public GemSetting recipeGem;
    public List<InventoryItem> recipeIngredients = new List<InventoryItem>();
    public List<IngredientType> recipeIngredientType = new List<IngredientType>();



    [Header("Results")]
    public MenuItem resultMenuItem;
    public InventoryItem resultIngredient;
}

public enum LiquidSetting
{
    None,
    Water,
    Milk,
    Oil
}

public enum TempSetting
{
    Negative,
    None,
    Low,
    Medium,
    High
}

public enum GemSetting
{
    None,
    Elven,
    Orcish
}
