using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSelector : MonoBehaviour
{
    public GemSetting gemName;
    public Texture icon;

    public void OnSelect()
    {
        MenuItemStation.instance.SelectGem((int)gemName);
    }
}
