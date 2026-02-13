using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemDragger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int itemSizeTiles;
    public GameObject anchorPointHolder;
    public List<GameObject> anchorPoints = new List<GameObject>();
    
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
}



