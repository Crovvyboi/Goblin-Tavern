using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CustomerStats
{
    [Header("VIP")]
    public bool isVIP;

    [Header("Table (Behaviour A)")]
    // For behaviour type A
    public bool isAtTable;
    public GameObject assignedTable;
    public GameObject assignedChair;

    [Header("Group (Behaviour B")]
    // For behaviour type B
    public List<CustomerStats> knowsOthers = new List<CustomerStats>();
    public Node meetingSpot;
    public Node standingSpot;

    [Header("Fixed stats")]
    public int budget;
    public int startHappiness;

    [Header("Species & Class")]
    public Species customerSpecies;
    public Class customerClass;

    [Header("Decision stats")]
    public int hunger;
    public int thirst;
    public int happiness;
    public int rowdyness;
    public int drunkeness;

    public CustomerStats(bool isVIP, int budget, Species customerSpecies, Class customerClass, bool isAtTable)
    {
        this.isVIP = isVIP;
        this.budget = budget;
        this.customerSpecies = customerSpecies;
        this.customerClass = customerClass;
        this.isAtTable = isAtTable;

        System.Random random = new System.Random();
        this.hunger = random.Next(50, 90);
        this.thirst = random.Next(50, 90);
        this.happiness = random.Next(40, 60);
        this.startHappiness = this.happiness;
        this.rowdyness = random.Next(0, 25);
        this.drunkeness = 1;

    }

    
}
