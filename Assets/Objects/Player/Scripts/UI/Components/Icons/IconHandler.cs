using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IconHandler : MonoBehaviour
{
    public static IconHandler instance;

    public GameObject iconPrefab;

    [Header("Species/Classes")]
    public Texture2D anyIcon;
    public List<SpeciesIcon> speciesIcons = new List<SpeciesIcon>();
    public List<ClassIcon> classIcons = new List<ClassIcon>();

    [Header("Stars")]
    public Texture2D fullStar;
    public Texture2D emptyStar;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    [Serializable]
    public struct SpeciesIcon
    {
        public Texture2D icon;
        public Species species;

        public SpeciesIcon(Texture2D icon, Species species)
        {
            this.icon = icon;
            this.species = species;
        }
    }

    [Serializable]
    public struct ClassIcon
    {
        public Texture2D icon;
        public Class classicon;

        public ClassIcon(Texture2D icon, Class classicon)
        {
            this.icon = icon;
            this.classicon = classicon;
        }
    }

    public Texture2D GetIconOnSpecies(Species species)
    {
        return speciesIcons.First(x => x.species == species).icon;
    }

    public Texture2D GetIconOnClass(Class iconclass)
    {
        return classIcons.First(x => x.classicon == iconclass).icon;
    }

    public void MakeIcon(Texture2D icon, GameObject parent)
    {
        GameObject newIcon = Instantiate(iconPrefab);
        newIcon.GetComponent<RawImage>().texture = icon;
        newIcon.transform.SetParent(parent.transform, false);
        newIcon.transform.position = new Vector3(0, 0, 0);
    }
}

