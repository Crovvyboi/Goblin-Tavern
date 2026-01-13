using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDiscovery : MonoBehaviour
{
    public static ItemDiscovery instance;

    public CanvasGroup canvasGroup;
    public TextMeshProUGUI itemNameText;
    public RawImage itemIcon;

    public List<Recipe> recipeQueue = new List<Recipe>();
    public Recipe nextRecipe;
    public float maxTime = 2.5f;
    public float currentTime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        canvasGroup.alpha = 0;
    }

    private void FixedUpdate()
    {
        if (recipeQueue.Count > 0)
        {
            currentTime += Time.deltaTime;
            if (currentTime > maxTime)
            {
                canvasGroup.alpha -= 0.25f;
                if (canvasGroup.alpha <= 0)
                {
                    recipeQueue.Remove(nextRecipe);
                    if (recipeQueue.Count > 0)
                    {
                        LoadNext();
                    }
                }
            }
        }
    }

    public void LoadNext()
    {
        currentTime = 0;
        nextRecipe = recipeQueue[0];
        canvasGroup.alpha = 1;

        if (nextRecipe.resultMenuItem)
        {
            itemNameText.text = nextRecipe.resultMenuItem.itemName;
            itemIcon.texture = nextRecipe.resultMenuItem.icon;
        }
        else if (nextRecipe.resultIngredient)
        {
            itemNameText.text = nextRecipe.resultIngredient.inventoryItemName;
            itemIcon.texture = nextRecipe.resultIngredient.hotbarIcon;
        }
        
    }

    public void AddToQueue(Recipe recipe)
    {
        recipeQueue.Add(recipe);
        if (recipeQueue.Count == 1)
        {
            LoadNext();
        }
    }
}
