using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TavernMilestones : MonoBehaviour
{
    public static TavernMilestones instance;



    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    #region Liquids
    [Header("Liquid milestones")]
    public bool waterMilestone;
    public bool milkMilestone;
    public bool oilMilestone;

    public bool CheckIfAnyLiquidIsUnlocked()
    {
        if (waterMilestone || milkMilestone || oilMilestone)
        {
            return true;
        }
        return false;
    }

    public bool CheckIfLiquidMilestoneIsAchieved(LiquidSetting liquid)
    {
        switch (liquid)
        {
            case LiquidSetting.Water:
                return waterMilestone;

            case LiquidSetting.Milk:
                return milkMilestone;

            case LiquidSetting.Oil:
                return oilMilestone;

            default:
                return true;
        }
    }
    #endregion

    #region Gems
    [Header("Gem milestones")]
    public bool elvengemMilestone;
    public bool orcishgemMilestone;

    public bool CheckIfAnyGemIsUnlocked()
    {
        if (elvengemMilestone || orcishgemMilestone)
        {
            return true;
        }
        return false;
    }

    public bool CheckIfGemMilestoneIsAchieved(GemSetting gem)
    {
        switch (gem)
        {
            case GemSetting.Elven:
                return elvengemMilestone;

            case GemSetting.Orcish:
                return orcishgemMilestone;

            default:
                return true;
        }
    }

    #endregion

    #region Temperature
    [Header("Temperature")]
    public bool negativeTempMilestone;
    public bool highTempMilestone;
    #endregion
}
