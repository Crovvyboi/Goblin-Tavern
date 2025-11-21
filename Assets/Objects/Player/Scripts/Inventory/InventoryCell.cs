using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCell : MonoBehaviour
{
    public bool activeCell;

    public Vector2 coords;

    public List<InventoryCell> neighborCells;
    public GameObject occupyingObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddNeigbor(InventoryCell cell)
    {
        neighborCells.Add(cell);
    }

    public Vector2 Get90Orientation()
    {
        return new Vector2(coords.y, -coords.x);
    }

    public Vector2 Get180Orientation()
    {
        return new Vector2(-coords.x, -coords.y);
    }

    public Vector2 Get270Orientation()
    {
        return new Vector2(-coords.y, coords.x);
    }
}
