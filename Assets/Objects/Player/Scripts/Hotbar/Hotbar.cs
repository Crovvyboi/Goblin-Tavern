using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    public static Hotbar instance;

    public PlayerControls playerControls;

    public List<GameObject> hotbarSlotsGameObjects = new List<GameObject>();
    public int highlightedPos = 0;

    public bool isInInteractable;
    public bool canPlaceFurniture;
    public GameObject furniturePlacementMold;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        isInInteractable = false;

        SelectSlot();
    }

    private void FixedUpdate()
    {
        if (hotbarSlotsGameObjects[highlightedPos].GetComponent<HotbarSlot>().assignedInventoryItem != null)
        {
            GameObject hotbarItem = hotbarSlotsGameObjects[highlightedPos].GetComponent<HotbarSlot>().assignedInventoryItem;

            switch (hotbarItem.GetComponent<InventoryItemHolder>().item.type)
            {
                case InventoryItemType.Quest:
                    InteractionSender.instance.canInteract = true;
                    break;
                case InventoryItemType.Ingredient:
                    InteractionSender.instance.canInteract = true;
                    break;
                case InventoryItemType.Furniture:
                    InteractionSender.instance.canInteract = false;
                    CheckIfFurnitureIsPlaceable(hotbarItem.GetComponent<InventoryItemHolder>().item);
                    break;
                default:
                    break;
            }
        }
        else
        {
            InteractionSender.instance.canInteract = true;
        }
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Menu.HotbarSelectSlot.performed += SelectFromScroll;
        playerControls.Menu.HotbarSelectByKey.performed += SelectFromKeyPress;
        playerControls.General.RotateHotbarItem.performed += RotateFurniture;
        playerControls.General.Interact.performed += PlaceFurniture;
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void SelectSlot()
    {
        hotbarSlotsGameObjects[highlightedPos].GetComponent<Outline>().enabled = true;
        if (furniturePlacementMold != null)
        {
            Destroy(furniturePlacementMold);
        }
    }
    public void DeselectSlot()
    {
        hotbarSlotsGameObjects[highlightedPos].GetComponent<Outline>().enabled = false;
    }

    public void SelectNextSlot()
    {
        DeselectSlot();
        if (highlightedPos == 9)
        {
            highlightedPos = 0;
        }
        else
        {
            highlightedPos++;
        }
        SelectSlot();
    }

    public void SelectPreviousSlot()
    {
        DeselectSlot();
        if (highlightedPos == 0)
        {
            highlightedPos = 9;
        }
        else
        {
            highlightedPos--;
        }
        SelectSlot();
    }

    public void SelectFromScroll(InputAction.CallbackContext input)
    {
        if (!PlayerUIManager.showingPause && !PlayerUIManager.showingPlayerMenu)
        {
            if (input.ReadValue<float>() < 0)
            {
                SelectPreviousSlot();
            }
            else if (input.ReadValue<float>() > 0)
            {
                SelectNextSlot();
            }
        }
    }

    public void SelectFromKeyPress(InputAction.CallbackContext input)
    {
        if (!PlayerUIManager.showingPause && !PlayerUIManager.showingPlayerMenu)
        {
            int inputIndex = playerControls.Menu.HotbarSelectByKey.GetBindingIndexForControl(input.control);
            if (inputIndex >= 0 && inputIndex <= 9)
            {
                DeselectSlot();
                highlightedPos = inputIndex;
                SelectSlot();
            }
        }
    }

    public void AssignToHotbar(GameObject gameObject, int index)
    {
        if (hotbarSlotsGameObjects[index].GetComponent<HotbarSlot>().assignedInventoryItem == gameObject)
        {
            hotbarSlotsGameObjects[index].GetComponent<HotbarSlot>().RemoveItem();

            GameObject assignedIconSlot = hotbarSlotsGameObjects[index].transform.GetChild(0).gameObject;
            assignedIconSlot.GetComponent<RawImage>().texture = null;
            assignedIconSlot.GetComponent<RawImage>().enabled = false;
        }
        else
        {
            // Clear slot if already assigned
            if (hotbarSlotsGameObjects.Any(x => x.GetComponent<HotbarSlot>().assignedInventoryItem == gameObject))
            {
                GameObject slot = hotbarSlotsGameObjects.First(x => x.GetComponent<HotbarSlot>().assignedInventoryItem == gameObject);
                slot.GetComponent<HotbarSlot>().assignedInventoryItem = null;
                slot.transform.GetChild(0).gameObject.GetComponent<RawImage>().texture = null;
                slot.transform.GetChild(0).gameObject.GetComponent<RawImage>().enabled = false;
            }

            // Assign to slot
            hotbarSlotsGameObjects[index].GetComponent<HotbarSlot>().AssignItem(gameObject);

            GameObject assignedIconSlot = hotbarSlotsGameObjects[index].transform.GetChild(0).gameObject;
            assignedIconSlot.GetComponent<RawImage>().texture = gameObject.GetComponent<InventoryItemHolder>().item.hotbarIcon;
            assignedIconSlot.GetComponent<RawImage>().enabled = true;
        }
        
    }
    public void RemoveFromHotbar(int slot)
    {
        hotbarSlotsGameObjects[slot].GetComponent<HotbarSlot>().assignedInventoryItem = null;
        hotbarSlotsGameObjects[slot].transform.GetChild(0).GetComponent<RawImage>().texture = null;
        hotbarSlotsGameObjects[slot].transform.GetChild(0).GetComponent<RawImage>().enabled = false;
    }

    public void CheckIfFurnitureIsPlaceable(InventoryItem item)
    {
        if (item.isPlaceable)
        {
            if (furniturePlacementMold == null)
            {
                furniturePlacementMold = GameObject.Instantiate(item.prefab);
                if (furniturePlacementMold.GetComponent<BoxCollider2D>() != null)
                {
                    furniturePlacementMold.GetComponent<BoxCollider2D>().enabled = false;
                    if (item.isPlaceable)
                    {
                        if (furniturePlacementMold.GetComponent<Table>() != null)
                        {
                            foreach (Transform child in furniturePlacementMold.transform)
                            {
                                if (furniturePlacementMold.GetComponent<Table>() != null)
                                {
                                    child.Find("OrderSelectedSquare").gameObject.SetActive(false);
                                    child.Find("OrderReadySquare").gameObject.SetActive(false);
                                }
                                if (child.Find("InteractionField") != null)
                                {
                                    child.Find("InteractionField").gameObject.SetActive(false);
                                }
                            }
                        }
                    }
                }
                else if (furniturePlacementMold.GetComponentInChildren<BoxCollider2D>() != null)
                {
                    furniturePlacementMold.GetComponentInChildren<BoxCollider2D>().enabled = false;
                    if (item.isPlaceable)
                    {
                        foreach (Transform child in furniturePlacementMold.transform)
                        {
                            if (furniturePlacementMold.GetComponent<Table>() != null)
                            {
                                child.Find("OrderSelectedSquare").gameObject.SetActive(false);
                                child.Find("OrderReadySquare").gameObject.SetActive(false);
                            }
                            if (child.Find("InteractionField") != null)
                            {
                                child.Find("InteractionField").gameObject.SetActive(false);
                            }
                        }
                        
                    }
                }
            }

            Vector3 mousepos = Camera.main.ScreenToWorldPoint(new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), 0));
            Vector3 tiled = new Vector3(Mathf.Round(mousepos.x), Mathf.Round(mousepos.y), 0);

            // Check if mouse position has tavern tile
            if (Vector3.Distance(tiled, GameObject.FindGameObjectWithTag("Player").transform.position) < 3f)
            {
                furniturePlacementMold.SetActive(true);
                furniturePlacementMold.transform.position = new Vector3(tiled.x + 0.5f, tiled.y - 0.5f, 0);

                Tilemap tilemap = furniturePlacementMold.GetComponentInChildren<Tilemap>();

                // Check if position is within range
                if (TavernTilemapManager.instance.CheckIfPositionHasTavernTile(furniturePlacementMold.GetComponent<FurnitureOrientation>().GetTiles()))
                {

                    // Check if furniture can be placed
                    if (TavernTilemapManager.instance.CanPlaceFurniture(furniturePlacementMold.GetComponent<FurnitureOrientation>().GetTiles()))
                    {
                        // show placable furniture prefab
                        if (tilemap != null)
                        {
                            tilemap.color = Color.green;
                            
                        }

                        canPlaceFurniture = true;
                    }
                    else
                    {
                        // show unplacable furniture prefab
                        if (tilemap != null)
                        {
                            tilemap.color = Color.red;
                        }
                        canPlaceFurniture = false;
                    }

                }
                else
                {
                    // show unplacable furniture prefab

                    if (tilemap != null)
                    {
                        tilemap.color = Color.red;
                    }
                    canPlaceFurniture = false;
                }
            }
            else
            {
                // show unplacable furniture prefab
                furniturePlacementMold.SetActive(false);
                canPlaceFurniture = false;
            }
        }
        
    }

    public void RotateFurniture(InputAction.CallbackContext input)
    {
        if (furniturePlacementMold != null && furniturePlacementMold.activeSelf && input.performed)
        {
            furniturePlacementMold.GetComponent<FurnitureOrientation>().RotateRight();
        }
    }

    public void PlaceFurniture(InputAction.CallbackContext input)
    {
        if (canPlaceFurniture && input.performed && furniturePlacementMold != null)
        {
            // Check if other interactables have priority
            if (!isInInteractable)
            {
                // Place furniture
                furniturePlacementMold.transform.SetParent(TavernManager.instance.furnitureContainer.transform, true);
                furniturePlacementMold.GetComponentInChildren<Tilemap>().color = Color.white;

                if (furniturePlacementMold.GetComponent<BoxCollider2D>() != null)
                {
                    furniturePlacementMold.GetComponent<BoxCollider2D>().enabled = true;
                }
                else if(furniturePlacementMold.GetComponentInChildren<BoxCollider2D>() != null)
                {
                    furniturePlacementMold.GetComponentInChildren<BoxCollider2D>().enabled = true;
                }

                foreach (Transform child in furniturePlacementMold.transform)
                {
                    if (furniturePlacementMold.GetComponent<Table>() != null)
                    {
                        child.Find("OrderSelectedSquare").gameObject.SetActive(true);
                        child.Find("OrderReadySquare").gameObject.SetActive(true);
                    }
                    if (child.Find("InteractionField") != null)
                    {
                        child.Find("InteractionField").gameObject.SetActive(true);
                    }

                }
                furniturePlacementMold = null;

                // Remove from inventory
                GameObject go = hotbarSlotsGameObjects[highlightedPos].GetComponent<HotbarSlot>().assignedInventoryItem;
                InventoryItem item = go.GetComponent<InventoryItemHolder>().item;
                PlayerInventory.instance.TakeItem(item);

                // Remove from hotbar
                RemoveFromHotbar(highlightedPos);

                // Reload furniture layer
                TavernTilemapManager.instance.GenerateFurnitureTiles();
            }
        }
    }
}
