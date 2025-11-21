using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepStation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartItemPrep(Table table, List<MenuItem> menuItems)
    {
        // Start prepping each item

        foreach (MenuItem item in menuItems) 
        {
            // If player can carry more items, make item
            if (PlayerServiceManager.instance.CanCarryItem())
            {
                // Wait until minigame has been completed



                // Remove made menuitem from order
                PlayerServiceManager.instance.MakeOrderItem(table, item);

            }
            else
            {
                break;
            }
        }
    }
}
