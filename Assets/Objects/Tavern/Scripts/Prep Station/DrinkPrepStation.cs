using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrinkPrepStation : PrepStation
{
    
    public void OnInteract()
    {
        Debug.Log("Drink prep interacted");

        switch (TavernManager.state)
        {
            case TavernState.OverworldDay:
                break;
            case TavernState.OverworldNight:
                break;
            case TavernState.Service:
                PrepDrink();
                break;
            case TavernState.ServiceFinalCall:
                PrepDrink();
                break;
            case TavernState.ServiceOverview:
                break;
            case TavernState.PlaceFurniture:
                break;
            default:
                break;
        }
    }

    public void PrepDrink()
    {
        if (PlayerServiceManager.instance.takenOrders.Count > 0)
        {
            KeyValuePair<Table, List<MenuItem>> selectedOrder = PlayerServiceManager.instance.GetHighlightedOrder();
            List<MenuItem> allMenuItems = selectedOrder.Value;

            // Filter on meals
            List<MenuItem> drinkItems = new List<MenuItem>();
            drinkItems = allMenuItems.Where(x => x.type == MenuItemType.Drink).ToList();

            StartItemPrep(selectedOrder.Key, drinkItems);
        }
    }
}
