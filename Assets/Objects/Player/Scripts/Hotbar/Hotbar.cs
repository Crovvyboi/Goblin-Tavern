using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    public static Hotbar instance;

    public PlayerControls playerControls;

    public List<GameObject> hotbarSlotsGameObjects = new List<GameObject>();
    public int highlightedPos = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        SelectSlot();
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
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void SelectSlot()
    {
        hotbarSlotsGameObjects[highlightedPos].GetComponent<Outline>().enabled = true;
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
}
