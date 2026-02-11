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
    private float maxStamina = 100;
    public float currentStamina;

    public bool staminaHasDrained;
    private float staminaDrainSpeed = 0.25f;

    private float sprintSpeed = 3f;

    public float recoveryTimer;
    public float fastRecoveryTimer;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        currentStamina = maxStamina;
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
        if (canMove) 
        {
            Move();
        }
        if (GetStaminaPercent() < 100)
        {
            if (recoveryTimer < 5f)
            {
                recoveryTimer += Time.deltaTime;
            }
            if (fastRecoveryTimer < 5f)
            {
                fastRecoveryTimer += Time.deltaTime;
            } 

            if (recoveryTimer >= 3f && fastRecoveryTimer >= 3f)
            {
                currentStamina += 0.25f;
                if (currentStamina >= maxStamina)
                {
                    currentStamina = maxStamina;
                }
                StaminaBar.instance.SetSliderPercent(GetStaminaPercent());
            }
            else if (recoveryTimer >= 3f)
            {
                currentStamina += 0.1f;
                if (currentStamina >= maxStamina)
                {
                    currentStamina = maxStamina;
                }
                StaminaBar.instance.SetSliderPercent(GetStaminaPercent());
            }

            if (staminaHasDrained && GetStaminaPercent() >= 40)
            {
                staminaHasDrained = false;
                StaminaBar.instance.ImageReset();
            }
        }
        else if(!StaminaBar.instance.isFading)
        {
            StaminaBar.instance.StartFadeout();
        }
    }

    public void Move()
    {
        if (staminaHasDrained)
        {
            transform.position += new Vector3(
                playerControls.General.Move.ReadValue<Vector2>().x * (movementSpeed * 0.75f) * Time.deltaTime,
                playerControls.General.Move.ReadValue<Vector2>().y * (movementSpeed * 0.75f) * Time.deltaTime
                );
            if (playerControls.General.Move.ReadValue<Vector2>() != new Vector2() && playerControls.General.Sprint.IsPressed())
            {
                fastRecoveryTimer = 0f;
            }
            else if (playerControls.General.Move.ReadValue<Vector2>() != new Vector2())
            {
                fastRecoveryTimer = 0f;
            }
        }
        else
        {
            transform.position += new Vector3(
            playerControls.General.Move.ReadValue<Vector2>().x * (movementSpeed + (sprintSpeed * Convert.ToInt32(playerControls.General.Sprint.IsPressed()))) * Time.deltaTime,
            playerControls.General.Move.ReadValue<Vector2>().y * (movementSpeed + (sprintSpeed * Convert.ToInt32(playerControls.General.Sprint.IsPressed()))) * Time.deltaTime
            );
            if (playerControls.General.Move.ReadValue<Vector2>() != new Vector2() && playerControls.General.Sprint.IsPressed())
            {
                recoveryTimer = 0f;
                fastRecoveryTimer = 0f;

                StaminaBar.instance.SetSliderPercent(GetStaminaPercent());
                currentStamina -= staminaDrainSpeed;
                if (currentStamina <= 0)
                {
                    currentStamina = 0;
                    staminaHasDrained = true;
                    StaminaBar.instance.SetRed();
                }
            }
            else if (playerControls.General.Move.ReadValue<Vector2>() != new Vector2())
            {
                fastRecoveryTimer = 0f;
            }
        }

        
    }

    public float GetStaminaPercent()
    {
        return ((currentStamina * 100) / maxStamina);
    }
}
