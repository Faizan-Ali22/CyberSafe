using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;
    AnimatorManager animatorManager;

    private float moveAmount;
    private Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;
    private Vector2 cameraInput;
    public float cameraInputX;
    public float cameraInputY;
     [Header("Input Flag")]
     public bool interactInput;
     public bool acceptInput = true;


    void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
    }


    void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.CameraMovement.performed += i => cameraInput = i.ReadValue<Vector2>();
            playerControls.PlayerActions.Interact.performed += i => interactInput = true;
            //playerControls.PlayerActions.Interact.canceled += i => interactInput = false;
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
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;
        

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
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
