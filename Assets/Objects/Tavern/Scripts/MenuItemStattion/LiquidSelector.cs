using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiquidSelector : MonoBehaviour
{
    public LiquidSetting liquidName;
    public Texture icon;

    public void OnSelect()
    {
        MenuItemStation.instance.SelectLiquid((int)liquidName);
    }
}
