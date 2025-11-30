using UnityEngine;

public class CameraWallCollision : MonoBehaviour
{
     [Header("Target Settings")]
    [SerializeField] private Transform target;
    
    [Header("Distance Settings")]
    [SerializeField] private float defaultDistance = 5f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 10f;
    
    [Header("Collision Settings")]
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float sphereRadius = 0.3f;
    [SerializeField] private float smoothSpeed = 10f;
    
    private float currentDistance;

    private void Start()
    {
        currentDistance = defaultDistance;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Camera target is not assigned!");
            return;
        }

        HandleCameraCollision();
    }

    private void HandleCameraCollision()
    {
        Vector3 targetPosition = target.position;
        Vector3 direction = (transform. position - targetPosition).normalized;
        
        float desiredDistance = defaultDistance;
        
        RaycastHit hit;
        if (Physics.SphereCast(targetPosition, sphereRadius, direction, out hit, maxDistance, collisionMask))
        {
            desiredDistance = Mathf. Clamp(hit.distance - sphereRadius * 2f, minDistance, defaultDistance);
        }
        
        currentDistance = Mathf. Lerp(currentDistance, desiredDistance, Time.deltaTime * smoothSpeed);
        
        Vector3 newPosition = targetPosition + direction * currentDistance;
        transform.position = newPosition;
    }

    private void OnDrawGizmosSelected()
    {
        if (target == null) return;

        Gizmos.color = Color.yellow;
        Vector3 direction = (transform.position - target. position).normalized;
        Gizmos.DrawWireSphere(target.position + direction * currentDistance, sphereRadius);
        Gizmos.DrawLine(target.position, transform.position);
    }
}
