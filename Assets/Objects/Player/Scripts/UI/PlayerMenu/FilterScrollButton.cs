using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterScrollButton : MonoBehaviour
{
    public FilterScrollType filterType;
    public Species filterSpecies;
    public Class filterClass;
    public FilterScrollState state;

    public GameObject selectedIcon;

    // Start is called before the first frame update
    void Start()
    {
        ChangeSelectedIcon();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TurnOff()
    {
        state = FilterScrollState.Off;
        ChangeSelectedIcon();
    }

    public void ChangeButtonStateKnownRecipes()
    {
        switch (state)
        {
            case FilterScrollState.Off:
                state = FilterScrollState.Like;
                break;
            case FilterScrollState.Like:
                state = FilterScrollState.Dislike;
                break;
            case FilterScrollState.Dislike:
                state = FilterScrollState.Off;
                break;
            default:
                break;
        }

        ChangeSelectedIcon();

        if (KnownRecipesUI.instance != null)
        {
            KnownRecipesUI.instance.ApplyFilter();
        }
    }

    public void ChangeButtonStateCurrentMenu()
    {
        switch (state)
        {
            case FilterScrollState.Off:
                state = FilterScrollState.Like;
                break;
            case FilterScrollState.Like:
                state = FilterScrollState.Dislike;
                break;
            case FilterScrollState.Dislike:
                state = FilterScrollState.Off;
                break;
            default:
                break;
        }

        ChangeSelectedIcon();

        if (CurrentMenuUI.instance != null)
        {
            CurrentMenuUI.instance.ApplyFilter();
        }
    }

    public void ChangeSelectedIcon()
    {
        switch (state)
        {
            case FilterScrollState.Off:
                selectedIcon.SetActive(false);
                break;
            case FilterScrollState.Like:
                selectedIcon.SetActive(true);
                selectedIcon.GetComponent<RawImage>().color = Color.green;
                break;
            case FilterScrollState.Dislike:
                selectedIcon.SetActive(true);
                selectedIcon.GetComponent<RawImage>().color = Color.red;
                break;
            default:
                break;
        }
    }
}

public enum FilterScrollType
{
    Species,
    Class
}
public enum FilterScrollState
{
    Off,
    Like,
    Dislike
}
