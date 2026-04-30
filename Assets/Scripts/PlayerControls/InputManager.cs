using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;
    AnimatorManager animatorManager;
    MalwareMazeGameManager gameManager;

    [Header("Movement Input Variables")]
    private float moveAmount;
    private Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;
    private Vector2 cameraInput;
    public float cameraInputX;
    public float cameraInputY;

    [Header("Joystick Flags")]
    public FixedJoystick movementJoystick;
    public FixedJoystick cameraJoystick;
    
    [Header("Sensitivity")]
    [SerializeField] public float joystickCameraSensitivity = 50.0f;

    [Header("Input Flag")]
    public bool interactInput;
    public bool acceptInput = true;

    void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        gameManager = FindFirstObjectByType<MalwareMazeGameManager>();
    }

    void Update()
    {
        HandleAllInputs();
    }

    void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            // Subscribe to performed events
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.CameraMovement.performed += i => cameraInput = i.ReadValue<Vector2>();
            playerControls.PlayerActions.Interact. performed += i => interactInput = true;

            // Subscribe to canceled events to reset input when released
            playerControls.PlayerMovement.Movement.canceled += i => movementInput = Vector2.zero;
            playerControls.PlayerMovement.CameraMovement. canceled += i => cameraInput = Vector2.zero;
        }
        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        if (!acceptInput) return;
        HandleMovementInput();
        HandleInteractInput();
    }

    public void HandleMovementInput()
    {
        // Get keyboard/gamepad input from Input System
        float keyboardVertical = movementInput.y;
        float keyboardHorizontal = movementInput. x;
        float keyboardCameraX = cameraInput.x;
        float keyboardCameraY = cameraInput.y;

        // Get joystick input (if joysticks exist)
        float joystickVertical = movementJoystick ?  movementJoystick. Vertical : 0f;
        float joystickHorizontal = movementJoystick ? movementJoystick.Horizontal : 0f;
        float joystickCameraX = cameraJoystick ? cameraJoystick.Horizontal * joystickCameraSensitivity : 0f;
        float joystickCameraY = cameraJoystick ? cameraJoystick.Vertical * joystickCameraSensitivity : 0f;

        // Combine both inputs (keyboard + joystick) and clamp to [-1, 1]
        verticalInput = Mathf. Clamp(keyboardVertical + joystickVertical, -1f, 1f);
        horizontalInput = Mathf.Clamp(keyboardHorizontal + joystickHorizontal, -1f, 1f);

        cameraInputX = keyboardCameraX + joystickCameraX;
        cameraInputY = keyboardCameraY + joystickCameraY;

        // Calculate move amount for animator
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf. Abs(verticalInput));
        animatorManager.UpdateAnimatorValues(0, moveAmount);
    }

    private void HandleInteractInput()
    {
        if (interactInput)
        {
            Debug.Log("Interacting");
            interactInput = false;
        }
    }
}