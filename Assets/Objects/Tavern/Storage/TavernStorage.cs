using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TavernStorage : MonoBehaviour
{
    public static TavernStorage instance;

    public List<TavernStorageObject> tavernInventory = new List<TavernStorageObject>();

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }  
    }

}

[Serializable]
public class TavernStorageObject
{
    public InventoryItem item;
    public int amount;

    public TavernStorageObject(InventoryItem item)
    {
        this.item = item;
        amount = 1;
    }

    public TavernStorageObject(InventoryItem item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }

    public void IncreaseByOne()
    {
        amount++;
    }

    public bool IsItem(InventoryItem item)
    {
        if (this.item == item)
        {
            return true;
        }
        return false;
    }
}
