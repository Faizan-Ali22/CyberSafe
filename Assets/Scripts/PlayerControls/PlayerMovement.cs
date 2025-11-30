using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
   [Header("Scripts")]
    InputManager inputManager;
    [Header("Movement")]
    Vector3 moveDirection;
    public Transform cameraObject;
    Rigidbody playerRigidbody;
    
    public float movementSpeed = 5f;
    public float rotationSpeed = 12f;
    
    void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        CacheCamera();
    }

    void LateUpdate()
    {
        // if timeline switched cameras, update the reference
        if (Camera.main && cameraObject != Camera.main.transform)
        {
            CacheCamera();
        }
    }

    private void CacheCamera()
    {
        cameraObject = Camera.main ? Camera.main.transform : null;
    }

    public void HandleAllMovement()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        // ALWAYS recalculate movement direction relative to CURRENT camera orientation
        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection += cameraObject.right * inputManager.horizontalInput;
        moveDirection.y = 0f;

        if (moveDirection.sqrMagnitude > 1f)
            moveDirection.Normalize();

        Vector3 horizontalVelocity = moveDirection * movementSpeed;
        Vector3 newVelocity = horizontalVelocity;
        newVelocity.y = playerRigidbody.linearVelocity.y; 
        playerRigidbody.linearVelocity = newVelocity;
    }

    private void HandleRotation()
    {
        // Use the SAME moveDirection that was just calculated in HandleMovement
        if (moveDirection.sqrMagnitude < 0.01f)
        {
            return; // No input - don't rotate
        }

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }
}
