using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static PlayerServiceManager;

public class Table : MonoBehaviour
{
    public GameObject chairPositionHolder;
    public List<GameObject> chairPositions;
    public List<GameObject> chairs;
    private int chairCount;

    public float orderTimer = 0f;
    public float orderInterval;
    public List<MenuItem> nextOrder = new List<MenuItem>();
    public List<MenuItem> currentOpenOrder;
    public List<MenuItem> deliveredItems = new List<MenuItem>();
    public bool orderTaken;

    public GameObject orderReadyMarker;
    public GameObject orderHighlightMarker;

    // Start is called before the first frame update
    void Start()
    {
        orderInterval = Random.Range(0, 4);

        GetChairs();

        orderReadyMarker.SetActive(false);
        orderHighlightMarker.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (TavernManager.state == TavernState.OverworldNight || TavernManager.state == TavernState.OverworldDay)
        {
            if (Hotbar.instance.furniturePlacementMold != null && Hotbar.instance.furniturePlacementMold.tag == "Chair")
            {
                chairPositionHolder.SetActive(true);
                GetChairs();
            }
            else
            {
                chairPositionHolder.SetActive(false);
            }
        }

        if (!orderTaken && nextOrder.Count > 0)
        {
            OrderTimer();
        }
    }

    public void OnInteract()
    {
        if (TavernManager.state == TavernState.Service || TavernManager.state == TavernState.ServiceFinalCall)
        {
            if (orderTaken)
            {
                PlaceHeldOrderItems();
            }
            else if (currentOpenOrder.Count > 0)
            {
                {
                    if (PlayerServiceManager.instance.CanTakeNewOrder())
                    {
                        PlayerServiceManager.instance.AddOrder(this, currentOpenOrder);
                        orderTaken = true;

                        orderReadyMarker.GetComponent<SpriteRenderer>().color = new Color(255, 255, 47);
                    }
                }
            }
        }
        else
        {
            ThoughtBubble.instance.Think("This table is ready for use!");
        }
    }

    public void UpdateChairSpotArrow(ChairSpot spot, bool add)
    {
        if (add)
        {
            spot.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.green;
        }
        else
        {
            spot.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;
        }
        
    }

    public void GetChairs()
    {
        chairPositions.Clear();
        chairs.Clear();
        foreach (Transform child in chairPositionHolder.transform)
        {
            chairPositions.Add(child.gameObject);

            List<GameObject> chairsInDirection = GameObject.FindGameObjectsWithTag("Chair").ToList();
            ChairSpot chairspot = child.gameObject.GetComponent<ChairSpot>();
            List<GameObject> morechairs =  chairsInDirection.Where(
                x =>
                x.GetComponent<Furniture>().currentOrientation == chairspot.needsDirection
                ).ToList();
            bool hasChair = morechairs.Any(
                x =>    
                Vector3.Distance(x.transform.position, new Vector3(child.position.x, child.position.y)) < 0.5f
                );
            if (hasChair)
            {
                GameObject foundChair = GameObject.FindGameObjectsWithTag("Chair").First(
                x =>
                x.GetComponent<Furniture>().currentOrientation == child.gameObject.GetComponent<ChairSpot>().needsDirection &&
                Vector3.Distance(x.transform.position, new Vector3(child.position.x, child.position.y)) < 0.5f
                );
                chairs.Add(foundChair);

                UpdateChairSpotArrow(chairspot, true);
            }
            else
            {
                UpdateChairSpotArrow(chairspot, false);
            }
        }

        chairCount = chairs.Count;
    }

    public int GetChairCount()
    {
        return chairCount;
    }


    public void OrderTimer()
    {
        orderTimer += Time.deltaTime;
        if (orderTimer > orderInterval)
        {
            orderTimer = 0f;
            orderInterval = Random.Range(0, 4);

            OrderForTable();
        }
    }

    public void OrderForTable()
    {
        if (!orderTaken)
        {
            currentOpenOrder.AddRange(nextOrder);
            nextOrder.Clear();

            orderReadyMarker.SetActive(true);
            orderReadyMarker.GetComponent<SpriteRenderer>().color = new Color(0, 255, 47);
        }  
    }

    public void PlaceHeldOrderItems()
    {
        if (PlayerServiceManager.instance.carryingItems.Count > 0)
        {
            List<MenuItem> tableitems = PlayerServiceManager.instance.carryingItems.Where(x => x.table.Equals(this)).Select(x => x.menuItem).ToList();

            if (tableitems.Count > 0)
            {
                foreach (MenuItem item in tableitems)
                {
                    deliveredItems.Add(item);
                    PlayerServiceManager.instance.carryingItems.Remove(new OrderItem(this, item));
                }

                List<MenuItem> remainingOrderItems = PlayerServiceManager.instance.takenOrders.GetValueOrDefault(this);
                if (remainingOrderItems.Count == 0)
                {
                    PlayerServiceManager.instance.RemoveOrder(this);
                    orderReadyMarker.SetActive(false);

                    orderTaken = false;
                    currentOpenOrder = new List<MenuItem>();

                    if (TavernManager.state == TavernState.ServiceFinalCall || TavernManager.state == TavernState.ServiceOverview)
                    {
                        OnFinalCall();
                    }
                    else
                    {
                        orderInterval = Random.Range(10, 15);
                    }  
                }
            }
        }
    }

    public void OnFinalCall()
    {
        orderReadyMarker.SetActive(false);
        nextOrder = new List<MenuItem>();
        currentOpenOrder = new List<MenuItem>();
    }
}
