using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DiscoveryPopupMenuItemStation : MonoBehaviour
{
    private PlayerControls playerControls;
    public CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();

        this.gameObject.SetActive(false);
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();

        playerControls.General.Interact.performed += ClosePopup;
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void ShowPopup()
    {
        this.gameObject.SetActive(true);
        canvasGroup.alpha = 1;
    }

    public void ClosePopup(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            canvasGroup.alpha = 0;

            this.gameObject.SetActive(false);
        }
    }
}
