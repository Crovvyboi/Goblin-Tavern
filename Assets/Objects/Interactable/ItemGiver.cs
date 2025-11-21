using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemGiver : MonoBehaviour
{
    public InventoryItem giveItem;

    public void GiveItem()
    {
        PlayerInventory.instance.GiveItem(giveItem);
    }
}
