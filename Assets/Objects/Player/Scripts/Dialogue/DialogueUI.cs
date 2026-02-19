using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI instance;
    public PlayerControls playerControls;

    [Header("Dialogue Controls")]
    public bool inDialogue;
    public DialoguePart dialoguePart;
    public int currentLine = 0;

    [Header("UI Elements")]
    public GameObject blackBars;
    public RawImage subjectImage;
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;

    [Header("Choices")]
    public VerticalLayoutGroup choicesVerticalLayoutGroup;
    public GameObject choiceContainer;
    public List<GameObject> choiceObjects;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        gameObject.SetActive(false);
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.General.Interact.performed += NextLine;
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void SetPart(DialoguePart part)
    {
        inDialogue = true;
        dialoguePart = part;
        currentLine = 0;

        SetLine();
    }

    public void NextLine(InputAction.CallbackContext input)
    {
        if (!dialoguePart.lines[currentLine].isChoice)
        {
            currentLine++;
            SetLine();
        }
    }

    public void SetLine()
    {
        if (dialoguePart.lines.Count > currentLine)
        {
            SetText(dialoguePart.lines[currentLine]);
            if (dialoguePart.lines[currentLine].isChoice)
            {
                choicesVerticalLayoutGroup.enabled = true;
                choiceContainer.SetActive(true);

                for (int i = 0; i < choiceObjects.Count; i++)
                {
                    if (i < dialoguePart.lines[currentLine].choices.Count)
                    {
                        choiceObjects[i].SetActive(true);
                        choiceObjects[i].GetComponent<ChoiceObject>().textObject.text = dialoguePart.lines[currentLine].choices[i].text;
                    }
                    else
                    {
                        choiceObjects[i].SetActive(false);
                    }
                }
            }
            else
            {
                choicesVerticalLayoutGroup.enabled = false;
                choiceContainer.SetActive(false);
            }
        }
        else
        {
            CloseUI();
        }
        
    }
    public void SetText(DialogueLine line)
    {
        dialogueText.text = line.text;
    }

    public void SelectOption(int i)
    {
        if (dialoguePart.lines[currentLine].choices[i] != null)
        {
            if (dialoguePart.lines[currentLine].choices[i].nextPart != null)
            {
                SetPart(dialoguePart.lines[currentLine].choices[i].nextPart);
            }
            else
            {
                CloseUI();
            }
        }
        else
        {
            CloseUI();
        }
    }

    public void ActivateUI(DialoguePart part)
    {
        // Disable controls
        PlayerMovement.instance.canMove = false;
        InteractionSender.instance.gameObject.SetActive(false);
        PlayerUIManager.instance.canInteract = false;

        // Show UI elements
        gameObject.SetActive(true);
        blackBars.SetActive(true);
        subjectImage.gameObject.SetActive(true);
        dialogueText.gameObject.SetActive(true);
        dialogueBox.SetActive(true);

        SetPart(part);
    }

    public void CloseUI()
    {
        currentLine = 0;
        dialoguePart = null;

        gameObject.SetActive(false);

        PlayerMovement.instance.canMove = true;
        InteractionSender.instance.gameObject.SetActive(true);
        PlayerUIManager.instance.canInteract = true;

        inDialogue = false;
    }

    
}
