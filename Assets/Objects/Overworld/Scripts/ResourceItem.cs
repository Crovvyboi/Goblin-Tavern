using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceItem : MonoBehaviour
{
    public int giveAmount;
    public int maxGiveAmount;

    // Start is called before the first frame update
    void Start()
    {
        giveAmount = maxGiveAmount;
    }

    public void TakeOne()
    {
        if (giveAmount -1 >= 0)
        {
            ItemGiver itemgiver = GetComponent<ItemGiver>();
            if (itemgiver != null)
            {
                itemgiver.GiveItem();
                giveAmount--;

                if (giveAmount == 0)
                {
                    GetComponent<CircleCollider2D>().enabled = false;
                }
            }
        }
        
    }
}
