using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemBook : MonoBehaviour
{
    public List<Recipe> selectedRecipes = new List<Recipe>();

    public void SetRecipeBook(InventoryItem inventoryItem)
    {
        selectedRecipes.Clear();
        if (inventoryItem.isRandom)
        {
            List<Recipe> copiedList = new List<Recipe>();
            copiedList.AddRange(inventoryItem.recipePool);
            for (int i = 0; i < inventoryItem.recipesToSelect; i++)
            {
                int random = Random.Range(0, copiedList.Count);
                selectedRecipes.Add(copiedList[random]);
                copiedList.RemoveAt(random);
            }
        }
        else
        {
            selectedRecipes.AddRange(inventoryItem.recipePool);
        }
    }

    public void LearnRecipes()
    {

    }
}
