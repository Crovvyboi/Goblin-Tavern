using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemDragger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int itemSizeTiles;
    public GameObject anchorPointHolder;
    public List<GameObject> anchorPoints = new List<GameObject>();

    public bool isPlaced;
    public ItemOrientation orientation;
    public Vector2 inventorySpotOrigin;

    // Start is called before the first frame update
    void Awake()
    {
        InitAnchorPoints();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public void InitAnchorPoints()
    {
        anchorPoints.Clear();
        for (int i = 0; i < anchorPointHolder.transform.childCount; i++)
        {
            anchorPoints.Add(anchorPointHolder.transform.GetChild(i).gameObject);
        }
    }

    public void OnClick()
    {
        if (!PlayerInventory.instance.isDraggingItem)
        {
            PlayerInventory.instance.DragItem(this.gameObject);
        }
        else
        {
            OnClickRelease();
        }
    }

    public void OnClickRelease()
    {
        PlayerInventory.instance.ReleaseItem();
    }

    public void OnRelease()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(gameObject.GetComponent<InventoryItemHolder>().item.inventoryItemName);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Exit");
    }

    public void PlaceItem(InventoryCell inventoryCell)
    {
        isPlaced = true;
        inventorySpotOrigin = inventoryCell.coords;

        SetOrientation();
    }
    public void SetOrientation()
    {
        switch (this.transform.rotation.z)
        {
            case 0:
                orientation = ItemOrientation.South;
                break;
            case 90:
                orientation = ItemOrientation.East;
                break;
            case 180:
                orientation = ItemOrientation.North;
                break;
            case 270:
                orientation = ItemOrientation.West;
                break;
            default:
                orientation = ItemOrientation.South;
                break;
        }
    }

    public void RemoveItem()
    {
        isPlaced = false;
        inventorySpotOrigin = Vector2.zero;
    }
}

public enum ItemOrientation
{
    North,  // 180
    East,   // 90
    South,  // 0, default
    West    // 270
}



