using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathfinderScript : MonoBehaviour
{
    public static PathfinderScript instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public List<Node> FindPathFromPosToPos(Vector3 startObject, Vector3 endObject)
    {
        Node startNode = TavernTilemapManager.instance.tavernFloorNodes.First(x => x.transform.position == startObject);
        Node endNode = TavernTilemapManager.instance.tavernFloorNodes.First(x => x.transform.position == endObject);

        return FindPathFromNodeToNode(startNode, endNode);
    }

    public List<Node> FindPathFromObjectToObject(GameObject startObject, GameObject endObject)
    {
        Node startNode = TavernTilemapManager.instance.tavernFloorNodes.First(x => x.transform.position == startObject.transform.position);
        Node endNode = TavernTilemapManager.instance.tavernFloorNodes.First(x => x.transform.position == endObject.transform.position);

        return FindPathFromNodeToNode(startNode, endNode);
    }

    public List<Node> FindPathFromNodeToNode(Node startNode, Node endNode)
    {
        return FindPath(startNode, endNode);
    }

    public List<Node> FindPath(Node start, Node end)
    {
        // Finding path using the A Star method

        List<Node> openSet = new List<Node>();

        foreach (Node n in FindObjectsOfType<Node>())
        {
            n.gScore = float.MaxValue;
        }

        start.gScore = 0;
        start.hScore = Vector2.Distance(start.transform.position, end.transform.position);
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            int lowestF = default;

            for (int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].fScore() < openSet[lowestF].fScore())
                {
                    lowestF = i;
                }
            }

            Node currentNode = openSet[lowestF];
            openSet.Remove(currentNode);

            if (currentNode == end)
            {
                List<Node> path = new List<Node>();
                path.Insert(0, end);
                while (currentNode != start)
                {
                    currentNode = currentNode.previousNode;
                    path.Add(currentNode);
                }
                path.Reverse();
                return path;
            }

            foreach (Node connectedNode in currentNode.connections)
            {
                float heldGScore = currentNode.gScore + Vector2.Distance(currentNode.transform.position, connectedNode.transform.position);

                if (heldGScore < connectedNode.gScore)
                {
                    connectedNode.previousNode = currentNode;
                    connectedNode.gScore = heldGScore;
                    connectedNode.hScore = Vector2.Distance(connectedNode.transform.position, end.transform.position);

                    if (!openSet.Contains(connectedNode))
                    {
                        openSet.Add(connectedNode);
                    }
                }
            }
        }

        return null;
    }

    public Node FindThroughpoint(Vector3 goal, Vector3 currentPos)
    {
        // Find center
        Vector3 center = Vector3.Lerp(goal, currentPos, 0.5f);
        float distanceFromCenter = Vector3.Distance(center, goal) / 1.7f;

        // Get points in ellipse
        List<Node> foundPoints = FindObjectsOfType<Node>().Where(x => Vector3.Distance(center, x.transform.position) <= distanceFromCenter).ToList();

        return foundPoints[Random.Range(0, foundPoints.Count - 1)];
    }
}
