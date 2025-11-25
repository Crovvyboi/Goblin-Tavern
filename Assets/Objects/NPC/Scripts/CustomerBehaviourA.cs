using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CustomerBehaviourA : CustomerBase
{
    /** 
     * This is the customer AI for archetype A.
     * 
     * The customer will be able to eat and drink at the table, leaving the table when dancing or squabbling
     * When an order is placed, add to table order and wait until order is placed.
     * 
    **/

    List<MenuItem> claimedItems = new List<MenuItem>();
    public int toPay = 0;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = Random.Range(1.5f, 2.5f);
        currentPos = transform.position;

        tickInterval = (float)Random.Range(10, 15);
        MakeDecision();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (customerState == CustomerState.Idling || customerState == CustomerState.Moving)
        {
            TickTimer();
        }
        
        if (!isDoingTask)
        {
            DoTask();
        }

        if (customerState == CustomerState.Moving && this.transform.position != currentgoal ||
            customerState == CustomerState.MovingToExit && this.transform.position != currentgoal)
        {
            if (currentPos != null)
            {
                CreatePath();
            }
        }
    }

    public void TickTimer()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer > tickInterval)
        {
            tickTimer = 0f;
            tickInterval = (float)Random.Range(10, 15);

            TickDown();
        }
    }

    public void TickDown()
    {
        // Drunkeness, happiness & rowdyness not implemented

        customerStats.hunger -= Random.Range(1, 5);
        if (customerStats.hunger < 0)
        {
            customerStats.hunger = 0;   
        }
        customerStats.thirst -= Random.Range(1, 5);
        if (customerStats.thirst < 0)
        {
            customerStats.thirst = 0;
        }

        MakeDecision();
    }

    public void MakeDecision()
    {
        if (customerGoal == CustomerGoal.None)
        {
            Dictionary<CustomerGoal, int> inRange = new Dictionary<CustomerGoal, int>();

            int hungerDiff = 100 - customerStats.hunger;
            inRange.Add(CustomerGoal.Hunger, hungerDiff);
            int thirstDiff = 100 - customerStats.thirst;
            inRange.Add(CustomerGoal.Thirst, thirstDiff);

            // Determine highest priority
            CustomerGoal goingToDo = CustomerGoal.None;
            int maxvalue = 0;
            foreach (KeyValuePair<CustomerGoal, int> keyvalue in inRange)
            {
                if (keyvalue.Value > maxvalue)
                {
                    maxvalue = keyvalue.Value;
                    goingToDo = keyvalue.Key;
                }
            }

            SetTask(goingToDo);
        }
    }

    public void SetTask(CustomerGoal goingToDo)
    {
        switch (goingToDo)
        {
            case CustomerGoal.Hunger:
                // Order food (& drink)
                AssignNewGoal(CustomerGoal.Hunger);

                if (this.transform.position != customerStats.assignedChair.transform.position)
                {
                    MoveToTarget(customerStats.assignedChair.transform.position, this.transform.position);
                }
                AssignNewState(CustomerState.Moving);
                break;
            case CustomerGoal.Thirst:
                // Order drink
                AssignNewGoal(CustomerGoal.Thirst);

                if (this.transform.position != customerStats.assignedChair.transform.position)
                {
                    MoveToTarget(customerStats.assignedChair.transform.position, this.transform.position);
                }
                AssignNewState(CustomerState.Moving);
                break;
            case CustomerGoal.Exit:
                if (this.transform.position != customerStats.assignedChair.transform.position)
                {
                    InjectNewGoalPosition(CustomerGenerator.instance.spawnLocation.transform.position);
                }
                AssignNewState(CustomerState.MovingToExit);
                break;
            default:
                AssignNewGoal(CustomerGoal.None);
                AssignNewState(CustomerState.Idling);
                break;
        }
    }

    public void DoTask()
    {
        isDoingTask = true;
        switch (customerState)
        {
            case CustomerState.Idling:
                break;
            case CustomerState.Moving:             

                // Depending on goal, determine next customerstate
                switch (customerGoal)
                {
                    case CustomerGoal.Hunger:
                        if (this.transform.position == customerStats.assignedChair.transform.position)
                        {
                            AssignNewState(CustomerState.Ordering);
                        }  
                        break;
                    case CustomerGoal.Thirst:
                        if (this.transform.position == customerStats.assignedChair.transform.position)
                        {
                            AssignNewState(CustomerState.Ordering);
                        }
                        break;
                    default:
                        AssignNewGoal(CustomerGoal.None);
                        AssignNewState(CustomerState.Idling);
                        break;
                }
                break;
            case CustomerState.Ordering:
                DecideOrder();

                break;
            case CustomerState.WaitingOnOrder:
                WaitingOnOrder();
                break;
            case CustomerState.EatingOrder:
                // Wait for time, dependent on consumption time of menu item
                if (claimedItems.Count > 0)
                {
                    StartCoroutine(ConsumeMenuItem(claimedItems));
                    claimedItems = new List<MenuItem>();
                }

                if (TavernManager.state == TavernState.ServiceOverview || TavernManager.state == TavernState.ServiceFinalCall)
                {
                    OnFinalCall();
                }
                else
                {
                    // Switch to Idle and make new decision
                    AssignNewState(CustomerState.Idling);
                    AssignNewGoal(CustomerGoal.None);
                }
               
                break;
            case CustomerState.MeetTarget:

                break;
            case CustomerState.MovingToExit:
                if (this.transform.position == CustomerGenerator.instance.spawnLocation.transform.position)
                {
                    GameObject.Destroy(this.gameObject);
                }
                break;
            default:
                break;
        }

        isDoingTask = false;
    }

    public void DecideOrder()
    {
        toPay = 0;
        claimedItems = new List<MenuItem>();
        currentOrder = new List<MenuItem>();

        // Decide order according to preferences & needs
        List<MenuItem> order = new List<MenuItem>();
        if (customerGoal == CustomerGoal.Hunger)
        {
            order = DetermineMealOrder();

            int thirstDiff = 100 - customerStats.thirst;
            if (thirstDiff >= 50)
            {
                order.AddRange(DetermineDrinkOrder());
            }
        }
        else if (customerGoal == CustomerGoal.Thirst)
        {
            order = DetermineDrinkOrder();

            int hungerDiff = 100 - customerStats.hunger;
            if (hungerDiff >= 50)
            {
                order.AddRange(DetermineMealOrder());
            }
        }

        // Add order to table for next order cycle
        if (order.Count > 0)
        {
            currentOrder = order;
            customerStats.assignedTable.GetComponent<Table>().nextOrder.AddRange(order);

            AssignNewState(CustomerState.WaitingOnOrder);
        }
        else
        {
            // Switch to waiting on order
            AssignNewGoal(CustomerGoal.None);
            AssignNewState(CustomerState.Idling);
        }
    }

    public void WaitingOnOrder()
    {
        // Check if order has been delivered
        List<MenuItem> currentOrderCopy = new List<MenuItem>();
        currentOrderCopy.AddRange(currentOrder);
        foreach (MenuItem item in currentOrderCopy)
        {
            if (customerStats.assignedTable.GetComponent<Table>().deliveredItems.Contains(item))
            {
                // Remove order from delivered list
                customerStats.assignedTable.GetComponent<Table>().deliveredItems.Remove(item);
                claimedItems.Add(item);
                currentOrder.Remove(item);

                // Add toPay
                toPay += item.cost;
            }
        }

        if (currentOrder.Count == 0)
        {
            // Pay
            Pay(toPay);

            // Update stats
            ServiceManager.instance.stats.AddToServedMenuItems(claimedItems);

            // Switch state to eating order
            AssignNewState(CustomerState.EatingOrder);
        }
    }
}
