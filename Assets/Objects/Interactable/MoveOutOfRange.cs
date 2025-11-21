using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveOutOfRange : MonoBehaviour
{
    public UnityEvent interaction;

    private void OnTriggerExit2D(Collider2D collision)
    {
        interaction.Invoke();
    }
}
