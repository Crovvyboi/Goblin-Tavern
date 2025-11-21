using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemContainer : MonoBehaviour
{
    public ContainerType containerType;

    public int maxItemsInContainer;
    public List<InventoryItem> itemsInContainer = new List<InventoryItem>();

    public bool CanAddItem()
    {
        if (itemsInContainer.Count < maxItemsInContainer)
        {
            return true;
        }
        return false;
    }

    public int ReturnItemCount()
    {
        return itemsInContainer.Count;
    }

    public void AddItem(InventoryItem addItem)
    {
        itemsInContainer.Add(addItem);

        // Set sprite to next stage

    }

    public void RemoveItem(InventoryItem removeItem)
    {
        itemsInContainer.Remove(removeItem);

        // Set sprite to previous stage

    }
}

public enum ContainerType
{
    Herb,
    Fish
}
