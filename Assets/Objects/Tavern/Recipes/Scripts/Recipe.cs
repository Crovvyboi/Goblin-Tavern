using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    // When all texts are strung together, they should form a full instruction
    public bool knowRecipeName;
    public bool menuItemBaseHint;
    public string menuItemBaseText;

    public bool liquidHint;
    public string liquidText;

    public List<IngredientHint> ingredientHints = new List<IngredientHint>();

    public bool gemHint;
    public string gemText;

    public bool tempHint;
    public string tempText;

    [Header("Results")]
    public MenuItem resultMenuItem;
    public InventoryItem resultIngredient;

    public bool CheckHints()
    {
        if (knowRecipeName || 
            menuItemBaseHint ||
            liquidHint ||
            tempHint ||
            gemHint ||
            ingredientHints.Any(x => x.knowHint)
            )
        {
            return true;
        }
        return false;
    }

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

    public List<IngredientTotal> GetTotalCost(List<IngredientTotal> totalCost)
    {
        foreach (InventoryItem ingredient in recipeIngredients)
        {
            if (totalCost.Any(x => x.inventoryItem == ingredient))
            {
                totalCost.First(x => x.inventoryItem == ingredient).amount++;
            }
            else
            {
                totalCost.Add(new IngredientTotal(ingredient, IngredientType.None));
            }
        }
        foreach (IngredientType type in recipeIngredientType)
        {
            if (totalCost.Any(x => x.ingredientType == type))
            {
                totalCost.First(x => x.ingredientType == type).amount++;
            }
            else
            {
                totalCost.Add(new IngredientTotal(null, type));
            }
        }

        if (menuItemBase && menuItemBase.recipe)
        {
            menuItemBase.recipe.GetTotalCost(totalCost);
        }

        return totalCost;
    }
}

[Serializable]

public class Ingredient
{
    public InventoryItem? inventoryItem;
    public IngredientType ingredientType;
}
public class IngredientHint : Ingredient
{
    public bool knowHint;

    public string ingredientText;
}

public class IngredientTotal : Ingredient
{
    public int amount;
    public IngredientTotal(InventoryItem? inventoryItem, IngredientType ingredientType)
    {
        this.inventoryItem = inventoryItem;
        this.ingredientType = ingredientType;
        this.amount = 1;
    }
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
