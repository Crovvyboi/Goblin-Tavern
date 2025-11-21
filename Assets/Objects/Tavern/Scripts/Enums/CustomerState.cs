using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CustomerState
{
    Idling,
    MovingToTable,
    Ordering,
    WaitingOnOrder,
    EatingOrder,
    MovingToExit
}

public enum CustomerGoal
{
    None,
    Hunger,
    Thirst,
    Exit
}
