using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour
{
    public List<GameObject> standingSpots = new List<GameObject>();
    private int standingSpotCount;

    // Start is called before the first frame update
    void Start()
    {
        GetStandingSpots();
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
