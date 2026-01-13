using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "InventoryItem")]
public class InventoryItem : ScriptableObject
{
    public string inventoryItemName;
    public InventoryItemType type;
    public int cost;

    public GameObject inventoryItemHolder;

    [Header("Container")]
    public bool containerItem;
    public ContainerType containerType;
    public Texture2D containerSprite;

    [Header("Hotbar")]
    public Texture2D hotbarIcon;

    [Header("Furniture")]
    public bool isPlaceable;
    public GameObject prefab;

    [Header("Recipe")]
    public bool recipeKnown;
    public Recipe inventoryItemRecipe;
    public bool isNew;
    public bool isFavorited;
    public string gridName;
    public string description;


    [Header("Ingredient")]
    public IngredientType ingredientType;

    [Header("Recipe book")]
    public bool isRecipeBook;
    public bool isRandom;
    public int recipesToSelect;
    public List<Recipe> recipePool = new List<Recipe>();

}

public enum InventoryItemType
{
    Quest,
    RecipeBook,
    Ingredient,
    Furniture
}

public enum IngredientType
{
    None,
    Cheese,
    Herb,
    Meat,
    Bread
}