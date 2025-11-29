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
        tiles.Clear();
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

    public List<Vector3> GetTiles()
    {
        FindTiles();
        return tiles;
    }

    public void RotateRight()
    {
        switch (currentOrientation)
        {
            case FurnitureDirection.Up:
                upSprite.gameObject.SetActive(false);

                rightSprite.gameObject.SetActive(true);
                rightSprite.GetComponent<BoxCollider2D>().enabled = false;
                currentOrientation = FurnitureDirection.Right;
                break;
            case FurnitureDirection.Down:
                downSprite.gameObject.SetActive(false);

                leftSprite.gameObject.SetActive(true);
                leftSprite.GetComponent<BoxCollider2D>().enabled = false;
                currentOrientation = FurnitureDirection.Left;
                break;
            case FurnitureDirection.Left:
                leftSprite.gameObject.SetActive(false);

                upSprite.gameObject.SetActive(true);
                upSprite.GetComponent<BoxCollider2D>().enabled = false;
                currentOrientation = FurnitureDirection.Up;
                break;
            case FurnitureDirection.Right:
                rightSprite.gameObject.SetActive(false);

                downSprite.gameObject.SetActive(true);
                downSprite.GetComponent<BoxCollider2D>().enabled = false;
                currentOrientation = FurnitureDirection.Down;
                break;
            default:
                break;
        }
    }
}

public enum FurnitureDirection
{
    Up,
    Down, 
    Left, 
    Right
}
