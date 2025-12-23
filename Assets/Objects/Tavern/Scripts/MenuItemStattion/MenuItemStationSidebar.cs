using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuItemStationSidebar : MonoBehaviour
{
    private PlayerControls playerControls;

    public GameObject closeButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        playerControls.General.PlayerMenu.performed += CloseSideBar;
    }

    public void CloseSideBar(InputAction.CallbackContext input)
    {
        CloseSideBar();
    }

    public void CloseSideBar()
    {
        this.transform.parent.gameObject.SetActive(false);
        this.gameObject.SetActive(false);

        MenuItemStation.instance.OnCloseSidebar();
    }

    public void SetPlayerControls(PlayerControls controls)
    {
        this.playerControls = controls;
    }
}
