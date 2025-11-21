using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;
    public PlayerControls playerControls;

    [Header("Movement")]
    private bool canMove = true;
    private float movementSpeed = 5f;

    [Header("Sprinting")]
    private float sprintSpeed = 3f;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }  

    }
    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (canMove) {
            Move();
        }
        
    }

    public void Move()
    {
        transform.position += new Vector3(
            playerControls.General.Move.ReadValue<Vector2>().x * ( movementSpeed + ( sprintSpeed * Convert.ToInt32(playerControls.General.Sprint.IsPressed()) )) * Time.deltaTime, 
            playerControls.General.Move.ReadValue<Vector2>().y * (movementSpeed + (sprintSpeed * Convert.ToInt32(playerControls.General.Sprint.IsPressed()))) * Time.deltaTime
            );

    }
}
