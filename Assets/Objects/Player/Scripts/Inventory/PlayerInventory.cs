using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    public PlayerControls playerControls;

    public GameObject cellHolder;
    public List<InventoryCell> cells = new List<InventoryCell>();

    public GameObject itemHolder;
    public List<GameObject> itemsInInventory = new List<GameObject>();

    public float placementMargin = 0.40f;

    public bool isDraggingItem;
    public GameObject draggingItem;
    public List<InventoryCell> draggedItemCells;
    public Vector2 draggedItemPos;
    public Quaternion draggedItemRot;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        playerControls = new PlayerControls();

        InitAllInventoryItems();
        InitAllInventoryCells();
    }
    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Menu.InventoryRotateItem.performed += RotateItem;
        playerControls.Menu.HotbarSelectByKey.performed += AssignToHotbar;
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (draggingItem && draggingItem != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            draggingItem.transform.position = mousePos;
        }
    }

    public void InitAllInventoryItems()
    {
        for (int i = 0; i < itemHolder.transform.childCount; i++)
        {
            itemsInInventory.Add(itemHolder.transform.GetChild(i).gameObject);
        }
    }

    public void InitAllInventoryCells()
    {
        for (int i = 0; i < cellHolder.transform.childCount; i++)
        {
            InventoryCell cell = cellHolder.transform.GetChild(i).GetComponent<InventoryCell>();
            cells.Add(cell);
        }

        foreach (InventoryCell cell in cells)
        {
            cell.FindCoords(cells);
        }

        // Assign neighbors to all cells
        foreach (InventoryCell cell in cells)
        {
            // left
            if (cells.Any(x => x.coords.x == cell.coords.x - 1 && x.coords.y == cell.coords.y))
            {
                cell.AddNeigbor(cells.First(x => x.coords.x == cell.coords.x - 1 && x.coords.y == cell.coords.y));
            }

            // right
            if (cells.Any(x => x.coords.x == cell.coords.x + 1 && x.coords.y == cell.coords.y))
            {
                cell.AddNeigbor(cells.First(x => x.coords.x == cell.coords.x + 1 && x.coords.y == cell.coords.y));
            }

            // up
            if (cells.Any(x => x.coords.x == cell.coords.x && x.coords.y == cell.coords.y - 1))
            {
                cell.AddNeigbor(cells.First(x => x.coords.x == cell.coords.x && x.coords.y == cell.coords.y - 1));
            }

            // down
            if (cells.Any(x => x.coords.x == cell.coords.x && x.coords.y == cell.coords.y + 1))
            {
                cell.AddNeigbor(cells.First(x => x.coords.x == cell.coords.x && x.coords.y == cell.coords.y + 1));
            }

            // Get occupied object 
            // ! MAKE SURE GRID LAYOUT IS TURNED OFF !

            // Debug.Log($"Cell - x: {cell.gameObject.transform.position.x}, y: {cell.gameObject.transform.position.y}");
            float cellx = cell.gameObject.transform.position.x;
            float celly = cell.gameObject.transform.position.y;

            foreach (GameObject item in itemsInInventory)
            {
                //Debug.Log($"Anchor - x: {anchor.gameObject.transform.position.x}, y: {anchor.gameObject.transform.position.y}");
                float itemx = item.gameObject.transform.position.x;
                float itemy = item.gameObject.transform.position.y;

                // Center item on cell
                if (Mathf.Abs(cellx - itemx) < 1f && Mathf.Abs(celly - itemy) < 1f)
                {
                    item.transform.position = new Vector3(cellx, celly);
                }

                item.GetComponent<InventoryItemDragger>().InitAnchorPoints();
                // Assign item to covered cells
                foreach (GameObject anchor in item.GetComponent<InventoryItemDragger>().anchorPoints)
                {
                    float anchorx = anchor.transform.position.x;
                    float anchory = anchor.transform.position.y;

                    if (Mathf.Abs(cellx - anchorx) < 1f && Mathf.Abs(celly - anchory) < 1f)
                    {
                        cell.occupyingObject = item;
                    }
                }
            }
        }
    }

    public int CountItem(InventoryItem item)
    {
        int count = 0;
        if (item.containerItem)
        {
            List<GameObject> containers = itemsInInventory.Where(x => x.GetComponent<InventoryItemContainer>() != null && x.GetComponent<InventoryItemContainer>().containerType == item.containerType).ToList();
            foreach (GameObject container in containers)
            {
                count += container.GetComponent<InventoryItemContainer>().itemsInContainer.Where(x => x == item).Count();
            }
        }
        else
        {
            count += itemsInInventory.Where(x => x.GetComponent<InventoryItemHolder>() != null && x.GetComponent<InventoryItemHolder>().item == item).Count();
        }
        return count;
    }
    public int CountItem(IngredientType item)
    {
        int count = 0;

        List<GameObject> containers = itemsInInventory.Where(x => x.GetComponent<InventoryItemContainer>() != null && x.GetComponent<InventoryItemContainer>().itemsInContainer.Any(y => y.ingredientType == item)).ToList();
        foreach (GameObject container in containers)
        {
            count += container.GetComponent<InventoryItemContainer>().itemsInContainer.Where(x => x.ingredientType == item).Count();
        }
        count += itemsInInventory.Where(x => x.GetComponent<InventoryItemHolder>() != null && x.GetComponent<InventoryItemHolder>().item.ingredientType == item).Count();
        
        return count;
    }


    #region Item interaction
    public void DragItem(GameObject dragItem)
    {
        draggedItemPos = dragItem.transform.position;
        draggedItemRot = dragItem.transform.rotation;
        isDraggingItem = true;
        draggingItem = dragItem;

        // Decouple item from cells
        draggedItemCells = new List<InventoryCell>();
        draggedItemCells = cells.Where(x => x.occupyingObject == draggingItem).ToList();
        foreach (InventoryCell item in draggedItemCells)
        {
            item.occupyingObject = null;
        }
    }

    public void ReleaseItem()
    {
        bool found = cells.Any(x => Mathf.Abs(x.gameObject.transform.position.x - draggingItem.transform.position.x) < 1f && Mathf.Abs(x.gameObject.transform.position.y - draggingItem.transform.position.y) < 1f);
        if (found)
        {
            bool checkCell = cells.Any(x => Mathf.Abs(x.gameObject.transform.position.x - draggingItem.transform.position.x) < 1f && Mathf.Abs(x.gameObject.transform.position.y - draggingItem.transform.position.y) < 1f && x.occupyingObject == null);
            if (checkCell)
            {
                PlaceItem();
            }

        }
        else
        {
            ResetItem();
        }
    }

    public void PlaceItem()
    {
        if (cells.Any(x => Mathf.Abs(x.gameObject.transform.position.x - draggingItem.transform.position.x) < placementMargin && Mathf.Abs(x.gameObject.transform.position.y - draggingItem.transform.position.y) < placementMargin && x.occupyingObject == null))
        {
            GameObject foundCell = cells.First(x => Mathf.Abs(x.gameObject.transform.position.x - draggingItem.transform.position.x) < placementMargin && Mathf.Abs(x.gameObject.transform.position.y - draggingItem.transform.position.y) < placementMargin && x.occupyingObject == null).gameObject;

            // Check if anchor points aren't obstructed
            int count = 0;
            foreach (GameObject anchor in draggingItem.GetComponent<InventoryItemDragger>().anchorPoints)
            {
                float anchorx = anchor.transform.position.x;
                float anchory = anchor.transform.position.y;

                bool foundCellAnchorPoint = cells.Any(x => Mathf.Abs(x.gameObject.transform.position.x - anchorx) < placementMargin && Mathf.Abs(x.gameObject.transform.position.y - anchory) < placementMargin && x.occupyingObject == null);

                if (foundCellAnchorPoint)
                {
                    count++;
                }
            }

            if (count == draggingItem.GetComponent<InventoryItemDragger>().anchorPoints.Count)
            {
                draggingItem.transform.position = foundCell.transform.position;
                // Couple to cells
                foreach (GameObject anchor in draggingItem.GetComponent<InventoryItemDragger>().anchorPoints)
                {
                    float anchorx = anchor.transform.position.x;
                    float anchory = anchor.transform.position.y;

                    bool foundCellCheck = cells.Any(x => Mathf.Abs(x.gameObject.transform.position.x - anchorx) < placementMargin && Mathf.Abs(x.gameObject.transform.position.y - anchory) < placementMargin && x.occupyingObject == null);
                    if (foundCellCheck)
                    {
                        InventoryCell foundCellAnchorPoint = cells.First(x => Mathf.Abs(x.gameObject.transform.position.x - anchorx) < placementMargin && Mathf.Abs(x.gameObject.transform.position.y - anchory) < placementMargin && x.occupyingObject == null);
                        foundCellAnchorPoint.occupyingObject = draggingItem;
                    }

                }

                isDraggingItem = false;
                draggingItem = null;
                draggedItemPos = Vector2.zero;
            }
        }
    }

    public void ResetItem()
    {
        draggingItem.transform.position = draggedItemPos;
        draggingItem.transform.rotation = draggedItemRot;
        // Couple to cells
        foreach (InventoryCell item in draggedItemCells)
        {
            item.occupyingObject = draggingItem;
        }

        isDraggingItem = false;
        draggingItem = null;
        draggedItemPos = Vector2.zero;
        draggedItemCells = new List<InventoryCell>();
    }

    public void RotateItem(InputAction.CallbackContext input)
    {
        if (isDraggingItem)
        {
            if (input.ReadValue<float>() < 0)
            {
                draggingItem.transform.rotation *= Quaternion.Euler(0, 0, 90);
            }
            else if (input.ReadValue<float>() > 0)
            {
                draggingItem.transform.rotation *= Quaternion.Euler(0, 0, -90);
            }
        }
    }

    public void AssignToHotbar(InputAction.CallbackContext input)
    {
        if (isDraggingItem)
        {
            if (!PlayerUIManager.showingPause && PlayerUIManager.showingPlayerMenu)
            {
                int inputIndex = playerControls.Menu.HotbarSelectByKey.GetBindingIndexForControl(input.control);
                if (inputIndex >= 0 && inputIndex <= 9)
                {
                    Hotbar.instance.AssignToHotbar(draggingItem, inputIndex);
                }
                ResetItem();
            }
        }  
    }
    #endregion

    #region Give item
    public bool GiveItem(InventoryItem giveItem)
    {
        // Check if there's room in the inventory to place the item
        // First if there's a container available
        // Second if there's room on the tiles

        bool hasContainer = itemsInInventory.Any(x =>
                x.GetComponent<InventoryItemContainer>() != null &&
                x.GetComponent<InventoryItemContainer>().containerType == giveItem.containerType &&
                x.GetComponent<InventoryItemContainer>().CanAddItem());

        if (giveItem.containerItem && hasContainer)
        {
            // Item is containeritem & has available container
            itemsInInventory.First(x =>
                x.GetComponent<InventoryItemContainer>() != null &&
                x.GetComponent<InventoryItemContainer>().containerType == giveItem.containerType &&
                x.GetComponent<InventoryItemContainer>().CanAddItem()).GetComponent<InventoryItemContainer>().AddItem(giveItem);

            // Show popup
            ItemPopupContainer.instance.AddItemPopup(giveItem);

            return true;
        }
        else if (FindSpot(giveItem, out InventoryCell selectedStartCell, out List<InventoryCell> cellsToOccupy, out int rotation))
        {
            // Item either needs a container of is loose item, has space to be placed
            // Instantiate ItemHolder prefab on position & rotation
            GameObject givenItemObject = GameObject.Instantiate(giveItem.inventoryItemHolder);
            givenItemObject.transform.SetParent(instance.itemHolder.transform, false);
            givenItemObject.transform.position = selectedStartCell.gameObject.transform.position;
            givenItemObject.transform.Rotate(0, 0, -rotation);
            givenItemObject.GetComponent<InventoryItemHolder>().item = giveItem;

            // Set occupied cells
            GameObject itemAnchorPoints = giveItem.inventoryItemHolder.transform.GetChild(0).gameObject;
            List<InventoryCell> anchorPointCells = itemAnchorPoints.GetComponentsInChildren<InventoryCell>().ToList();
            List<InventoryCell> inventoryCells = cells.Where(x => x.occupyingObject == null).ToList();

            OccupyInventoryCells(cellsToOccupy, givenItemObject);

            // Add to inventory items
            itemsInInventory.Add(givenItemObject);

            // Show popup
            ItemPopupContainer.instance.AddItemPopup(giveItem);

            // If container, place item in container
            if (giveItem.containerItem)
            {
                givenItemObject.GetComponent<InventoryItemContainer>().AddItem(giveItem);
            }
            if (giveItem.isRecipeBook)
            {
                givenItemObject.GetComponent<InventoryItemBook>().SetRecipeBook(giveItem);
            }

            // Add to hotbar
            Hotbar.instance.AssignToHotbar(givenItemObject);

            return true;
        }

        // Item has no space to be placed
        return false;
        
    }

    public bool FindSpot(InventoryItem giveItem, out InventoryCell selected, out List<InventoryCell> cellsToOccupyNext, out int rotation)
    {
        GameObject itemAnchorPoints = giveItem.inventoryItemHolder.GetComponent<InventoryItemDragger>().anchorPointHolder.gameObject;
        List<InventoryCell> anchorPointCells = itemAnchorPoints.GetComponentsInChildren<InventoryCell>().ToList();

        List<InventoryCell> inventoryCells = cells.Where(x => x.occupyingObject == null).ToList();

        if (anchorPointCells.Count > 0)
        {
            foreach (InventoryCell cell in inventoryCells)
            {
                if (FindSpotThroughPattern0(anchorPointCells, cell, inventoryCells, out List<InventoryCell> cellsToOccupyNorth))
                {
                    selected = cell;
                    cellsToOccupyNext = cellsToOccupyNorth;
                    rotation = 0;
                    return true;
                }
                else if (FindSpotThroughPattern90(anchorPointCells, cell, inventoryCells, out List<InventoryCell> cellsToOccupyEast))
                {
                    selected = cell;
                    cellsToOccupyNext = cellsToOccupyEast;
                    rotation = 90;
                    return true;
                }
                else if (FindSpotThroughPattern180(anchorPointCells, cell, inventoryCells, out List<InventoryCell> cellsToOccupySouth))
                {
                    selected = cell;
                    cellsToOccupyNext = cellsToOccupySouth;
                    rotation = 180;
                    return true;
                }
                else if (FindSpotThroughPattern270(anchorPointCells, cell, inventoryCells, out List<InventoryCell> cellsToOccupyWest))
                {
                    selected = cell;
                    cellsToOccupyNext = cellsToOccupyWest;
                    rotation = 270;
                    return true;
                }
            }
        }
        selected = null;
        cellsToOccupyNext = null;
        rotation = 0;
        return false;
    }

    public bool FindSpotThroughPattern180(List<InventoryCell> anchorPoints, InventoryCell startCell, List<InventoryCell> inventoryCells, out List<InventoryCell> cellsToOccupy)
    {
        cellsToOccupy = new List<InventoryCell>();

        foreach (InventoryCell anchorObject in anchorPoints)
        {
            Vector2 coords180 = anchorObject.Get180Orientation();

            // Check if inventory has available cell on anchorobject pos if placed on startcell
            bool cellAvailable = inventoryCells.Any(
                x => x.coords.x == startCell.coords.x + coords180.x &&
                x.coords.y == startCell.coords.y + coords180.y);

            if (!cellAvailable)
            {
                // Orientation not possible, return
                return false;
            }

            cellsToOccupy.Add(inventoryCells.First(
                x => x.coords.x == startCell.coords.x + coords180.x &&
                x.coords.y == startCell.coords.y + coords180.y));
        }

        return true;
    }

    public bool FindSpotThroughPattern0(List<InventoryCell> anchorPoints, InventoryCell startCell, List<InventoryCell> inventoryCells, out List<InventoryCell> cellsToOccupy)
    {
        cellsToOccupy = new List<InventoryCell>();

        foreach (InventoryCell anchorObject in anchorPoints)
        {
            // Check if inventory has available cell on anchorobject pos if placed on startcell
            bool cellAvailable = inventoryCells.Any(
                x => x.coords.x == startCell.coords.x - anchorObject.coords.x &&
                x.coords.y == startCell.coords.y - anchorObject.coords.y);

            if (!cellAvailable)
            {
                // Orientation not possible, return
                return false;
            }

            cellsToOccupy.Add(inventoryCells.First(
                x => x.coords.x == startCell.coords.x - anchorObject.coords.x &&
                x.coords.y == startCell.coords.y - anchorObject.coords.y));
        }

        return true;
    }

    public bool FindSpotThroughPattern90(List<InventoryCell> anchorPoints, InventoryCell startCell, List<InventoryCell> inventoryCells, out List<InventoryCell> cellsToOccupy)
    {
        cellsToOccupy = new List<InventoryCell>();

        foreach (InventoryCell anchorObject in anchorPoints)
        {
            Vector2 coords90 = anchorObject.Get90Orientation();

            // Check if inventory has available cell on anchorobject pos if placed on startcell
            bool cellAvailable = inventoryCells.Any(
                x => x.coords.x == startCell.coords.x + coords90.x &&
                x.coords.y == startCell.coords.y - coords90.y);

            if (!cellAvailable)
            {
                // Orientation not possible, return
                return false;
            }

            cellsToOccupy.Add(inventoryCells.First(
                x => x.coords.x == startCell.coords.x + coords90.x &&
                x.coords.y == startCell.coords.y - coords90.y));
        }

        return true;
    }

    public bool FindSpotThroughPattern270(List<InventoryCell> anchorPoints, InventoryCell startCell, List<InventoryCell> inventoryCells, out List<InventoryCell> cellsToOccupy)
    {
        cellsToOccupy = new List<InventoryCell>();

        foreach (InventoryCell anchorObject in anchorPoints)
        {
            Vector2 coords270 = anchorObject.Get270Orientation();

            // Check if inventory has available cell on anchorobject pos if placed on startcell
            bool cellAvailable = inventoryCells.Any(
                x => x.coords.x == startCell.coords.x - coords270.x &&
                x.coords.y == startCell.coords.y + coords270.y);

            if (!cellAvailable)
            {
                // Orientation not possible, return
                return false;
            }

            cellsToOccupy.Add(inventoryCells.First(
                x => x.coords.x == startCell.coords.x - coords270.x &&
                x.coords.y == startCell.coords.y + coords270.y));
        }

        return true;
    }


    public void OccupyInventoryCells(List<InventoryCell> cellsToOccupy, GameObject givenItemObject)
    {
        foreach (InventoryCell cell in cellsToOccupy)
        {
            cell.occupyingObject = givenItemObject;
        }
    }
    #endregion

    #region Take item
    public void TakeItem(InventoryItem takeItem)
    {
        bool hasContainer = itemsInInventory.Any(x =>
            x.GetComponent<InventoryItemContainer>() != null &&
            x.GetComponent<InventoryItemContainer>().containerType == takeItem.containerType &&
            x.GetComponent<InventoryItemContainer>().ReturnItemCount() > 0 &&
            x.GetComponent<InventoryItemContainer>().itemsInContainer.Contains(takeItem)
            );

        if (takeItem.containerItem && hasContainer)
        {
            // Take item from container
            GameObject itemContainer = itemsInInventory.First(x =>
                x.GetComponent<InventoryItemContainer>() != null &&
                x.GetComponent<InventoryItemContainer>().containerType == takeItem.containerType &&
                x.GetComponent<InventoryItemContainer>().ReturnItemCount() > 0 &&
                x.GetComponent<InventoryItemContainer>().itemsInContainer.Contains(takeItem)
                );
            itemContainer.GetComponent<InventoryItemContainer>().RemoveItem(takeItem);

            // Check if container is empty and delete if it is
            if (itemContainer.GetComponent<InventoryItemContainer>().ReturnItemCount() == 0)
            {
                // Decouple inventory cells
                List<InventoryCell> occupiedCells = cells.Where(x => x.occupyingObject == itemContainer).ToList();
                foreach (InventoryCell cell in occupiedCells)
                {
                    cell.occupyingObject = null;
                }

                // Delete container
                itemsInInventory.Remove(itemContainer);
                GameObject.Destroy(itemContainer);
            }
        }
        else if (itemsInInventory.Any(x => x.GetComponent<InventoryItemHolder>().item == takeItem))
        {
            // Else take singular item
            GameObject itemFromInventory = itemsInInventory.First(x => x.GetComponent<InventoryItemHolder>().item == takeItem);

            itemsInInventory.Remove(itemFromInventory);
            GameObject.Destroy(itemFromInventory);
        }
        else
        {
            // No such item in inventory

        }
    }
    public void TakeItem(IngredientType ingredientType)
    {
        GameObject item = itemsInInventory.First(x => 
            x.GetComponent<InventoryItemContainer>() && x.GetComponent<InventoryItemContainer>().itemsInContainer.Any(y => y.ingredientType == ingredientType) ||
            x.GetComponent<InventoryItemHolder>() && x.GetComponent<InventoryItemHolder>().item.ingredientType == ingredientType);
        InventoryItem takeItem = null;
        if (item.GetComponent<InventoryItemContainer>())
        {
            takeItem = item.GetComponent<InventoryItemContainer>().itemsInContainer.First(y => y.ingredientType == ingredientType);
        }
        else
        {
            takeItem = item.GetComponent<InventoryItemHolder>().item;
        }

        if (takeItem != null)
        {
            bool hasContainer = itemsInInventory.Any(x =>
            x.GetComponent<InventoryItemContainer>() != null &&
            x.GetComponent<InventoryItemContainer>().containerType == takeItem.containerType &&
            x.GetComponent<InventoryItemContainer>().ReturnItemCount() > 0 &&
            x.GetComponent<InventoryItemContainer>().itemsInContainer.Contains(takeItem)
            );

            if (takeItem.containerItem && hasContainer)
            {
                // Take item from container
                GameObject itemContainer = itemsInInventory.First(x =>
                    x.GetComponent<InventoryItemContainer>() != null &&
                    x.GetComponent<InventoryItemContainer>().containerType == takeItem.containerType &&
                    x.GetComponent<InventoryItemContainer>().ReturnItemCount() > 0 &&
                    x.GetComponent<InventoryItemContainer>().itemsInContainer.Contains(takeItem)
                    );
                itemContainer.GetComponent<InventoryItemContainer>().RemoveItem(takeItem);

                // Check if container is empty and delete if it is
                if (itemContainer.GetComponent<InventoryItemContainer>().ReturnItemCount() == 0)
                {
                    // Decouple inventory cells
                    List<InventoryCell> occupiedCells = cells.Where(x => x.occupyingObject == itemContainer).ToList();
                    foreach (InventoryCell cell in occupiedCells)
                    {
                        cell.occupyingObject = null;
                    }

                    // Delete container
                    itemsInInventory.Remove(itemContainer);
                    GameObject.Destroy(itemContainer);
                }
            }
            else if (itemsInInventory.Any(x => x.GetComponent<InventoryItemHolder>().item == takeItem))
            {
                // Else take singular item
                GameObject itemFromInventory = itemsInInventory.First(x => x.GetComponent<InventoryItemHolder>().item == takeItem);

                itemsInInventory.Remove(itemFromInventory);
                GameObject.Destroy(itemFromInventory);
            }
            else
            {
                // No such item in inventory

            }
        }
    }
    #endregion
}
