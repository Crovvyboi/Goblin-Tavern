using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InventoryItem")]
public class InventoryItem : ScriptableObject
{
    public string inventoryItemName;
    public InventoryItemType type;

    public GameObject inventoryItemHolder;

    [Header("Container")]
    public bool containerItem;
    public ContainerType containerType;
    public Texture2D containerSprite;

    [Header("Hotbar")]
    public Texture2D hotbarIcon;
}

public enum InventoryItemType
{
    Quest,
    Ingredient,
    Furniture
}