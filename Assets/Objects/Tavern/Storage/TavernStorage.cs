using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public void AddItem(InventoryItem item, int amount)
    {
        if (tavernInventory.Any(x => x.item == item))
        {
            TavernStorageObject storageobject = tavernInventory.First(x => x.item == item);
            storageobject.amount += amount;
        }
        else
        {
            tavernInventory.Add(new TavernStorageObject(item, amount));
        }
    }

    public void TakeItem(InventoryItem item)
    {
        if (tavernInventory.Any(x => x.item == item))
        {
            TavernStorageObject storageobject = tavernInventory.First(x => x.item == item);
            storageobject.amount--;
            if (storageobject.amount <= 0)
            {
                tavernInventory.Remove(storageobject);
            }
        }
    }
    public void TakeItem(IngredientType item)
    {
        if (tavernInventory.Any(x => x.item.ingredientType == item))
        {
            TavernStorageObject storageobject = tavernInventory.First(x => x.item.ingredientType == item);
            storageobject.amount--;
            if (storageobject.amount <= 0)
            {
                tavernInventory.Remove(storageobject);
            }
        }
    }

    public int CountItem(InventoryItem item)
    {
        int count = 0;

        if (tavernInventory.Any(x => x.item == item))
        {
            count += tavernInventory.First(x => x.item == item).amount;
        }

        return count;
    }
    public int CountItem(IngredientType item)
    {
        int count = 0;

        if (tavernInventory.Any(x => x.item.ingredientType == item))
        {
            count += tavernInventory.First(x => x.item.ingredientType == item).amount;
        }

        return count;
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
