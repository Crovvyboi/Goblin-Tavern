using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarSlot : MonoBehaviour
{
    public GameObject assignedInventoryItem;

    public void AssignItem(GameObject gameObject)
    {
        assignedInventoryItem = gameObject;
    }

    public void RemoveItem()
    {
        assignedInventoryItem = null;
    }
}
