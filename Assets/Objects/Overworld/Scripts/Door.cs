using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject teleportToDropoff;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (teleportToDropoff != null && collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.position = new Vector3(teleportToDropoff.transform.position.x, teleportToDropoff.transform.position.y);
        }
    }
}
