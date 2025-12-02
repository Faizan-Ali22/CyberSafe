using UnityEngine;

public class CameraManager : MonoBehaviour
{
    InputManager inputManager;  
    public Transform playerTransform;
    public Transform cameraPivot;
    private Vector3 cameraFollowVelocity = Vector3.zero;
    
    [Header("Camera Movement and Rotation")]
    public float cameraFollowSpeed = 0.1f;
    public float cameraLookSpeed = 0.1f;
    public float cameraPivotSpeed = 0.1f;
    public float lookAngle;
    public float pivotAngle;
    public float minimumPivot = -30f;
    public float maximumPivot = 30f;

    void Awake()
    {
        inputManager = FindFirstObjectByType<InputManager>();
        playerTransform = FindFirstObjectByType<PlayerManager>()?.transform;
    }

    public void HandleAllCameraMovement()
    {
        if (playerTransform == null || inputManager == null) return;
        
        FollowTarget();
        RotateCamera();
    }

    void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, playerTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);
        transform.position = targetPosition;
    }

    private void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;

        lookAngle = lookAngle + (inputManager.cameraInputX * cameraLookSpeed);
        pivotAngle = pivotAngle - (inputManager.cameraInputY * cameraPivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }
}
