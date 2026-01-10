using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuItemBaseSelector : MonoBehaviour
{
    public MenuItem baseMenuItem;

    public RawImage icon;
    public TextMeshProUGUI menuItemName;

    public void OnSelect()
    {
        MenuItemStation.instance.SelectMenuItemBase(baseMenuItem);
        MenuItemStation.instance.OnCloseSidebar();
    }
}
