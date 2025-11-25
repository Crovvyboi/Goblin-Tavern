using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureOrientation : MonoBehaviour
{
    public FurnitureDirection currentOrientation;

    public GameObject upSprite;
    public GameObject downSprite;
    public GameObject leftSprite;
    public GameObject rightSprite;

    public List<Vector3> tiles = new List<Vector3>();

    private void Start()
    {
        FindTiles();
    }
    public void FindTiles()
    {
        switch (currentOrientation)
        {
            case FurnitureDirection.Up:
                foreach (Transform child in upSprite.transform.Find("Tiles").transform)
                {
                    tiles.Add(child.position);
                }
                break;
            case FurnitureDirection.Down:
                foreach (Transform child in downSprite.transform.Find("Tiles").transform)
                {
                    tiles.Add(child.position);
                }
                break;
            case FurnitureDirection.Left:
                foreach (Transform child in leftSprite.transform.Find("Tiles").transform)
                {
                    tiles.Add(child.position);
                }
                break;
            case FurnitureDirection.Right:
                foreach (Transform child in rightSprite.transform.Find("Tiles").transform)
                {
                    tiles.Add(child.position);
                }
                break;
            default:
                break;
        }
    }

    public void RotateRight()
    {

    }

    public void RotateLeft()
    {

    }
}

public enum FurnitureDirection
{
    Up,
    Down, 
    Left, 
    Right
}
