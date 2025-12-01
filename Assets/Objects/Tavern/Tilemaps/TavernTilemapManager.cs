using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TavernTilemapManager : MonoBehaviour
{
    public static TavernTilemapManager instance;

    public GameObject nodePrefab;

    public List<Vector3> tilePostitionsWorld = new List<Vector3>();
    public List<Node> tavernFloorNodes = new List<Node>();
    public List<Node> hangoutSpots = new List<Node>();

    [Header("Tilemaps")]
    public Tilemap tavernFloorMap;
    public Tilemap tavernWallMap;
    public Tilemap tavernZoningMap;

    [Header("Collisions")]
    public List<Vector3> tavernFloorBorders = new List<Vector3>();
    public Tilemap tavernCollisionMap;
    public RuleTile collisionTile;

    [Header("Tavern Furniture")]
    public GameObject tavernFurnitureContainer;
    public List<Vector3> tavernFurnitureTiles;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        RemapTavern();
    }


    public void RemapTavern()
    {
        tilePostitionsWorld.Clear();
        tavernFloorNodes.Clear();
        tavernFloorBorders.Clear();

        tavernCollisionMap.ClearAllTiles();

        GenerateFloorNodes();
        GenerateWallCollisions();

        GenerateFurnitureTiles();
        GenerateHangoutSpots();
    }

    public void GenerateFloorNodes()
    {
        foreach (Node item in tavernFloorNodes)
        {
            Destroy(item.gameObject);
        }

        tilePostitionsWorld.Clear();
        tavernFloorNodes.Clear();

        GenerateFurnitureTiles();
        GenerateHangoutSpots();
        for (int i = tavernFloorMap.cellBounds.xMin; i < tavernFloorMap.cellBounds.xMax; i++)
        {
            for (int j = tavernFloorMap.cellBounds.yMin; j < tavernFloorMap.cellBounds.yMax; j++)
            {
                Vector3Int localPlace = (new Vector3Int(i, j, (int)tavernFloorMap.transform.position.y));
                Vector3 place = tavernFloorMap.CellToWorld(localPlace);

                // Check if floormap has tile on that pos
                if (tavernFloorMap.HasTile(localPlace))
                {
                    tilePostitionsWorld.Add(place);

                    // Check if furnituremap does not have tile on that pos
                    if (!CheckIfPositionHasFurniture(localPlace))
                    {
                        GameObject newNode = GameObject.Instantiate(nodePrefab);
                        newNode.transform.SetParent(tavernFloorMap.gameObject.transform, false);
                        newNode.transform.position = new Vector3(place.x + 0.5f, place.y + 0.5f);
                        tavernFloorNodes.Add(newNode.GetComponent<Node>());
                    }
                }
            }
        }

        // Connect nodes
        foreach (Node node in tavernFloorNodes)
        {
            ConnectNode(node); 
        }
    }

    public void ConnectNode(Node node)
    {
        Vector3 nodePos = node.gameObject.transform.position;

        if (tavernFloorNodes.Where(x => x.transform.position.y == nodePos.y).Any(x => x.transform.position.x == nodePos.x - 1f))
        {
            Node newNode = tavernFloorNodes.Where(x => x.transform.position.y == nodePos.y).First(x => x.transform.position.x == nodePos.x - 1f);
            node.connections.Add(newNode);
        }
        if (tavernFloorNodes.Where(x => x.transform.position.y == nodePos.y).Any(x => x.transform.position.x == nodePos.x + 1f))
        {
            Node newNode = tavernFloorNodes.Where(x => x.transform.position.y == nodePos.y).First(x => x.transform.position.x == nodePos.x + 1f);
            node.connections.Add(newNode);
        }
        if (tavernFloorNodes.Where(x => x.transform.position.x == nodePos.x).Any(x => x.transform.position.y == nodePos.y + 1f))
        {
            Node newNode = tavernFloorNodes.Where(x => x.transform.position.x == nodePos.x).First(x => x.transform.position.y == nodePos.y + 1f);
            node.connections.Add(newNode);
        }
        if (tavernFloorNodes.Where(x => x.transform.position.x == nodePos.x).Any(x => x.transform.position.y == nodePos.y - 1f))
        {
            Node newNode = tavernFloorNodes.Where(x => x.transform.position.x == nodePos.x).First(x => x.transform.position.y == nodePos.y - 1f);
            node.connections.Add(newNode);
        }
    }

    public bool CheckIfPositionHasFurniture(Vector3 position)
    {
        if (tavernFurnitureTiles.Any(x => Vector3.Distance(new Vector3(x.x - 0.5f, x.y - 0.5f), position) < 0.75f))
        {
            return true;
        }
        return false;
    }

    public bool CheckIfPositionHasTavernTile(List<Vector3> positions)
    {
        foreach (Vector3 position in positions)
        {
            if (!tilePostitionsWorld.Any(x => Vector3.Distance(new Vector3(x.x + 0.5f, x.y + 0.5f, 0), position) < 0.1f))
            {
                return false;
            }
        }
        return true;
    }

    public bool CheckIfPositionHasNode(Vector3 position)
    {
        if (!tavernFloorNodes.Any(x => Vector3.Distance(new Vector3(x.transform.position.x, x.transform.position.y, 0), position) < 0.1f))
        {
            return false;
        }
        return true;
    }

    public bool CanPlaceFurniture(List<Vector3> positions)
    {
        foreach (Vector3 position in positions)
        {
            if (tavernFurnitureTiles.Any(x => Vector3.Distance(new Vector3(x.x, x.y), position) < 0.1f))
            {
                return false;
            }
        }
        return true;
    }

    public void GenerateFurnitureTiles()
    {
        tavernFurnitureTiles = new List<Vector3>();

        foreach (Furniture tile in tavernFurnitureContainer.GetComponentsInChildren<Furniture>())
        {
            tavernFurnitureTiles.AddRange(tile.tiles);
        }
    }
    
    public Vector3? GetClosestPos(Vector3 mousepos)
    {
        if (tilePostitionsWorld.Any(x => Vector3.Distance(x, mousepos) < 1f))
        {
            return tilePostitionsWorld.First(x => Vector3.Distance(x, mousepos) < 1f);
        }
        return null;
    }

    public GameObject? GetFurnitureOnPos(Vector3 position)
    {
        foreach (Furniture tile in tavernFurnitureContainer.GetComponentsInChildren<Furniture>())
        {
            if (tile.tiles.Any(x => Vector3.Distance(x, position) < 0.5f))
            {
                return tile.gameObject;
            }
        }
        return null;
    }

    public void OnFurnitureAdd(List<Vector3> tiles)
    {
        foreach (Vector3 tile in tiles)
        {
            Vector3Int localPlace = (new Vector3Int(Convert.ToInt32(tile.x - 0.5f), Convert.ToInt32(tile.y - 0.5f), (int)tavernFloorMap.transform.position.y));
            Vector3 place = tavernFloorMap.CellToWorld(localPlace);

            if (tavernFloorNodes.Any(x => x.transform.position == new Vector3(place.x + 0.5f, place.y + 0.5f)))
            {
                Node node = tavernFloorNodes.First(x => x.transform.position == new Vector3(place.x + 0.5f, place.y + 0.5f));
                tavernFloorNodes.Remove(node);

                List<Node> nodes = tavernFloorNodes.Where(x => x.connections.Contains(node)).ToList();
                foreach (Node item in nodes)
                {
                    item.connections.Remove(node);
                }

                Destroy(node.gameObject);
            }

            tavernFurnitureTiles.Add(tile);
        }
    }
    public void OnFurnitureRemove(List<Vector3> tiles)
    {
        foreach (Vector3 tile in tiles)
        {
            Vector3Int localPlace = (new Vector3Int(Convert.ToInt32(tile.x - 0.5f), Convert.ToInt32(tile.y - 0.5f), (int)tavernFloorMap.transform.position.y));
            Vector3 place = tavernFloorMap.CellToWorld(localPlace);
            // Check if floormap has tile on that pos
            if (tavernFloorMap.HasTile(localPlace))
            {
                GameObject newNode = GameObject.Instantiate(nodePrefab);
                newNode.transform.SetParent(tavernFloorMap.gameObject.transform, false);
                newNode.transform.position = new Vector3(place.x + 0.5f, place.y + 0.5f);
                tavernFloorNodes.Add(newNode.GetComponent<Node>());

                ConnectNode(newNode.GetComponent<Node>());
               
            }

            tavernFurnitureTiles.Remove(tile);
        }
    }

    public void GenerateWallCollisions()
    {
        foreach (Vector3 floorTilePos in tilePostitionsWorld)
        {
            Vector3 left = new Vector3(floorTilePos.x - 1, floorTilePos.y, floorTilePos.z);
            Vector3 right = new Vector3(floorTilePos.x + 1, floorTilePos.y, floorTilePos.z);
            Vector3 up = new Vector3(floorTilePos.x, floorTilePos.y + 1, floorTilePos.z);
            Vector3 down = new Vector3(floorTilePos.x, floorTilePos.y - 1, floorTilePos.z);

            Vector3 botleft = new Vector3(floorTilePos.x - 1, floorTilePos.y - 1, floorTilePos.z);
            Vector3 topleft = new Vector3(floorTilePos.x - 1, floorTilePos.y + 1, floorTilePos.z);
            Vector3 botright = new Vector3(floorTilePos.x + 1, floorTilePos.y - 1, floorTilePos.z);
            Vector3 topright = new Vector3(floorTilePos.x + 1, floorTilePos.y + 1, floorTilePos.z);

            // Check left
            if (!tilePostitionsWorld.Contains(left))
            {
                if (!tavernFloorBorders.Contains(left))
                {
                    tavernFloorBorders.Add(left);
                    tavernCollisionMap.SetTile(new Vector3Int((int)left.x, (int)left.y), collisionTile);
                }
            }

            // Check right
            if (!tilePostitionsWorld.Contains(right))
            {
                if (!tavernFloorBorders.Contains(right))
                {
                    tavernFloorBorders.Add(right);
                    tavernCollisionMap.SetTile(new Vector3Int((int)right.x, (int)right.y), collisionTile);
                }
            }

            // Check up
            if (!tilePostitionsWorld.Contains(up))
            {
                if (!tavernFloorBorders.Contains(up))
                {
                    tavernFloorBorders.Add(up);
                    tavernCollisionMap.SetTile(new Vector3Int((int)up.x, (int)up.y), collisionTile);
                }
            }

            // Check down
            if (!tilePostitionsWorld.Contains(down))
            {
                if (!tavernFloorBorders.Contains(down))
                {
                    tavernFloorBorders.Add(down);
                    tavernCollisionMap.SetTile(new Vector3Int((int)down.x, (int)down.y), collisionTile);
                }
            }

            // Check diagonals
            if (!tilePostitionsWorld.Contains(topleft))
            {
                if (!tavernFloorBorders.Contains(topleft))
                {
                    tavernFloorBorders.Add(topleft);
                    tavernCollisionMap.SetTile(new Vector3Int((int)topleft.x, (int)topleft.y), collisionTile);
                }
            }
            if (!tilePostitionsWorld.Contains(botleft))
            {
                if (!tavernFloorBorders.Contains(botleft))
                {
                    tavernFloorBorders.Add(botleft);
                    tavernCollisionMap.SetTile(new Vector3Int((int)botleft.x, (int)botleft.y), collisionTile);
                }
            }
            if (!tilePostitionsWorld.Contains(topright))
            {
                if (!tavernFloorBorders.Contains(topright))
                {
                    tavernFloorBorders.Add(topright);
                    tavernCollisionMap.SetTile(new Vector3Int((int)topright.x, (int)topright.y), collisionTile);
                }
            }
            if (!tilePostitionsWorld.Contains(botright))
            {
                if (!tavernFloorBorders.Contains(botright))
                {
                    tavernFloorBorders.Add(botright);
                    tavernCollisionMap.SetTile(new Vector3Int((int)botright.x, (int)botright.y), collisionTile);
                }
            }
        }
    }

    public void GenerateHangoutSpots()
    {
        // A hangout spot is a spot in the tavern where a group of 4 can stand in a circle
        hangoutSpots = new List<Node>();
        foreach (Node node in tavernFloorNodes)
        {
            if (node.connections.Count == 4)
            {
                hangoutSpots.Add(node);
            }
        }

    }

    public Node AssignHangoutSpot(List<Node> occupiedHangouts)
    {
        for (int i = 0; i < 5; i++)
        {
            Node hangout = hangoutSpots[Random.Range(0, hangoutSpots.Count - 1)];
            if (hangout != null && !occupiedHangouts.Contains(hangout)) 
            {
                if (occupiedHangouts.Count == 0 || occupiedHangouts.Any(x => Vector3.Distance(x.transform.position, hangout.transform.position) > 2))
                {
                    foreach (Node connections in hangout.connections)
                    {
                        if (!occupiedHangouts.Contains(connections) && !occupiedHangouts.Any(x => x.connections.Contains(connections)))
                        {
                            return hangout;
                        }
                    }
                }
                
            }
        }

        foreach (Node hangout in hangoutSpots)
        {
            if (!occupiedHangouts.Contains(hangout))
            {
                if (occupiedHangouts.Count == 0 || occupiedHangouts.Any(x => Vector3.Distance(x.transform.position, hangout.transform.position) > 2))
                {
                    foreach (Node connections in hangout.connections)
                    {
                        if (!occupiedHangouts.Contains(connections) && !occupiedHangouts.Any(x => x.connections.Contains(connections)))
                        {
                            return hangout;
                        }
                    }
                }
            }
        }
        return null;
        
    }
}
