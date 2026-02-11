using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemPopupContainer : MonoBehaviour
{
    public static ItemPopupContainer instance;

    public GameObject itemPopupPrefab;
    public GameObject itemPopupContainer;

    public List<GameObject> activePopupList;

    public Dictionary<InventoryItem, int> itemQueue;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        itemQueue = new Dictionary<InventoryItem, int>();
        activePopupList = new List<GameObject>();
    }

    public void MakeNewPopup(InventoryItem item, int amount)
    {
        GameObject newPopup = GameObject.Instantiate(itemPopupPrefab);
        newPopup.transform.SetParent(itemPopupContainer.transform, false);
        newPopup.GetComponent<ItemPopup>().inventoryItem = item;
        newPopup.GetComponent<ItemPopup>().amount = amount;
        newPopup.GetComponent<ItemPopup>().UpdateText();

        activePopupList.Add(newPopup);
    }

    public void AddItemPopup(InventoryItem item)
    {
        if (activePopupList.Any(x => x.GetComponent<ItemPopup>().inventoryItem == item))
        {
            // Add & refresh existing popup
            activePopupList.First(x => x.GetComponent<ItemPopup>().inventoryItem == item).GetComponent<ItemPopup>().AddToAmount();
        }
        else if (activePopupList.Count == 5)
        {
            // If max popups, add to queue
            if (itemQueue.ContainsKey(item))
            {
                itemQueue[item]++;
            }
            else
            {
                itemQueue.Add(item, 1);
            }
        }
        else
        {
            // Add new popup
            MakeNewPopup(item, 1);
        }
    }

    public void RemovePopup(ItemPopup item)
    {
        activePopupList.Remove(item.gameObject);
        if (itemQueue.Count > 0)
        {
            KeyValuePair<InventoryItem, int> keyvalue = itemQueue.ElementAt(0);
            MakeNewPopup(keyvalue.Key, keyvalue.Value);
            itemQueue.Remove(item.inventoryItem);
        }
    }
}
