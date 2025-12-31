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

    [Header("Hints")]
    public bool knowRecipeName;
    public List<RecipeHint> hints = new List<RecipeHint>();

    [Header("Results")]
    public MenuItem resultMenuItem;
    public InventoryItem resultIngredient;

    public bool CheckIngredients(List<InventoryItem> ingredientsToCheck)
    {
        List<InventoryItem> ingedrients = new List<InventoryItem>();
        ingedrients.AddRange(ingredientsToCheck);

        List<InventoryItem> recipeHolder = new List<InventoryItem>();
        recipeHolder.AddRange(recipeIngredients);
        List<IngredientType> recipeTypeHolder = new List<IngredientType>();
        recipeTypeHolder.AddRange(recipeIngredientType);

        // Check ingredients
        List<InventoryItem> ingredientsToRemove = new List<InventoryItem>();
        foreach (InventoryItem item in ingedrients)
        {
            if (recipeHolder.Contains(item))
            {
                ingredientsToRemove.Add(item);
                recipeHolder.Remove(item);
            }
        }
        foreach (InventoryItem item in ingredientsToRemove)
        {
            ingedrients.Remove(item);
        }
        ingredientsToRemove.Clear();

        // Check types
        foreach (InventoryItem item in ingedrients)
        {
            if (recipeTypeHolder.Contains(item.ingredientType))
            {
                ingredientsToRemove.Add(item);
                recipeTypeHolder.Remove(item.ingredientType);
            }
        }
        foreach (InventoryItem item in ingredientsToRemove)
        {
            ingedrients.Remove(item);
        }

        // Check if any ingredient(type) is left
        if (recipeHolder.Count == 0 && recipeTypeHolder.Count == 0 && ingedrients.Count == 0)
        {
            return true;
        }
        return false;
    }
}

public class RecipeHint
{
    public bool knowHint;
    public string hintText;
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
