using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionSender : MonoBehaviour
{
    public static InteractionSender instance;

    public bool canInteract = true;
    private PlayerControls playerControls;

    private Collider2D interactCollider;

    private bool isInTrigger = false;
    public Collider2D collisionObject;

    // Start is called before the first frame update
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Awake()
    {
        playerControls = new PlayerControls();
        interactCollider = GetComponent<Collider2D>();
    }
    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.General.Interact.performed += OnInteract;
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void FixedUpdate()
    {
        
    }

    public void OnInteract(InputAction.CallbackContext input)
    {
        if (canInteract)
        {
            if (collisionObject != null && collisionObject.GetComponent<InteractionReceiver>() != null)
            {
                canInteract = false;
                collisionObject.GetComponent<InteractionReceiver>().OnInteract(this);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        isInTrigger = true;
        collisionObject = collision;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isInTrigger = false;
        collisionObject = null;
    }

    
}
