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
        if (hotbarSlotsGameObjects[highlightedPos].GetComponent<HotbarSlot>().assignedInventoryItem != null && !PlayerUIManager.showingPlayerMenu && !PlayerUIManager.showingPause)
        {
            GameObject hotbarItem = hotbarSlotsGameObjects[highlightedPos].GetComponent<HotbarSlot>().assignedInventoryItem;

            switch (hotbarItem.GetComponent<InventoryItemHolder>().item.type)
            {
                case InventoryItemType.Furniture:
                    InteractionSender.instance.canInteract = false;
                    CheckIfFurnitureIsPlaceable(hotbarItem.GetComponent<InventoryItemHolder>().item);
                    break;
                default:
                    InteractionSender.instance.canInteract = true;
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
        playerControls.General.Interact.performed += ActivateHighlightedItem;
        playerControls.General.AlternateInteract.performed += TakeFurniture;
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
    public void AssignToHotbar(GameObject gameObject)
    {
        // Get next empty slot
        int index = -1;
        for (int i = 0; i < hotbarSlotsGameObjects.Count; i++)
        {            
            if (hotbarSlotsGameObjects[i].GetComponent<HotbarSlot>().assignedInventoryItem == null)
            {
                index = i;
                break;
            }
        }

        // Assign to hotbar
        if (index != -1)
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
    }

    public void RemoveFromHotbar(int slot)
    {
        hotbarSlotsGameObjects[slot].GetComponent<HotbarSlot>().assignedInventoryItem = null;
        hotbarSlotsGameObjects[slot].transform.GetChild(0).GetComponent<RawImage>().texture = null;
        hotbarSlotsGameObjects[slot].transform.GetChild(0).GetComponent<RawImage>().enabled = false;
    }

    public void ActivateHighlightedItem(InputAction.CallbackContext input)
    {
        if (input.performed && !PlayerUIManager.showingPlayerMenu && !PlayerUIManager.showingPause)
        {
            if (hotbarSlotsGameObjects[highlightedPos].GetComponent<HotbarSlot>().assignedInventoryItem != null)
            {
                GameObject hotbarItem = hotbarSlotsGameObjects[highlightedPos].GetComponent<HotbarSlot>().assignedInventoryItem;

                switch (hotbarItem.GetComponent<InventoryItemHolder>().item.type)
                {
                    case InventoryItemType.RecipeBook:
                        ReadRecipeBook(hotbarItem.GetComponent<InventoryItemBook>());
                        break;

                    case InventoryItemType.Furniture:
                        PlaceFurniture();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    #region Furniture
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

            furniturePlacementMold.SetActive(true);
            Vector3? pos = TavernTilemapManager.instance.GetClosestPos(new Vector3(mousepos.x, mousepos.y, 0));

            if (pos != null)
            {
                Vector3 vector3 = (Vector3)pos;
                furniturePlacementMold.transform.position = new Vector3(vector3.x + 0.5f, vector3.y + 0.5f, 0);

                Tilemap tilemap = furniturePlacementMold.GetComponentInChildren<Tilemap>();

                if (Vector3.Distance(tiled, GameObject.FindGameObjectWithTag("Player").transform.position) < 5.5f)
                {
                    if (!CheckIfPlayerIsInTiles(furniturePlacementMold.GetComponent<Furniture>().GetTiles()) &&
                        TavernTilemapManager.instance.CheckIfPositionHasTavernTile(furniturePlacementMold.GetComponent<Furniture>().GetTiles()) &&
                        TavernTilemapManager.instance.CanPlaceFurniture(furniturePlacementMold.GetComponent<Furniture>().GetTiles()))
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
                    furniturePlacementMold.SetActive(false);
                    canPlaceFurniture = false;
                }

            }

        }

    }

    public bool CheckIfPlayerIsInTiles(List<Vector3> tiles)
    {
        Vector3 playerpos = GameObject.FindGameObjectWithTag("Player").transform.position;
        foreach (Vector3 tile in tiles)
        {
            if (Vector3.Distance(playerpos, tile) < 1f)
            {
                return true;
            }
        }
        return false;

    }

    public void RotateFurniture(InputAction.CallbackContext input)
    {
        if (!PlayerUIManager.showingPlayerMenu && !PlayerUIManager.showingPause)
        {
            if (furniturePlacementMold != null && furniturePlacementMold.activeSelf && input.performed)
            {
                furniturePlacementMold.GetComponent<Furniture>().RotateRight();
            }
        }
    }

    public void PlaceFurniture()
    {
        if (canPlaceFurniture && furniturePlacementMold != null)
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
                else if (furniturePlacementMold.GetComponentInChildren<BoxCollider2D>() != null)
                {
                    furniturePlacementMold.GetComponentInChildren<BoxCollider2D>().enabled = true;
                }

                foreach (Transform child in furniturePlacementMold.transform)
                {
                    if (child.Find("InteractionField") != null)
                    {
                        child.Find("InteractionField").gameObject.SetActive(true);
                    }

                }

                // Remove from inventory
                GameObject go = hotbarSlotsGameObjects[highlightedPos].GetComponent<HotbarSlot>().assignedInventoryItem;
                InventoryItem item = go.GetComponent<InventoryItemHolder>().item;
                PlayerInventory.instance.TakeItem(item);

                // Remove from hotbar
                RemoveFromHotbar(highlightedPos);

                // Reload furniture layer
                TavernTilemapManager.instance.OnFurnitureAdd(furniturePlacementMold.GetComponent<Furniture>().tiles);

                furniturePlacementMold = null;
            }
        }
        
    }
    public void TakeFurniture(InputAction.CallbackContext input)
    {
        if (input.performed && !PlayerUIManager.showingPlayerMenu && !PlayerUIManager.showingPause)
        {
            Vector3 mousepos = Camera.main.ScreenToWorldPoint(new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), 0));
            Vector3 tiled = new Vector3(Mathf.Round(mousepos.x), Mathf.Round(mousepos.y), 0);

            Vector3? pos = TavernTilemapManager.instance.GetClosestPos(new Vector3(mousepos.x, mousepos.y, 0));

            // Check if mouse position has tavern tile
            if (Vector3.Distance(tiled, GameObject.FindGameObjectWithTag("Player").transform.position) < 4.5f && pos != null)
            {
                Vector3 vector3 = (Vector3)pos;
                Vector3 tilepos = new Vector3(vector3.x + 0.5f, vector3.y + 0.5f, 0);

                if (TavernTilemapManager.instance.CheckIfPositionHasFurniture(tilepos))
                {
                    // Get furniture
                    GameObject furniture = TavernTilemapManager.instance.GetFurnitureOnPos(tilepos);
                    if (furniture != null)
                    {
                        // Find InventoryItem
                        Furniture furnitureComponent = furniture.GetComponent<Furniture>();
                        if (furnitureComponent.inventoryItem != null)
                        {
                            // Try placing in inventory
                            if (PlayerInventory.instance.GiveItem(furnitureComponent.inventoryItem))
                            {
                                // Remove furniture item
                                TavernTilemapManager.instance.OnFurnitureRemove(furnitureComponent.tiles);

                                Destroy(furniture);
                            }
                            else
                            {
                                // Tell player that there's no room in inventory

                            }
                        }
                    }
                }

            }
        }
    }
    #endregion

    #region Recipe book
    public void ReadRecipeBook(InventoryItemBook book)
    {
        if (book.selectedRecipes.Any(x => x.resultMenuItem && !x.resultMenuItem.recipeKnown || x.resultIngredient && !x.resultIngredient.recipeKnown))
        {
            foreach (Recipe recipe in book.selectedRecipes)
            {
                if (recipe.resultMenuItem && !recipe.resultMenuItem.recipeKnown)
                {
                    recipe.resultMenuItem.recipeKnown = true;
                    ShowRecipeDiscoverPopup(recipe);
                }
                else if (recipe.resultIngredient && !recipe.resultIngredient.recipeKnown)
                {
                    recipe.resultIngredient.recipeKnown = true;
                    ShowRecipeDiscoverPopup(recipe);
                }

            }

            // Remove from inventory
            GameObject go = hotbarSlotsGameObjects[highlightedPos].GetComponent<HotbarSlot>().assignedInventoryItem;
            InventoryItem item = go.GetComponent<InventoryItemHolder>().item;
            PlayerInventory.instance.TakeItem(item);

            // Remove from hotbar
            RemoveFromHotbar(highlightedPos);
        }
        else
        {
            // Popup thoughtbubble with text "I already know all these recipes"


        }
    }

    public void ShowRecipeDiscoverPopup(Recipe recipe)
    {
        ItemDiscovery.instance.AddToQueue(recipe);
    }
    #endregion
}
