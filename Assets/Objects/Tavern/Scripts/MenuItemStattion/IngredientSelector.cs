using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientSelector : MonoBehaviour
{
    public InventoryItem ingredient;
    public int amount;

    public RawImage icon;
    public TextMeshProUGUI ingredientName;
    public TextMeshProUGUI amountText;


    public void SetSelector(Texture texture, string name, int amount)
    {
        icon.texture = texture;
        ingredientName.text = name;
        amountText.text = $"{amount} X";
    }

    public void FinalizeSelector()
    {
        icon.texture = ingredient.hotbarIcon;
        ingredientName.text = ingredient.inventoryItemName;
        amountText.text = amount.ToString();
    }

    public void OnSelect()
    {
        MenuItemStation.instance.SelectIngredient(ingredient);
        MenuItemStation.instance.OnCloseSidebar();
    }
}
