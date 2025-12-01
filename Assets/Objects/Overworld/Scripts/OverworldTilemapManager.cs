using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OverworldTilemapManager : MonoBehaviour
{
    public static OverworldTilemapManager instance;

    public List<Vector3> tilePositionsWorld = new List<Vector3>();

    [Header("Tilemaps")]
    public Tilemap overworldFloorMap;
    public Tilemap overworldUnderMap;
    public Tilemap overworldZoningMap;

    [Header("Collisions")]
    public List<Vector3> overworldFloorBorders = new List<Vector3>();
    public Tilemap overworldCollisionMap;
    public RuleTile collisionTile;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        RemapOverworld();
    }

    public void RemapOverworld()
    {
        tilePositionsWorld.Clear();


        overworldCollisionMap.ClearAllTiles();

        GenerateFloorTiles();
        GenerateCollisions();

    }

    public void GenerateFloorTiles()
    {
        tilePositionsWorld.Clear();

        foreach (Vector3Int pos in overworldFloorMap.cellBounds.allPositionsWithin)
        {
            Vector3 place = overworldFloorMap.CellToWorld(pos);

            // Check if floormap has tile on that pos
            if (overworldFloorMap.HasTile(pos))
            {
                tilePositionsWorld.Add(pos);

                
            }
        }
    }

    public void GenerateCollisions()
    {
        foreach (Vector3 floorTilePos in tilePositionsWorld)
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
            if (!tilePositionsWorld.Contains(left))
            {
                if (!overworldFloorBorders.Contains(left))
                {
                    overworldFloorBorders.Add(left);
                    overworldCollisionMap.SetTile(new Vector3Int((int)left.x, (int)left.y), collisionTile);
                }
            }

            // Check right
            if (!tilePositionsWorld.Contains(right))
            {
                if (!overworldFloorBorders.Contains(right))
                {
                    overworldFloorBorders.Add(right);
                    overworldCollisionMap.SetTile(new Vector3Int((int)right.x, (int)right.y), collisionTile);
                }
            }

            // Check up
            if (!tilePositionsWorld.Contains(up))
            {
                if (!overworldFloorBorders.Contains(up))
                {
                    overworldFloorBorders.Add(up);
                    overworldCollisionMap.SetTile(new Vector3Int((int)up.x, (int)up.y), collisionTile);
                }
            }

            // Check down
            if (!tilePositionsWorld.Contains(down))
            {
                if (!overworldFloorBorders.Contains(down))
                {
                    overworldFloorBorders.Add(down);
                    overworldCollisionMap.SetTile(new Vector3Int((int)down.x, (int)down.y), collisionTile);
                }
            }

            // Check diagonals
            if (!tilePositionsWorld.Contains(topleft))
            {
                if (!overworldFloorBorders.Contains(topleft))
                {
                    overworldFloorBorders.Add(topleft);
                    overworldCollisionMap.SetTile(new Vector3Int((int)topleft.x, (int)topleft.y), collisionTile);
                }
            }
            if (!tilePositionsWorld.Contains(botleft))
            {
                if (!overworldFloorBorders.Contains(botleft))
                {
                    overworldFloorBorders.Add(botleft);
                    overworldCollisionMap.SetTile(new Vector3Int((int)botleft.x, (int)botleft.y), collisionTile);
                }
            }
            if (!tilePositionsWorld.Contains(topright))
            {
                if (!overworldFloorBorders.Contains(topright))
                {
                    overworldFloorBorders.Add(topright);
                    overworldCollisionMap.SetTile(new Vector3Int((int)topright.x, (int)topright.y), collisionTile);
                }
            }
            if (!tilePositionsWorld.Contains(botright))
            {
                if (!overworldFloorBorders.Contains(botright))
                {
                    overworldFloorBorders.Add(botright);
                    overworldCollisionMap.SetTile(new Vector3Int((int)botright.x, (int)botright.y), collisionTile);
                }
            }
        }
    }
}
