
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaySomething : MonoBehaviour
{
    public List<string> speechLines = new List<string>()
    {
        "I don't know what to say. But thanks for interacting with me!",
        "I'm a red, little cube! :3"
    };
    public GameObject speechbubble;
    public TextMeshProUGUI speechtext;

    private void Awake()
    {
        speechbubble.SetActive(false);
    }

    public void SaySomethingWithBubble()
    {
        int random = Random.Range(0, speechLines.Count);
        speechbubble.SetActive(true);
        speechtext.text = speechLines[random];

    }

    public void SaySomethingWithBubble(string speechline)
    {
        speechbubble.SetActive(true);
        speechtext.text = speechline;

    }
}
