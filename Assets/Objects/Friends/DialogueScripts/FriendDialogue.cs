using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class FriendDialogue : MonoBehaviour
{
    public List<DialoguePart> openings;

    public void OpenDialogue()
    {
        if (openings.Count > 0)
        {
            // Select opening dialogue part
            DialoguePart openingPart = openings[UnityEngine.Random.Range(0, openings.Count())];

            // Open dialogue interface
            DialogueUI.instance.ActivateUI(openingPart);

        }

    }

    public void CloseDialogue()
    {

    }
}

[Serializable]
[CreateAssetMenu(menuName = "Dialogue/DialoguePart")]
public class DialoguePart : ScriptableObject
{
    public bool isOpening;

    public List<DialogueLine> lines;
}

[Serializable]
public class DialogueLine 
{
    public string text;
    // <- Insert emotion


    public bool isChoice;
    public List<DialogueChoice> choices;
}

[Serializable]
public class DialogueChoice 
{
    public string text;
    public DialoguePart nextPart;
}


