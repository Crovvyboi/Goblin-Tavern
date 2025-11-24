using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bar : MonoBehaviour
{
    public static Bar instance;

    public List<GameObject> standingSpots = new List<GameObject>();
    private int standingSpotCount;

    public List<MenuItem> barOrders = new List<MenuItem>();
    public List<MenuItem> madeOrders = new List<MenuItem>();

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        GetStandingSpots();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnServiceStart()
    {
        foreach (GameObject item in standingSpots)
        {
            item.GetComponent<BarWaitingSpot>().occupyingObject = null;
        }
    }

    public void MakeBarOrder()
    {
        if (barOrders.Count > 0)
        {
            MenuItem madeItem = barOrders[0];

            // Remove from orders
            barOrders.RemoveAt(0);

            // Add to made pool
            madeOrders.Add(madeItem);
        }
    }

    public void GetStandingSpots()
    {
        foreach (Transform child in this.transform)
        {
            if (child.tag == "Bar Standing Spot")
            {
                standingSpots.Add(child.gameObject);
            }
        }

        standingSpotCount = standingSpots.Count;
    }

    public int GetStandingSpotCount()
    {
        return standingSpotCount;
    }

    public bool OccupySpot(GameObject customer, out GameObject freeSpot)
    {
        List<GameObject> freeSpots = standingSpots.Where(x => x.GetComponent<BarWaitingSpot>().occupyingObject == null).ToList();
        if (freeSpots.Count > 0)
        {
            freeSpot = freeSpots[Random.Range(0, freeSpots.Count - 1)];
            freeSpot.GetComponent<BarWaitingSpot>().occupyingObject = customer;
            return true;
        }
        else
        {
            freeSpot = null;
            return false;
        }
    }

    public void LeaveSpot(GameObject customer)
    {
        if (standingSpots.Any(x => x.GetComponent<BarWaitingSpot>().occupyingObject == customer))
        {
            standingSpots.First(x => x.GetComponent<BarWaitingSpot>().occupyingObject == customer).GetComponent<BarWaitingSpot>().occupyingObject = null;
        }
    }
}
