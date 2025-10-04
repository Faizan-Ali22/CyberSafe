using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    PlayerMovement playerMovement;
    CameraManager cameraManager;
    
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundDistance = 0.2f; 
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
        if (groundCheck == null) return;

        // ===== CHANGED =====
        // assign to the class-level isGrounded (not declaring a new local variable)
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        if (isGrounded)
        {
            // Zero small downward velocity to avoid "hover"/slow slide when standing on steps
            if (rb.linearVelocity.y < -0.1f)
            {
                Vector3 vel = rb.linearVelocity;
                vel.y = 0f;
                rb.linearVelocity = vel;
            }
        }
        else
        {
            
            // AddForce with ForceMode.Acceleration:
            // rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
        }
        
    }

    void FixedUpdate()
    {
        playerMovement.HandleAllMovement();
        HandleGravity();
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}