using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerServiceManager : MonoBehaviour
{
    public static PlayerServiceManager instance;
    public PlayerControls playerControls;

    [Header("Orders")]
    public Dictionary<Table, List<MenuItem>> takenOrders = new Dictionary<Table, List<MenuItem>>();
    public List<OrderItem> carryingItems = new List<OrderItem>();
    public int carryingCapacity = 10;

    [Header("Order selection")]
    public GameObject orderSelectionBarPrefab;
    public GameObject orderSelectionTagPrefab;

    public List<GameObject> pickedupOrderBarAnchors;
    public Dictionary<Table, GameObject> orderSelectionTags = new Dictionary<Table, GameObject>();
    public GameObject selectionBar;
    public int highlightedOrder = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }


        this.GetComponent<PlayerServiceManager>().enabled = false;
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    private void OnEnable()
    {
        playerControls.Enable();

        playerControls.General.Interact.started += OnInteract;
        playerControls.Service.ScrollOrderTag.started += OnScroll;
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void OnInteract(InputAction.CallbackContext input)
    {
        if (input.started && PlayerUIManager.instance.canInteract && TavernManager.state == TavernState.ServiceOverview)
        {
            if (ServiceOverviewUI.instance != null)
            {
                ServiceOverviewUI.instance.OnInteract();   
            }
        }
    }

    public void OnScroll(InputAction.CallbackContext input)
    {
        if (TavernManager.state == TavernState.Service || TavernManager.state == TavernState.ServiceFinalCall)
        {
            if (orderSelectionTags.Count > 1 && input.ReadValue<float>() != 0)
            {
                NextOrder();
            }
        }
        
    }

    public void OnServiceStart()
    {
        takenOrders = new Dictionary<Table, List<MenuItem>>();
        carryingItems = new List<OrderItem>();
        orderSelectionTags = new Dictionary<Table, GameObject>();
        highlightedOrder = 0;
    }


    #region Orders
    public bool CanTakeNewOrder()
    {
        if (takenOrders.Count +1 <= 3)
        {
            return true;
        }
        return false;
    }

    public void AddOrder(Table table, List<MenuItem> itemlist)
    {
        takenOrders.Add(table, itemlist);

        GameObject newObject = GameObject.Instantiate(orderSelectionTagPrefab);
        newObject.GetComponentInChildren<TextMeshProUGUI>().text = OrderToString(table, itemlist);
        
        int ordercount = orderSelectionTags.Count;
        GameObject barAnchor = pickedupOrderBarAnchors[ordercount];
        newObject.transform.SetParent(barAnchor.transform);

        newObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        newObject.transform.localPosition = Vector3.zero;

        orderSelectionTags.Add(table, newObject);

        if (selectionBar == null)
        {
            highlightedOrder = 0;
            selectionBar = GameObject.Instantiate(orderSelectionBarPrefab);
            
        }
        SelectOrder();
    }

    public void SelectOrder()
    {
        selectionBar.transform.SetParent(pickedupOrderBarAnchors[highlightedOrder].transform);
        selectionBar.transform.SetSiblingIndex(0);
        selectionBar.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        selectionBar.transform.localPosition = Vector3.zero;

        orderSelectionTags.Keys.ElementAt(highlightedOrder).gameObject.transform.Find("OrderSelectedSquare").gameObject.SetActive(true);
    }

    public void RemoveOrder(Table table)
    {
        orderSelectionTags.Keys.ElementAt(highlightedOrder).gameObject.transform.Find("OrderSelectedSquare").gameObject.SetActive(false);

        GameObject.Destroy(orderSelectionTags.GetValueOrDefault(table));
        orderSelectionTags.Remove(table);

        takenOrders.Remove(table);

        if (takenOrders.Count == 0 && highlightedOrder == 0)
        {
            GameObject.Destroy(selectionBar);
        }
        else
        {
            Dictionary<Table, List<MenuItem>> orders = new Dictionary<Table, List<MenuItem>>();
            foreach (KeyValuePair<Table, List<MenuItem>> item in takenOrders)
            {
                orders.Add(item.Key, item.Value);
            }
            takenOrders = orders;

            int count = 0;
            foreach (KeyValuePair<Table, GameObject> item in orderSelectionTags)
            {
                // Get gameobject value index
                int currentanchor = pickedupOrderBarAnchors.FindIndex(x => x == item.Value.transform.parent.gameObject);

                // if index != count, move gameobject to anchor on index count
                if (currentanchor != count)
                {
                    item.Value.transform.SetParent(pickedupOrderBarAnchors[count].transform, false);
                }

                count++;
            }

            // Move order tag selection
            if (takenOrders.Count > 0 && highlightedOrder != 0 && highlightedOrder > takenOrders.Count - 1)
            {
                highlightedOrder = takenOrders.Count - 1;
            }
            SelectOrder();

        }
    }
    #endregion

    #region Making order items
    public void MakeOrderItem(Table table, MenuItem menuItem)
    {
        carryingItems.Add(new OrderItem(table, menuItem));
        takenOrders.GetValueOrDefault(table).Remove(menuItem);

        // Update order tag
        GameObject tag = orderSelectionTags.GetValueOrDefault(table);
        tag.GetComponentInChildren<TextMeshProUGUI>().text = OrderToString(table, takenOrders.GetValueOrDefault(table));

        // Check if order is completed
        if (takenOrders.GetValueOrDefault(table).Count == 0)
        {
            table.transform.Find("OrderReadySquare").GetComponent<SpriteRenderer>().color = new Color(0, 0, 255);
        }
    }

    public struct OrderItem
    {
        public Table table;
        public MenuItem menuItem;

        public OrderItem(Table table, MenuItem menuitem)
        {
            this.table = table;
            this.menuItem = menuitem;
        }
    }

    public bool CanCarryItem()
    {
        if (carryingItems.Count < carryingCapacity)
        {
            return true;
        }
        return false;
    }
    #endregion

    #region Order tags
    public string OrderToString(Table table, List<MenuItem> menuItems)
    {
        string orderString = "";

        if (menuItems.Count > 0)
        {
            List<MenuItem> groupedItems = menuItems.Distinct().ToList();
            for (int i = 0; i < groupedItems.Count; i++)
            {
                orderString += " - " + groupedItems[i].itemName + " ";

                orderString += menuItems.Where(x => x.itemName == groupedItems[i].itemName).Count().ToString() + "x ";

                if (menuItems.Count != i + 1)
                {
                    orderString += "\n";
                }
            }
            orderString += "\n";
            List<OrderItem> tableitems = carryingItems.Where(x => x.table == table).ToList();
            if (tableitems.Count > 0)
            {
                orderString += $"Carrying {tableitems.Count} items.";
            }
        }
        else
        {
            orderString = "All items made!";
        }

        return orderString;
    }

    public void NextOrder()
    {
        orderSelectionTags.Keys.ElementAt(highlightedOrder).gameObject.transform.Find("OrderSelectedSquare").gameObject.SetActive(false);

        if (playerControls.Service.ScrollOrderTag.ReadValue<float>() < 0)
        {
            if (highlightedOrder - 1 == -1)
            {
                highlightedOrder = orderSelectionTags.Count - 1;
                SelectOrder();
            }
            else
            {
                highlightedOrder--;
                SelectOrder();
            }
        }
        else if (playerControls.Service.ScrollOrderTag.ReadValue<float>() > 0)
        {
            if (highlightedOrder + 1 > orderSelectionTags.Count - 1)
            {
                highlightedOrder = 0;
                SelectOrder();
            }
            else
            {
                highlightedOrder++;
                SelectOrder();
            }
        }
    }

    public KeyValuePair<Table, List<MenuItem>> GetHighlightedOrder()
    {
        KeyValuePair<Table, List<MenuItem>> keyValuePair = takenOrders.ElementAt(highlightedOrder);
        return keyValuePair;
    }
    #endregion


    
}
