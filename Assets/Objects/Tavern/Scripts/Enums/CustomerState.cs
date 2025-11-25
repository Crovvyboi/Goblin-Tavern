using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CustomerState
{
    Idling,
    Moving,
    Ordering,
    WaitingOnOrder,
    EatingOrder,
    MeetTarget,
    MovingToExit
}

public enum CustomerGoal
{
    None,

    Hunger,
    Thirst,

    Meet,
    Idle,

    Exit
}
