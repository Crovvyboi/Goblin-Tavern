using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ServiceOverviewUI : MonoBehaviour
{
    public static ServiceOverviewUI instance;
    private Coroutine startedCoroutine;
    private bool isShowingRolling;

    [Header("Seat")]
    public GameObject seatObject;
    public TextMeshProUGUI seatText;

    [Header("Customer")]
    public GameObject mostPopularObject;
    public GameObject classObject;
    public TextMeshProUGUI classText;
    public RawImage classIcon;
    public GameObject speciesObject;
    public TextMeshProUGUI speciesText;
    public RawImage speciesIcon;

    [Header("Menuitems served")]
    public GameObject drinksServedObject;
    public TextMeshProUGUI drinksServedText;
    public GameObject mealsServedObject;
    public TextMeshProUGUI mealsServedText;

    [Header("Menuitems popular")]
    public GameObject mostSoldObject;
    public GameObject mealObject;
    public TextMeshProUGUI mealText;
    public TextMeshProUGUI mealSoldText;
    public RawImage mealIcon;
    public GameObject drinkObject;
    public TextMeshProUGUI drinkText;
    public TextMeshProUGUI drinkSoldText;
    public RawImage drinkIcon;

    [Header("Final")]
    public GameObject goldObject;
    public TextMeshProUGUI goldText;
    public RawImage goldTrendIcon;
    public GameObject reputationObject;
    public TextMeshProUGUI reputationText;
    public RawImage reputationTrendIcon;
    public GameObject continueText;

    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void OnInteract()
    {
        if (isShowingRolling)
        {
            SkipOverviewAnimation();
        }
        else
        {
            ServiceManager.instance.StopServiceHook();
        }
    }

    public void ShowOverviewUI()
    {
        DisableAll();

        startedCoroutine = StartCoroutine(StartShowCoroutine());
    }

    public void DisableAll()
    {
        seatText.gameObject.SetActive(false);
        classText.gameObject.SetActive(false);
        classIcon.gameObject.SetActive(false);
        speciesText.gameObject.SetActive(false);
        speciesIcon.gameObject.SetActive(false);
        drinksServedText.gameObject.SetActive(false);
        mealsServedText.gameObject.SetActive(false);
        mealText.gameObject.SetActive(false);
        mealSoldText.gameObject.SetActive(false);
        mealIcon.gameObject.SetActive(false);
        drinkText.gameObject.SetActive(false);
        drinkSoldText.gameObject.SetActive(false);
        drinkIcon.gameObject.SetActive(false);
        goldText.gameObject.SetActive(false);
        goldTrendIcon.gameObject.SetActive(false);
        reputationText.gameObject.SetActive(false);
        reputationTrendIcon.gameObject.SetActive(false);
        continueText.gameObject.SetActive(false);

    }

    public void EnableAll()
    {
        seatText.gameObject.SetActive(true);
        classText.gameObject.SetActive(true);
        classIcon.gameObject.SetActive(true);
        speciesText.gameObject.SetActive(true);
        speciesIcon.gameObject.SetActive(true);
        drinksServedText.gameObject.SetActive(true);
        mealsServedText.gameObject.SetActive(true);
        mealText.gameObject.SetActive(true);
        mealSoldText.gameObject.SetActive(true);
        mealIcon.gameObject.SetActive(true);
        drinkText.gameObject.SetActive(true);
        drinkSoldText.gameObject.SetActive(true);
        drinkIcon.gameObject.SetActive(true);
        goldText.gameObject.SetActive(true);
        goldTrendIcon.gameObject.SetActive(true);
        reputationText.gameObject.SetActive(true);
        reputationTrendIcon.gameObject.SetActive(true);
        continueText.gameObject.SetActive(true);
    }

    public void SkipOverviewAnimation()
    {
        StopCoroutine(startedCoroutine);
        isShowingRolling = false;

        EnableAll();

        // Assign variables to text fields
        // Seats
        seatText.text = ServiceManager.instance.stats.GetFilledSeat().ToString();

        // Class
        KeyValuePair<Class, int> classpair = ServiceManager.instance.stats.GetMostVisitedClass();
        // - Text
        classText.text = $"{classpair.Key.ToString()} ({classpair.Value.ToString()})";
        // - Icon


        // Species
        KeyValuePair<Species, int> speciespair = ServiceManager.instance.stats.GetMostVisitedSpecies();
        // - Text
        speciesText.text = $"{speciespair.Key.ToString()} ({speciespair.Value.ToString()})";
        // - Icon


        // Meals
        mealsServedText.text = ServiceManager.instance.stats.GetServedMeals().ToString();
        // Drinks
        drinksServedText.text = ServiceManager.instance.stats.GetServedDrinks().ToString();

        // Pop meals
        KeyValuePair<MenuItem, int> mealpair = ServiceManager.instance.stats.GetMostServedMeal();
        // - Text
        if (mealpair.Key != null)
        {
            mealText.text = mealpair.Key.itemName;
            mealSoldText.text = $"Amount sold: {mealpair.Value.ToString()}";
        }
        else
        {
            mealText.text = "No meals served";
            mealSoldText.text = $"";
        }
        
        // - Icon


        // Pop drinks
        KeyValuePair<MenuItem, int> drinkpair = ServiceManager.instance.stats.GetMostServedDrink();
        // - Text
        if (drinkpair.Key != null)
        {
            drinkText.text = drinkpair.Key.itemName;
            drinkSoldText.text = $"Amount sold: {drinkpair.Value.ToString()}";
        }
        else
        {
            drinkText.text = "No drinks served";
            drinkSoldText.text = $"";
        }
        // - Icon


        // Gold
        // - Text
        goldText.text = ServiceManager.instance.stats.GetGold().ToString();
        // - Icon

        // Rep
        // - Text
        reputationText.text = ServiceManager.instance.stats.GetReputation().ToString();
        // - Icon

    }

    public IEnumerator StartShowCoroutine()
    {
        isShowingRolling = true;
        yield return new WaitForSeconds(1);

        // Seats filled
        seatText.gameObject.SetActive(true);
        int seats = ServiceManager.instance.stats.GetFilledSeat();
        for (int i = 0; i <= seats; i++)
        {
            seatText.text = i.ToString();
            yield return new WaitForSeconds(0.02f); 
        }
        yield return new WaitForSeconds(0.5f);

        // Most popular with class
        KeyValuePair<Class, int> classpair = ServiceManager.instance.stats.GetMostVisitedClass();
        classText.gameObject.SetActive(true);
        // - Set text
        classText.text = $"{classpair.Key.ToString()} ({classpair.Value.ToString()})";
        // - Set icon
        classIcon.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        // Most popular with species
        KeyValuePair<Species, int> speciespair = ServiceManager.instance.stats.GetMostVisitedSpecies();
        speciesText.gameObject.SetActive(true);
        // - Set text
        speciesText.text = $"{speciespair.Key.ToString()} ({speciespair.Value.ToString()})";
        // - Set icon
        speciesIcon.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        // Meals served
        mealsServedText.gameObject.SetActive(true);
        int meals = ServiceManager.instance.stats.GetServedMeals();
        for (int i = 0; i <= meals; i++)
        {
            mealsServedText.text = i.ToString();
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(0.5f);

        // Drinks served
        drinksServedText.gameObject.SetActive(true);
        int drinks = ServiceManager.instance.stats.GetServedDrinks();
        for (int i = 0; i <= drinks; i++)
        {
            drinksServedText.text = i.ToString();
            yield return new WaitForSeconds(0.02f);
        }

        yield return new WaitForSeconds(1f);

        // Most popular meal
        KeyValuePair<MenuItem, int> mealpair = ServiceManager.instance.stats.GetMostServedMeal();
        mealText.gameObject.SetActive(true);
        mealIcon.gameObject.SetActive(true);
        // - Set text
        if (mealpair.Key != null)
        {
            mealText.text = mealpair.Key.itemName;
            mealSoldText.gameObject.SetActive(true);
            // - Set Text
            mealSoldText.text = $"Amount sold: {mealpair.Value.ToString()}";
        }
        else
        {
            mealText.text = "No meals served";
        }
        // - Set Icon

        

        yield return new WaitForSeconds(1f);

        // Most popular drink
        KeyValuePair<MenuItem, int> drinkpair = ServiceManager.instance.stats.GetMostServedDrink();
        drinkText.gameObject.SetActive(true);
        drinkIcon.gameObject.SetActive(true);
        // - Set text
        if (drinkpair.Key != null)
        {
            drinkText.text = drinkpair.Key.itemName;
            drinkSoldText.gameObject.SetActive(true);
            // - Set Text
            drinkSoldText.text = $"Amount sold: {drinkpair.Value.ToString()}";
        }
        else
        {
            drinkText.text = "No drinks served";
        }
        // - Set Icon


        yield return new WaitForSeconds(0.5f);

        // Gold made
        goldText.gameObject.SetActive(true);
        goldTrendIcon.gameObject.SetActive(true);
        // - Set Text
        int gold = ServiceManager.instance.stats.GetGold();
        for (int i = 0; i <= gold; i++)
        {
            goldText.text = i.ToString();
            yield return new WaitForSeconds(0.02f);
        }
        // - Set Icon


        yield return new WaitForSeconds(0.5f);

        // Reputation gained
        reputationText.gameObject.SetActive(true);
        reputationTrendIcon.gameObject.SetActive(true);
        // - Set Text
        int rep = ServiceManager.instance.stats.GetReputation();
        for (int i = 0; i <= rep; i++)
        {
            reputationText.text = i.ToString();
            yield return new WaitForSeconds(0.02f);
        }
        // - Set Icon


        // Enable button
        continueText.gameObject.SetActive(true);
        isShowingRolling = false;
    }
}
