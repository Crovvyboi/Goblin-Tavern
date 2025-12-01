using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    public FurnitureDirection currentOrientation;

    public GameObject upSprite;
    public GameObject downSprite;
    public GameObject leftSprite;
    public GameObject rightSprite;

    public List<Vector3> tiles = new List<Vector3>();

    public InventoryItem inventoryItem;

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
                if (rightSprite.GetComponent<BoxCollider2D>() != null)
                {
                    rightSprite.GetComponent<BoxCollider2D>().enabled = false;
                }

                if (this.gameObject.TryGetComponent(typeof(Table), out Component tablecompr))
                {
                    Table table = tablecompr as Table;
                    table.orderReadyMarker = rightSprite.transform.Find("OrderReadySquare").gameObject;
                    table.orderHighlightMarker = rightSprite.transform.Find("OrderSelectedSquare").gameObject;
                }

                currentOrientation = FurnitureDirection.Right;
                break;
            case FurnitureDirection.Down:
                downSprite.gameObject.SetActive(false);

                leftSprite.gameObject.SetActive(true);
                if (leftSprite.GetComponent<BoxCollider2D>() != null)
                {
                    leftSprite.GetComponent<BoxCollider2D>().enabled = false;
                }

                if (this.gameObject.TryGetComponent(typeof(Table), out Component tablecompl))
                {
                    Table table = tablecompl as Table;
                    table.orderReadyMarker = leftSprite.transform.Find("OrderReadySquare").gameObject;
                    table.orderHighlightMarker = leftSprite.transform.Find("OrderSelectedSquare").gameObject;
                }

                currentOrientation = FurnitureDirection.Left;
                break;
            case FurnitureDirection.Left:
                leftSprite.gameObject.SetActive(false);

                upSprite.gameObject.SetActive(true);
                if (upSprite.GetComponent<BoxCollider2D>() != null)
                {
                    upSprite.GetComponent<BoxCollider2D>().enabled = false;
                }

                if (this.gameObject.TryGetComponent(typeof(Table), out Component tablecompu))
                {
                    Table table = tablecompu as Table;
                    table.orderReadyMarker = upSprite.transform.Find("OrderReadySquare").gameObject;
                    table.orderHighlightMarker = upSprite.transform.Find("OrderSelectedSquare").gameObject;
                }

                currentOrientation = FurnitureDirection.Up;
                break;
            case FurnitureDirection.Right:
                rightSprite.gameObject.SetActive(false);

                downSprite.gameObject.SetActive(true);
                if (downSprite.GetComponent<BoxCollider2D>() != null)
                {
                    downSprite.GetComponent<BoxCollider2D>().enabled = false;
                }

                if (this.gameObject.TryGetComponent(typeof(Table), out Component tablecompd))
                {
                    Table table = tablecompd as Table;
                    table.orderReadyMarker = downSprite.transform.Find("OrderReadySquare").gameObject;
                    table.orderHighlightMarker = downSprite.transform.Find("OrderSelectedSquare").gameObject;
                }

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
