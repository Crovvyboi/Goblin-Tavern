using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class CustomerBase : MonoBehaviour
{
    [SerializeField]
    public CustomerStats customerStats;

    //public Vector3? targetPosition;
    //public bool isMoving = false;

    public CustomerGoal customerGoal = CustomerGoal.None;
    public CustomerState customerState = CustomerState.Idling;

    public List<MenuItem> currentOrder = new List<MenuItem>();

    [Header("Movement")]
    public float moveSpeed;
    public Vector3 currentPos;

    public List<Node> path = new List<Node>();

    public List<Vector3> goalQueue = new List<Vector3>();
    public Vector3? currentgoal;

    public bool canWander = false;

    #region Movement
    //public void SetTarget(Vector3 target)
    //{
    //    this.targetPosition = target;
    //}

    //public IEnumerator MoveToTarget()
    //{
    //    isMoving = true;

    //    Debug.Log("Moving to target");
    //    yield return new WaitForSeconds(5);

    //    Vector3 target = (Vector3)targetPosition;
    //    this.transform.position = target;

    //    if (this.transform.position == target)
    //    {
    //        targetPosition = null;
    //    }

    //    Debug.Log("Moved to target");

    //    isMoving = false;
    //}

    // Goal injections
    public void InjectNewGoalNode(Node newGoal)
    {
        if (newGoal.gameObject.transform.position != currentPos && newGoal.gameObject.transform.position != currentgoal)
        {
            goalQueue.Clear();
            path.Clear();
            currentgoal = newGoal.gameObject.transform.position;
        }
    }
    public void InjectNewGoalObject(GameObject newGoal)
    {
        if (newGoal.transform.position != currentPos && newGoal.transform.position != currentgoal)
        {
            goalQueue.Clear();
            path.Clear();
            currentgoal = newGoal.gameObject.transform.position;
        }
    }
    public void InjectNewGoalPosition(Vector3 newGoal)
    {
        if (newGoal != currentPos && newGoal != currentgoal)
        {
            goalQueue.Clear();
            path.Clear();
            currentgoal = newGoal;
        }
    }

    // Queue injections
    public void InjectNewQueue(List<Node> newGoals)
    {
        goalQueue.Clear();
        foreach (Node goal in newGoals)
        {
            AddToQueue(goal.transform.position);
        }

        currentgoal = goalQueue[0];
        goalQueue.RemoveAt(0);
    }
    public void InjectNewQueue(List<Vector3> newGoals)
    {
        goalQueue.Clear();
        foreach (Vector3 goal in newGoals)
        {
            AddToQueue(goal);
        }

        currentgoal = goalQueue[0];
        goalQueue.RemoveAt(0);
    }
    public void AddToQueue(Vector3 newGoal)
    {
        goalQueue.Add(newGoal);
    }

    // Through point calculations
    public Node FindThroughpoint(Vector3 goal, Vector3 currentPos)
    {
        return PathfinderScript.instance.FindThroughpoint(goal, currentPos);

    }

    // Path creation
    public void CreatePath()
    {
        if (path.Count > 0)
        {
            int x = 0;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(path[x].transform.position.x, path[x].transform.position.y, 0), moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, path[x].transform.position) < 0.01f)
            {
                currentPos = path[x].gameObject.transform.position;
                transform.position = currentPos;
                path.RemoveAt(x);
            }

            if (path.Count == 0)
            {
                // Check if there's another goal in queue
                if (goalQueue.Count > 0)
                {
                    currentgoal = goalQueue[0];
                    goalQueue.RemoveAt(0);
                }
                else
                {
                    currentgoal = null;
                }    
            }
        }
        else if (currentgoal != null)
        {
            path = PathfinderScript.instance.FindPathFromPosToPos(currentPos, (Vector3)currentgoal);
            if (path == null || path.Count == 0)
            {
                currentgoal = null;
                path = new List<Node>();
            }

        }
        else
        {
            if (canWander && customerState == CustomerState.Idling)
            {
                Node[] nodes = FindObjectsOfType<Node>();
                while (path == null || path.Count == 0)
                {
                    path = PathfinderScript.instance.FindPathFromPosToPos(currentPos, nodes[Random.Range(0, nodes.Length)].gameObject.transform.position);
                }
            }
        }
    }

    public bool IsMovingToTarget()
    {
        if (currentgoal != null || this.transform.position != currentgoal)
        {
            return true;
        }
        return false;
    }

    public void OnFinalCall()
    {
        customerGoal = CustomerGoal.Exit;
        customerState = CustomerState.MovingToExit;

        InjectNewGoalPosition(CustomerGenerator.instance.spawnLocation.transform.position);
        
    }
    #endregion

    #region Ordering & Consuming
    public List<MenuItem> DetermineDrinkOrder()
    {
        List<MenuItem> order = new List<MenuItem>();

        List<MenuItem> preferredItems = new List<MenuItem>();

        // Filter out preferred of both species & class items in budget
        preferredItems = ServiceManager.instance.definitiveMenu.FindAll(x => 
            x.preferredBySpecies.Contains(customerStats.customerSpecies) && x.preferredByClass.Contains(customerStats.customerClass) && x.cost <= customerStats.budget && x.type == MenuItemType.Drink ||
            x.preferredByAllSpecies && x.preferredByAllClass && x.cost <= customerStats.budget && x.type == MenuItemType.Drink
        );
        if (preferredItems.Count > 0)
        {
            order.Add(preferredItems[Random.Range(0, preferredItems.Count)]);
        }
        else
        {
            // if no preferred items, filter out preferred on 1
            preferredItems = ServiceManager.instance.definitiveMenu.FindAll(x =>
                x.preferredBySpecies.Contains(customerStats.customerSpecies) && x.cost <= customerStats.budget || x.preferredByClass.Contains(customerStats.customerClass) && x.cost <= customerStats.budget && x.type == MenuItemType.Drink
            );
            if (preferredItems.Count > 0)
            {
                order.Add(preferredItems[Random.Range(0, preferredItems.Count)]);
            }
            else
            {
                // if no preferred items, filter out neutral items in budget
                preferredItems = ServiceManager.instance.definitiveMenu.FindAll(x =>
                    !x.dislikedBySpecies.Contains(customerStats.customerSpecies) && !x.dislikedByClass.Contains(customerStats.customerClass) && x.cost <= customerStats.budget && 
                    !x.dislikedByAllSpecies && !x.dislikedByAllClass && x.type == MenuItemType.Drink
                );
                if (preferredItems.Count > 0)
                {
                    order.Add(preferredItems[Random.Range(0, preferredItems.Count)]);
                }
                else
                {
                    // else, order water
                    order.Add(ServiceManager.instance.definitiveMenu.Find(x => x.itemName == "Water"));
                }
            }
        }

        return order;
    }

    public List<MenuItem> DetermineMealOrder()
    {
        List<MenuItem> order = new List<MenuItem>();

        List<MenuItem> preferredItems = new List<MenuItem>();

        // Filter out preferred of both species & class items in budget
        preferredItems = ServiceManager.instance.definitiveMenu.FindAll(x =>
            x.preferredBySpecies.Contains(customerStats.customerSpecies) && x.preferredByClass.Contains(customerStats.customerClass) && x.cost <= customerStats.budget && x.type == MenuItemType.Meal ||
            x.preferredByAllSpecies && x.preferredByAllClass && x.cost <= customerStats.budget && x.type == MenuItemType.Meal
        );
        if (preferredItems.Count > 0)
        {
            order.Add(preferredItems[Random.Range(0, preferredItems.Count)]);
        }
        else
        {
            // if no preferred items, filter out preferred on 1
            preferredItems = ServiceManager.instance.definitiveMenu.FindAll(x =>
                x.preferredBySpecies.Contains(customerStats.customerSpecies) && x.cost <= customerStats.budget || x.preferredByClass.Contains(customerStats.customerClass) && x.cost <= customerStats.budget && x.type == MenuItemType.Meal
            );
            if (preferredItems.Count > 0)
            {
                order.Add(preferredItems[Random.Range(0, preferredItems.Count)]);
            }
            else
            {
                // if no preferred items, filter out neutral items in budget
                preferredItems = ServiceManager.instance.definitiveMenu.FindAll(x =>
                    !x.dislikedBySpecies.Contains(customerStats.customerSpecies) && !x.dislikedByClass.Contains(customerStats.customerClass) && x.cost <= customerStats.budget && 
                    !x.dislikedByAllSpecies && !x.dislikedByAllClass && x.type == MenuItemType.Meal
                );
                if (preferredItems.Count > 0)
                {
                    order.Add(preferredItems[Random.Range(0, preferredItems.Count)]);
                }
                else
                {
                    // else, order Gruul
                    order.Add(ServiceManager.instance.definitiveMenu.Find(x => x.itemName == "Gruul"));
                }
            }
        }

        return order;
    }

    public IEnumerator ConsumeMenuItem(List<MenuItem> currentOrder)
    {
        // Wait for seconds based on consumptiontime menuitem
        yield return new WaitForSeconds(4.5f * currentOrder.Count);

        // Adjust stats based on menuitem
        foreach (MenuItem item in currentOrder)
        {
            customerStats.hunger += item.addToHunger;
            customerStats.thirst += item.addToThirst;

            customerStats.drunkeness += (int)(item.alcoholPercent / 100);

            customerStats.happiness += (int)(item.addToHappiness * ((Convert.ToInt32(item.preferredByClass.Contains(customerStats.customerClass)) + Convert.ToInt32(item.preferredBySpecies.Contains(customerStats.customerSpecies))) * 1.3));
            customerStats.rowdyness += (item.addToRowdyness * customerStats.drunkeness);
        }
    }

    public void Pay(int amount)
    {
        ServiceManager.instance.goldMadeInService += amount;
        ServiceManager.instance.stats.AddGold(amount);
    }
    #endregion


    private void OnDrawGizmos()
    {
        if (path.Count > 0)
        {
            Gizmos.color = Color.red;
            foreach (Node node in path)
            {
                if (node.previousNode != null)
                {
                    Gizmos.DrawLine(node.transform.position, node.previousNode.transform.position);
                }

            }
        }
    }
}
