using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    //Instancia del singleton para poder referenciarlo globalmente
    public static UserInput Instance;

    public Vector2 moveInput { get; private set; }
    public Vector2 cameraMoveInput { get; private set; }
    public bool backInput { get; private set; }
    public bool selectInput { get; private set; }
    public bool cameraToggled { get; private set; }

    private PlayerInput playerInput;

    private InputAction backAction;
    private InputAction selectAction;
    private InputAction moveAction;
    private InputAction cameraMoveAction;
    private InputAction cameraToggleAction;

    //Configura el objeto como singleton
    private void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerInput = GetComponent<PlayerInput>();

        SetupInputActions();
    }
    private void SetupInputActions()
    {
        backAction = playerInput.actions["Back"];
        selectAction = playerInput.actions["Select"];
        moveAction = playerInput.actions["Move"];
        cameraMoveAction = playerInput.actions["MoveCamera"];
        cameraToggleAction = playerInput.actions["ToggleMouse Camera"];
    }

    private void Update()
    {
        UpdateInputs();
    }
    private void UpdateInputs()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        cameraMoveInput = cameraMoveAction.ReadValue<Vector2>();
        backInput = backAction.WasPressedThisFrame();
        selectInput = selectAction.WasPressedThisFrame();
        cameraToggled = cameraToggleAction.IsPressed();
    }
}
