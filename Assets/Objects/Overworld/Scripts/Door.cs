using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject teleportToDropoff;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (teleportToDropoff != null)
        {
            collision.gameObject.transform.position = teleportToDropoff.transform.position;
        }
    }
}
