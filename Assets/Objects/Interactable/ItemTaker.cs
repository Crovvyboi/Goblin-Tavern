using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemTaker : MonoBehaviour
{
    public InventoryItem takeItem;

    public void TakeItem()
    {
        PlayerInventory.instance.TakeItem(takeItem);
    }
}
