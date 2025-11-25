using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CustomerGenerator : MonoBehaviour
{
    public static CustomerGenerator instance;

    public GameObject customerPrefab;
    public int? minTableSize;
    public int? maxTableSize;

    public GameObject spawnLocation;
    public float spawnTimer = 0f;
    public float spawnInterval;

    public List<Node> occupiedHangouts = new List<Node>();

    [Header("Reputation")]
    public Dictionary<Class, int> classReputations = new Dictionary<Class, int>();
    public Dictionary<Species, int> speciesReputations = new Dictionary<Species, int>();

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        spawnInterval = (float)Random.Range(1, 3);
    }

    private void FixedUpdate()
    {
        if (TavernManager.state == TavernState.Service && ServiceManager.instance.customerGroupPool.Count > 0)
        {
            SpawnTimer();
        }
    }


    public List<List<CustomerStats>> GenerateAllCustomers()
    {
        GetMinMaxTableSize();

        PrepareClassReputations();
        PrepareSpeciesReputations();

        List<List<CustomerStats>> customerGroups = new List<List<CustomerStats>>();
        occupiedHangouts.Clear();

        // Generate passive customer groups
        List<GameObject> tablelist = GameObject.FindGameObjectsWithTag("Table").Where(x => x.GetComponent<Table>() != null).ToList();
        foreach (GameObject table in tablelist) {
            table.GetComponent<Table>().GetChairs();
            List<CustomerStats> customersAtTable = GenerateCustomerGroup(table.GetComponent<Table>().GetChairCount(), true);

            // Assign table & chair to customer
            List<GameObject> availableChairs = table.GetComponent<Table>().chairs;
            foreach (CustomerStats stats in customersAtTable)
            {
                stats.assignedTable = table;

                int chairIndex = Random.Range(0, availableChairs.Count);
                stats.assignedChair = availableChairs[chairIndex];
                availableChairs.RemoveAt(chairIndex);
            }

            customerGroups.Add(customersAtTable);
        }

        // Generate active customer groups based on tavern size
        int amount = 5;
        for (int i = 0; i < amount; i++)
        {
            List<CustomerStats> group = GenerateCustomerGroup();
            if (group != null)
            {
                customerGroups.Add(group);
            }
        }

        return customerGroups;
    }

    public void GetMinMaxTableSize()
    {
        minTableSize = null; 
        maxTableSize = null;

        // Get all tables
        List<GameObject> tablelist = GameObject.FindGameObjectsWithTag("Table").Where(x => x.GetComponent<Table>() != null).ToList();

        foreach (GameObject tableobject in tablelist) 
        {
            Table table = tableobject.GetComponent<Table>();
            if (minTableSize > table.GetChairCount()|| minTableSize == null)
            {
                minTableSize = table.GetChairCount();
            }

            if (maxTableSize < table.GetChairCount() || maxTableSize == null)
            {
                maxTableSize = table.GetChairCount();
            }
        }
    }

    public List<CustomerStats> GenerateCustomerGroup(int tablesize, bool isTableGroup)
    {
        // Set group size based off of most seats at a table and least seats at a table
        // int groupsize = Random.Range((int)minTableSize, (int)maxTableSize);

        // Generate non VIP customers
        List<CustomerStats> group = new List<CustomerStats>();
        for (int i = 0; i < tablesize; i++)
        {
            CustomerStats newCustomer = GenerateCustomer(isTableGroup);
            group.Add(newCustomer);
        }

        return group;
    }

    public List<CustomerStats> GenerateCustomerGroup()
    {
        // Set group size based off of space available in tavern

        // Generate non VIP customers
        List<CustomerStats> group = new List<CustomerStats>();
        for (int i = 0; i < Random.Range(2,5); i++)
        {
            CustomerStats newCustomer = GenerateCustomer(false);
            group.Add(newCustomer);
        }

        // Determine hangout spot
        Node hangoutSpot = TavernTilemapManager.instance.AssignHangoutSpot(occupiedHangouts);
        if (hangoutSpot != null)
        {
            occupiedHangouts.Add(hangoutSpot);

            // Assign group to each other
            foreach (CustomerStats customer in group)
            {
                customer.knowsOthers = new List<CustomerStats>(group);
                customer.knowsOthers.Remove(customer);
                customer.AssignHangoutSpot(hangoutSpot);

            }

            return group;
        }
        return null;
    }

    public void GenerateVIP()
    {

    }

    public CustomerStats GenerateCustomer(bool isAtTable)
    {
        // Generate customer object based on reputation
        bool isVIP = false;

        // Determine gold on hand based on tavern reputation
        int budget = 5;

        // Determine species and class based on tavern stats
        Species? customerSpecies = GenerateSpecies();
        if (customerSpecies == null)
        {
            customerSpecies = Species.Human;
        }

        Class? customerClass = GenerateClass();
        if (customerClass == null)
        {
            customerClass = Class.Fighter;
        }


        // Set customer variables
        CustomerStats newcustomer = new CustomerStats(isVIP, budget, (Species)customerSpecies, (Class)customerClass, isAtTable);

        return newcustomer;
    }

    public void PrepareClassReputations()
    {
        classReputations = new Dictionary<Class, int>();
        foreach (Class item in Enum.GetValues(typeof(Class)))
        {
            // Check if class can be generated
            if (CanGenerateClass())
            {
                int generationShare = 0;

                // Add general reputation
                if (TavernStatManager.instance.classReputation.Any(x => x.classrep == item))
                {
                    generationShare += TavernStatManager.instance.classReputation.First(x => x.classrep == item).reputationScore;
                }
                else
                {
                    generationShare += 1;
                }

                // Add set menu modifier
                foreach (MenuItem menuitem in ServiceManager.instance.definitiveMenu)
                {
                    if (menuitem.preferredByAllClass || menuitem.preferredByClass.Any(x => x == item))
                    {
                        generationShare += 5;
                    }
                    if (menuitem.dislikedByAllClass || menuitem.dislikedByClass.Any(x => x == item))
                    {
                        generationShare -= 5;
                    }
                }

                // Add furniture modifier

                // Add music modifier

                // Add class to list
                if (generationShare > 0)
                {
                    classReputations.Add(item, generationShare);
                }

            }
        }
    }
    public bool CanGenerateClass()
    {
        if (true)
        {
            return true;
        }
        return false;
    }
    public Class? GenerateClass()
    {
        int classSum = classReputations.Values.Sum();
        int generated = Random.Range(1, classSum + 1);

        foreach (KeyValuePair<Class, int> item in classReputations)
        {
            generated -= item.Value;
            if (generated <= 0)
            {
                return item.Key;
            }
        }
        return null;
    }

    public void PrepareSpeciesReputations()
    {
        speciesReputations = new Dictionary<Species, int>();
        foreach (Species item in Enum.GetValues(typeof(Species)))
        {
            // Check if class can be generated
            if (CanGenerateSpecies())
            {
                int generationShare = 0;

                // Add general reputation
                if (TavernStatManager.instance.speciesReputation.Any(x => x.speciesrep == item))
                {
                    generationShare += TavernStatManager.instance.speciesReputation.First(x => x.speciesrep == item).reputationScore;
                }
                else
                {
                    generationShare += 1;
                }

                // Add set menu modifier
                foreach (MenuItem menuitem in ServiceManager.instance.definitiveMenu)
                {
                    if (menuitem.preferredByAllSpecies || menuitem.preferredBySpecies.Any(x => x == item))
                    {
                        generationShare += 5;
                    }
                    if (menuitem.dislikedByAllSpecies || menuitem.dislikedBySpecies.Any(x => x == item))
                    {
                        generationShare -= 5;
                    }
                }

                // Add furniture modifier

                // Add music modifier

                // Add class to list
                if (generationShare > 0)
                {
                    speciesReputations.Add(item, generationShare);
                }
            }
        }
    }
    public bool CanGenerateSpecies()
    {
        if (true)
        {
            return true;
        }
        return false;
    }
    public Species? GenerateSpecies()
    {
        int speciesSum = speciesReputations.Values.Sum();
        int generated = Random.Range(1, speciesSum + 1);

        foreach (KeyValuePair<Species, int> item in speciesReputations)
        {
            generated -= item.Value;
            if (generated <= 0)
            {
                return item.Key;
            }
        }
        return null;
    }


    public void SpawnTimer()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnInterval)
        {
            spawnTimer = 0f;
            spawnInterval = Random.Range(1, 3);

            SpawnCustomerGroup();
        }
    }

    public void SpawnCustomerGroup()
    {
        // Get random group
        int groupindex = Random.Range(0, ServiceManager.instance.customerGroupPool.Count);
        List<CustomerStats> customergroup = ServiceManager.instance.customerGroupPool[groupindex];

        // Remove random group from list
        ServiceManager.instance.customerGroupPool.RemoveAt(groupindex);

        // Spawn in every customer in group
        foreach (CustomerStats stats in customergroup)
        {
            GameObject newCustomer;

            if (stats.isVIP)
            {
                newCustomer = new GameObject();

            }
            else if (stats.isAtTable)
            {
                newCustomer = GameObject.Instantiate(customerPrefab);

                // Assign Behaviour type A
                newCustomer.AddComponent(typeof(CustomerBehaviourA));
                newCustomer.GetComponent<CustomerBehaviourA>().customerStats = stats;

                // Update stats
                ServiceManager.instance.stats.AddFilledSeat();
            }
            else
            {
                newCustomer = GameObject.Instantiate(customerPrefab);

                // Assign Behaviour type B
                newCustomer.AddComponent(typeof(CustomerBehaviourB));
                newCustomer.GetComponent<CustomerBehaviourB>().customerStats = stats;
            }

            newCustomer.transform.position = spawnLocation.transform.position;

            ServiceManager.instance.generatedCustomers.Add(newCustomer);

            // Update stats
            ServiceManager.instance.stats.AddToServedCustomer(stats);
        }

    }
}
