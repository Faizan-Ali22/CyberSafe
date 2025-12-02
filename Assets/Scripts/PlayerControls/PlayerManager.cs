using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    CameraManager cameraManager;
    PlayerMovement playerMovement;
    AnimatorManager animatorManager;

    void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerMovement = GetComponent<PlayerMovement>();
        animatorManager = GetComponent<AnimatorManager>();
        cameraManager = FindFirstObjectByType<CameraManager>();
    }

    void OnEnable()
    {
        // Re-find camera manager if it was disabled during timeline
        if (cameraManager == null)
        {
            cameraManager = FindFirstObjectByType<CameraManager>();
        }
    }

    void Update()
    {
        // Add null checks before accessing components
        if (inputManager == null || playerMovement == null || cameraManager == null)
            return;

        inputManager.HandleAllInputs();
    }

    void FixedUpdate()
    {
        // Add null checks before accessing components
        if (playerMovement == null)
            return;

        playerMovement.HandleAllMovement();
    }

    void LateUpdate()
    {
        // Add null checks before accessing components
        if (cameraManager == null || inputManager == null)
            return;

        cameraManager.HandleAllCameraMovement();
    }
}