using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    PlayerMovement playerMovement;
    CameraManager cameraManager;
    public float gravityIntensity = -9.81f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundDistance = 0.4f;
    bool isGrounded;
    void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerMovement = GetComponent<PlayerMovement>();
        cameraManager = FindFirstObjectByType<CameraManager>();
    }

    void Update()
    {

        inputManager.HandleAllInputs();
        cameraManager.HandleAllCameraMovement();
    }
    void HandleGravity()
{
    Rigidbody rb = GetComponent<Rigidbody>();
    bool isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

    if (!isGrounded)
    {
        // Apply gravity directly for fast, realistic falling
        rb.linearVelocity += Vector3.up * gravityIntensity * Time.deltaTime;
    }
    else
    {
        // Optionally, zero out vertical velocity when grounded
        Vector3 vel = rb.linearVelocity;
        if (vel.y < 0)
        {
            vel.y = 0;
            rb.linearVelocity = vel;
        }
    }
}
    void FixedUpdate()
    {
        playerMovement.HandleAllMovement();

        HandleGravity();
    }
}
