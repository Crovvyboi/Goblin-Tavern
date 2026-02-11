using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionReceiver : MonoBehaviour
{
    public UnityEvent interaction;
    public void OnInteract(InteractionSender sender)
    {
        interaction.Invoke();

        sender.canInteract = true;
        
        
    }
}
