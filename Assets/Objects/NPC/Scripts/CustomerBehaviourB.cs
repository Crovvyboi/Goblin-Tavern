using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomerBehaviourB : CustomerBase
{
    /** 
     * This is the customer AI for archetype B.
     * 
     * The customer will be able to order drinks at the bar. The customer does not get hungry.
     * When an order is placed, the customer will wait at the bar. If someone is already waiting, wait in line.
    **/

    public GameObject barWaitingSpot;
    public bool hasOrdered;
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
        if (customerState != CustomerState.EatingOrder && customerState != CustomerState.MovingToExit)
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


        customerStats.thirst -= Random.Range(1, 5);
        if (customerStats.thirst < 0)
        {
            customerStats.thirst = 0;
        }

        if (customerState == CustomerState.Idling)
        {
            MakeDecision();
        }
    }

    public void MakeDecision()
    {
        if (customerGoal == CustomerGoal.None)
        {
            Dictionary<CustomerGoal, int> inRange = new Dictionary<CustomerGoal, int>();

            int thirstDiff = 100 - customerStats.thirst;
            inRange.Add(CustomerGoal.Thirst, thirstDiff);

            // inRange.Add(CustomerGoal.Meet, 15);
            inRange.Add(CustomerGoal.Idle, 15);

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
            case CustomerGoal.Thirst:
                
                if (Bar.instance.OccupySpot(this.gameObject, out GameObject freeSpot))
                {
                    barWaitingSpot = freeSpot;
                    customerGoal = CustomerGoal.Thirst;
                    if (this.transform.position != barWaitingSpot.transform.position)
                    {
                        List<Vector3> newQueue = new List<Vector3>();
                        Vector3 throughPoint = FindThroughpoint(barWaitingSpot.transform.position, this.transform.position).transform.position;
                        if (throughPoint != null)
                        {
                            newQueue.Add(throughPoint);
                        }
                        newQueue.Add(barWaitingSpot.transform.position);
                        InjectNewQueue(newQueue);
                    }
                    hasOrdered = false;
                    customerState = CustomerState.Moving;
                }

                break;
            case CustomerGoal.Meet:
                break;
            case CustomerGoal.Idle:
                customerState = CustomerState.Idling;
                break;
            case CustomerGoal.Exit:
                InjectNewGoalPosition(CustomerGenerator.instance.spawnLocation.transform.position);
                customerState = CustomerState.MovingToExit;
                break;
            default:
                customerState = CustomerState.Idling;
                break;
        }
    }

    public void DoTask()
    {
        isDoingTask = true;

        switch (customerState)
        {
            case CustomerState.Idling:
                TargetedIdle();
                break;
            case CustomerState.Moving:
                switch (customerGoal)
                {
                    case CustomerGoal.Thirst:
                        if (!hasOrdered)
                        {
                            // Moved to the bar to order items
                            if (this.transform.position == barWaitingSpot.transform.position)
                            {
                                customerState = CustomerState.Ordering;
                            }
                        }
                        else
                        {
                            // Has moved to standing spot to consume items
                            if (this.transform.position == customerStats.standingSpot.transform.position)
                            {
                                customerState = CustomerState.EatingOrder;
                            }
                        }
                        break;
                    case CustomerGoal.Meet:
                        break;
                    default:
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
                    customerState = CustomerState.Idling;
                    customerGoal = CustomerGoal.None;
                }
                break;
            case CustomerState.MovingToExit:
                if (this.transform.position == CustomerGenerator.instance.spawnLocation.transform.position)
                {
                    GameObject.Destroy(this.gameObject);
                }
                break;
            default:
                TargetedIdle();
                break;
        }

        isDoingTask = false;
    }

    public void TargetedIdle()
    {
        // Idle at specific spot
        customerGoal = CustomerGoal.None;
        customerState = CustomerState.Idling;

        // Go to meeting spot
        if (customerStats.standingSpot  == null || this.currentPos != customerStats.standingSpot.transform.position)
        {
            // Move to range
            List<Vector3> newQueue = new List<Vector3>();
            Vector3 throughPoint = FindThroughpoint(customerStats.standingSpot.transform.position, this.transform.position).transform.position;
            if (throughPoint != null)
            {
                newQueue.Add(throughPoint);
            }
            newQueue.Add(customerStats.standingSpot.transform.position);
            InjectNewQueue(newQueue);
            customerState = CustomerState.Moving;

        }
    }

    public void DecideOrder()
    {
        toPay = 0;
        claimedItems = new List<MenuItem>();
        currentOrder = new List<MenuItem>();

        currentOrder = DetermineDrinkOrder();
        Bar.instance.barOrders.AddRange(currentOrder);

        customerState = CustomerState.WaitingOnOrder;
    }

    public void WaitingOnOrder()
    {
        // Check if order has been delivered
        List<MenuItem> currentOrderCopy = new List<MenuItem>();
        currentOrderCopy.AddRange(currentOrder);
        foreach (MenuItem item in currentOrderCopy)
        {
            if (Bar.instance.madeOrders.Contains(item))
            {
                // Remove order from delivered list
                Bar.instance.madeOrders.Remove(item);
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

            // Move to standing spot
            Bar.instance.LeaveSpot(this.gameObject);
            if (TavernManager.state != TavernState.ServiceFinalCall || TavernManager.state != TavernState.ServiceOverview)
            {
                if (this.transform.position != customerStats.standingSpot.transform.position)
                {
                    List<Vector3> newQueue = new List<Vector3>();
                    Vector3 throughPoint = FindThroughpoint(customerStats.standingSpot.transform.position, this.transform.position).transform.position;
                    if (throughPoint != null)
                    {
                        newQueue.Add(throughPoint);
                    }
                    newQueue.Add(customerStats.standingSpot.transform.position);
                    InjectNewQueue(newQueue);
                }
                hasOrdered = true;
                customerState = CustomerState.Moving;
            }
            else
            {
                customerState = CustomerState.EatingOrder;
            }
            
        }
    }
}
