using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class InventoryItemContainer : MonoBehaviour
{
    public ContainerType containerType;

    public int maxItemsInContainer;
    public List<InventoryItem> itemsInContainer = new List<InventoryItem>();
    public List<GameObject> itemPositions = new List<GameObject>();

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
    public int ReturnItemCount(InventoryItem item)
    {
        return itemPositions.Count(x => x == item);
    }

    public void AddItem(InventoryItem addItem)
    {
        
        if (itemPositions.Any(x => x.GetComponent<RawImage>().texture == null))
        {
            itemsInContainer.Add(addItem);

            // Set sprite to next stage
            RawImage image = itemPositions.First(x => x.GetComponent<RawImage>().texture == null).GetComponent<RawImage>();
            image.enabled = true;
            image.texture = addItem.containerSprite;
        }
        
    }

    public void RemoveItem(InventoryItem removeItem)
    {

        itemsInContainer.Remove(removeItem);

        // Set sprite to previous stage
        RawImage image = itemPositions.Last(x => x.GetComponent<RawImage>().texture == removeItem.containerSprite).GetComponent<RawImage>();
        image.texture = null;
        image.enabled = false;
    }
}

public enum ContainerType
{
    Herb,
    Fish,
    Flour,
    Yeast
}
