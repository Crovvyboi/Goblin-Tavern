using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPopup : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public RawImage icon;
    public TextMeshProUGUI itemTitleText;
    public TextMeshProUGUI itemAmountText;

    public float fadeTimer;

    public InventoryItem inventoryItem;
    public int amount;

    // Update is called once per frame
    void FixedUpdate()
    {
        fadeTimer += Time.deltaTime;
        if (fadeTimer >= 5f)
        {
            canvasGroup.alpha -= 0.2f;
            if (canvasGroup.alpha <= 0f)
            {
                ItemPopupContainer.instance.RemovePopup(this);
                Destroy(this.gameObject);
            }
        }
    }

    public void AddToAmount()
    {
        canvasGroup.alpha = 1f;
        fadeTimer = 0f;
        amount++;
        UpdateText();
    }

    public void UpdateText()
    {
        icon.texture = inventoryItem.hotbarIcon;
        itemTitleText.text = inventoryItem.inventoryItemName;
        itemAmountText.text = $"{amount.ToString()}X";
    }
}
