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
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        // build movement direction relative to camera
        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection += cameraObject.right * inputManager.horizontalInput;
        moveDirection.y = 0f;

        // normalize only if needed (prevents diagonal > 1 speed)
        if (moveDirection.sqrMagnitude > 1f)
            moveDirection.Normalize();

        // horizontal velocity in units/sec
        Vector3 horizontalVelocity = moveDirection * movementSpeed;

        
        Vector3 newVelocity = horizontalVelocity;
        newVelocity.y = playerRigidbody.linearVelocity.y; 
        playerRigidbody.linearVelocity = newVelocity;
        // ===================
    }

    private void HandleRotation()
    {
        Vector3 targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection += cameraObject.right * inputManager.horizontalInput;
        targetDirection.y = 0f;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }
}
