using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MealPrepStation : PrepStation
{

    public void OnInteract()
    {
        Debug.Log("Meal prep interacted");

        switch (TavernManager.state)
        {
            case TavernState.OverworldDay:
                break;
            case TavernState.OverworldNight:
                break;
            case TavernState.Service:
                PrepMeal();
                break;
            case TavernState.ServiceFinalCall:
                PrepMeal();
                break;
            case TavernState.ServiceOverview:
                break;
            default:
                break;
        }
    }

    public void PrepMeal()
    {
        if (PlayerServiceManager.instance.takenOrders.Count > 0)
        {
            KeyValuePair<Table, List<MenuItem>> selectedOrder = PlayerServiceManager.instance.GetHighlightedOrder();
            List<MenuItem> allMenuItems = selectedOrder.Value;

            // Filter on meals
            List<MenuItem> mealitems = new List<MenuItem>();
            mealitems = allMenuItems.Where(x => x.menuItemType == MenuItemType.Meal).ToList();

            StartItemPrep(selectedOrder.Key, mealitems);
        }
    }
}
