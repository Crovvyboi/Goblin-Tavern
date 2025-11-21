using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node previousNode;

    public List<Node> connections;

    // How many moves it took to get to this node
    public float gScore;

    // Estimated distance from this node to end node
    public float hScore;

    public float fScore()
    {
        return gScore + hScore;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        if (connections.Count > 0)
        {
            foreach (Node node in connections)
            {
                Gizmos.DrawLine(transform.position, node.transform.position);
            }
        }
    }
}
