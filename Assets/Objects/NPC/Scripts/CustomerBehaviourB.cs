using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerBehaviourB : CustomerBase
{
    /** 
     * This is the customer AI for archetype B.
     * 
     * The customer will be able to order drinks at the bar. The customer does not get hungry.
     * When an order is placed, the customer will wait at the bar. If someone is already waiting, wait in line.
    **/

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

            inRange.Add(CustomerGoal.Meet, 15);
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
            case CustomerGoal.None:
                break;
            case CustomerGoal.Thirst:
                break;
            case CustomerGoal.Meet:
                break;
            case CustomerGoal.Idle:
                TargetedIdle();
                break;
            case CustomerGoal.Exit:
                if (this.transform.position != customerStats.assignedChair.transform.position)
                {
                    InjectNewGoalPosition(CustomerGenerator.instance.spawnLocation.transform.position);
                }
                customerState = CustomerState.MovingToExit;
                break;
            default:
                TargetedIdle();
                break;
        }
    }

    public void DoTask()
    {

    }
}
