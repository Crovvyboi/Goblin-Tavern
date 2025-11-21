using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TavernStatManager : MonoBehaviour
{
    public static TavernStatManager instance;

    public int dirtyness;

    [Header("Reputations")]
    public int generalReputation = 20;
    public int basePenalty = 5;
    public int speciesClassPenalty = 2;
    public int daysSkipped = 0;
    public List<SpeciesReputation> speciesReputation;
    public List<ClassReputation> classReputation;

    [Header("ReputationGain")]
    public int zeroStar;
    public int oneStar;
    public int twoStar;
    public int threeStar;
    public int fourStar;
    public int fiveStar;

    [Header("ReputationTiers")]
    public ReputationTier currentTier;
    public List<ReputationTier> reputationTiers = new List<ReputationTier>();

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        UpdateRepTier();
    }

    public void ModifyAfterService(ServiceStats stats)
    {
        ResetDaysSkipped();

        ModifyGenRep(stats);
        ModifyRepForSpecies(stats);
        ModifyRepForClass(stats);

        StandardizeRep();
    }

    public void ModifyGenRep(ServiceStats stats)
    {
        // General Reputation
        int baseRep = 0;

        // Get served menuitems
        // Get rep for each star rating
        int repFromStars = 0;
        // Get rep for total served items
        int repFromServedItems = 0;
        foreach (KeyValuePair<MenuItem, int> item in stats.menuitemsServed)
        {
            switch (item.Key.starrating)
            {
                case 0:
                    repFromStars += zeroStar * item.Value;
                    break;
                case 1:
                    repFromStars += oneStar * item.Value;
                    break;
                case 2:
                    repFromStars += twoStar * item.Value;
                    break;
                case 3:
                    repFromStars += threeStar * item.Value;
                    break;
                case 4:
                    repFromStars += fourStar * item.Value;
                    break;
                case 5:
                    repFromStars += fiveStar * item.Value;
                    break;
                default:
                    repFromStars += zeroStar * item.Value;
                    break;
            }

            repFromServedItems += item.Value;
        }

        // Get all happiness diff for each customer 
        // Get rep for avg happiness
        int totalHappinessGain = 0;
        foreach (CustomerStats customer in stats.customersVisited)
        {
            totalHappinessGain += customer.happiness - customer.startHappiness;
        }

        // Add up reputation gain based on share
        baseRep += (repFromStars) + (repFromServedItems) + (Mathf.RoundToInt(totalHappinessGain * 0.25f));

        generalReputation += baseRep;
        stats.avgReputationGained = baseRep;
    }

    public void ModifyRepForSpecies(ServiceStats stats)
    {
        // Modify rep per species
        int totalCount = stats.customersVisited.Count;
        int avgRepGained = stats.avgReputationGained;

        // Get served items per species
        foreach (Species species in Enum.GetValues(typeof(Species)))
        {
            int customersVisited = stats.customersVisited.Where(x => x.customerSpecies == species).Count();

            if (!speciesReputation.Any(x => x.speciesrep == species))
            {
                SpeciesReputation speciesrep = new SpeciesReputation(species);
                if (customersVisited != 0)
                {
                    speciesrep.reputationScore += Mathf.RoundToInt((float)avgRepGained * ((float)customersVisited / (float)totalCount));
                }
                else
                {
                    speciesrep.reputationScore -= 5;
                }
                speciesReputation.Add(speciesrep);  
            }
            else
            {
                if (customersVisited != 0)
                {
                    speciesReputation.First(x => x.speciesrep == species).reputationScore += Mathf.RoundToInt((float)avgRepGained * ((float)customersVisited / (float)totalCount));
                }
                else
                {
                    speciesReputation.First(x => x.speciesrep == species).reputationScore -= 5;
                }
            }

            if (speciesReputation.First(x => x.speciesrep == species).reputationScore < 0)
            {
                speciesReputation.First(x => x.speciesrep == species).reputationScore = 0;
            }
            else if (speciesReputation.First(x => x.speciesrep == species).reputationScore > 100)
            {
                speciesReputation.First(x => x.speciesrep == species).reputationScore = 100;
            }

        }


    }

    public void ModifyRepForClass(ServiceStats stats)
    {
        // Modify rep per class
        int totalCount = stats.customersVisited.Count;
        int avgRepGained = stats.avgReputationGained;

        // Get served items per species
        foreach (Class repclass in Enum.GetValues(typeof(Class)))
        {
            int customersVisited = stats.customersVisited.Where(x => x.customerClass == repclass).Count();

            if (!classReputation.Any(x => x.classrep == repclass))
            {
                ClassReputation classrepu = new ClassReputation(repclass);
                if (customersVisited != 0)
                {
                    classrepu.reputationScore += Mathf.RoundToInt((float)avgRepGained * ((float)customersVisited / (float)totalCount));
                }
                else
                {
                    classrepu.reputationScore -= 5;
                }

                classReputation.Add(classrepu);
            }
            else
            {
                // classReputation.First(x => x.classrep == repclass).reputationScore += totalRepMod;

                if (customersVisited != 0)
                {
                    classReputation.First(x => x.classrep == repclass).reputationScore += Mathf.RoundToInt((float)avgRepGained * ((float)customersVisited / (float)totalCount));
                }
                else
                {
                    classReputation.First(x => x.classrep == repclass).reputationScore -= 5;
                }
            }

            if (classReputation.First(x => x.classrep == repclass).reputationScore < 0)
            {
                classReputation.First(x => x.classrep == repclass).reputationScore = 0;
            }
            else if (classReputation.First(x => x.classrep == repclass).reputationScore > 1000)
            {
                classReputation.First(x => x.classrep == repclass).reputationScore = 1000;
            }

        }
    }

    public void UpdateRepTier()
    {
        ReputationTier tier = reputationTiers.Where(x => x.reputationTierThreshhold < generalReputation).Last();
        currentTier = tier;
    }

    public void ResetDaysSkipped()
    {
        daysSkipped = 0;
    }

    public void OnDaySkipped()
    {
        OnSkipService();

        // Reduce Species/class reps
        SkippedRep();

        daysSkipped++;
        StandardizeRep();
    }

    public void SkippedRep()
    {
        foreach (Species species in Enum.GetValues(typeof(Species)))
        {
            if (speciesReputation.Any(x => x.speciesrep == species))
            {
                speciesReputation.First(x => x.speciesrep == species).reputationScore -= speciesClassPenalty;
                if (speciesReputation.First(x => x.speciesrep == species).reputationScore < 0)
                {
                    speciesReputation.First(x => x.speciesrep == species).reputationScore = 0;
                }
            }
        }

        foreach (Class repclass in Enum.GetValues(typeof(Class)))
        {
            if (classReputation.Any(x => x.classrep == repclass))
            {
                classReputation.First(x => x.classrep == repclass).reputationScore -= speciesClassPenalty;
                if (classReputation.First(x => x.classrep == repclass).reputationScore < 0)
                {
                    classReputation.First(x => x.classrep == repclass).reputationScore = 0;
                }
            }
        }
    }

    public void OnSkipService()
    {
        // Penalize reputation based on how many days skipped
        int penalty = Mathf.RoundToInt(basePenalty * MathF.Pow(2.5f, (float)daysSkipped));
        generalReputation -= penalty;
    }

    public void StandardizeRep()
    {
        if (generalReputation < 20)
        {
            generalReputation = 20;
        }
    }
}

[Serializable]
public class ReputationTier
{
    public string reputationTierName;
    public int reputationTierThreshhold;
}

[Serializable]
public class Reputation
{
    public int reputationScore;
}
[Serializable]
public class SpeciesReputation : Reputation
{
    public Species speciesrep;

    public SpeciesReputation(Species species)
    {
        this.speciesrep = species;
        this.reputationScore = 0;
    }
}
[Serializable]
public class ClassReputation : Reputation
{
    public Class classrep;

    public ClassReputation(Class repclass)
    {
        this.classrep = repclass;
        this.reputationScore = 0;
    }
}
