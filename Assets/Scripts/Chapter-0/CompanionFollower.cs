using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class CompanionFollower : MonoBehaviour
{
    [Header("Follow Target")]
    [SerializeField] private Transform target;
    [SerializeField] private float safeDistance = 2.5f;
    [SerializeField] private float followDistance = 5.0f;
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private Vector3 localOffset = new Vector3(0, 0, -1.5f);

    [Header("Boundary (optional)")]
    [SerializeField] private bool useBoundary = false;
    [SerializeField] private Vector3 boundaryCenter = Vector3.zero;
    [SerializeField] private Vector3 boundarySize = new Vector3(20, 10, 20);

    [Header("Separation")]
    [SerializeField] private List<CompanionFollower> otherFollowers = new List<CompanionFollower>();
    [SerializeField] private float separationDistance = 1.0f;
    [SerializeField] private float separationStrength = 1.0f;
    [SerializeField] private float minDistanceToTarget = 1.5f;

    [Header("Grounding")]
    [SerializeField] private bool enableGroundCheck = true;
    [SerializeField] private float groundCheckOffset = 0.5f;
    [SerializeField] private float groundCheckDistance = 1.2f;
    [SerializeField] private LayerMask groundLayers = ~0;
    [SerializeField] private float groundSnapSpeed = 20f;

    [Header("Animation Params")]
    [SerializeField] private string walkBoolParam = "isWalking";

    [Header("Debug Gizmos")]
    [SerializeField] private bool showGizmos = true;

    private Animator anim;
    private Vector3 lastPos;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        lastPos = transform.position;
    }

    private void Update()
    {
        if (target == null) return;

        Vector3 desired = target.TransformPoint(localOffset);
        Vector3 toDest = desired - transform.position;
        float dist = toDest.magnitude;

        // STRONG separation from other followers
        Vector3 separation = Vector3.zero;
        foreach (var other in otherFollowers)
        {
            if (other == null || other == this) continue;
            Vector3 diff = transform.position - other.transform.position;
            float d = diff.magnitude;
            
            // Enforce minimum distance more aggressively
            if (d < separationDistance)
            {
                if (d < 0.1f) d = 0.1f; // prevent divide by zero
                float force = Mathf.Pow((separationDistance - d) / separationDistance, 2);
                separation += (diff / d) * force * separationStrength;
            }
        }

        // STRONG push away from target if too close
        Vector3 toTarget = transform.position - target.position;
        float distToTarget = toTarget.magnitude;
        if (distToTarget < minDistanceToTarget)
        {
            if (distToTarget < 0.1f) distToTarget = 0.1f;
            float force = Mathf.Pow((minDistanceToTarget - distToTarget) / minDistanceToTarget, 2);
            separation += (toTarget / distToTarget) * force * separationStrength;
        }

        // Movement decision
        Vector3 moveDir = Vector3.zero;
        
        // Always apply separation first
        moveDir += separation;
        
        // Then add following behavior
        if (dist > followDistance)
        {
            moveDir += toDest.normalized;
        }
        else if (dist < safeDistance)
        {
            moveDir += (transform.position - target.position).normalized;
        }

        Vector3 newPos = transform.position;
        if (moveDir.sqrMagnitude > 0.0001f)
        {
            Vector3 dir = moveDir.normalized;
            newPos = Vector3.MoveTowards(transform.position, transform.position + dir, moveSpeed * Time.deltaTime);

            Quaternion look = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z), Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
        }

        // Ground check & snap
        if (enableGroundCheck)
        {
            float targetY = newPos.y;
            Vector3 rayOrigin = newPos + Vector3.up * groundCheckOffset;
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit,
                                groundCheckOffset + groundCheckDistance,
                                groundLayers, QueryTriggerInteraction.Ignore))
            {
                targetY = hit.point.y;
            }
            newPos.y = Mathf.Lerp(transform.position.y, targetY, groundSnapSpeed * Time.deltaTime);
        }

        // Boundary clamp
        if (useBoundary)
        {
            Vector3 half = boundarySize * 0.5f;
            newPos.x = Mathf.Clamp(newPos.x, boundaryCenter.x - half.x, boundaryCenter.x + half.x);
            newPos.y = Mathf.Clamp(newPos.y, boundaryCenter.y - half.y, boundaryCenter.y + half.y);
            newPos.z = Mathf.Clamp(newPos.z, boundaryCenter.z - half.z, boundaryCenter.z + half.z);
        }

        // HARD ENFORCE: Don't allow position closer than minimum distances
        Vector3 finalToTarget = newPos - target.position;
        if (finalToTarget.magnitude < minDistanceToTarget)
        {
            newPos = target.position + finalToTarget.normalized * minDistanceToTarget;
        }

        foreach (var other in otherFollowers)
        {
            if (other == null || other == this) continue;
            Vector3 finalDiff = newPos - other.transform.position;
            if (finalDiff.magnitude < separationDistance)
            {
                newPos = other.transform.position + finalDiff.normalized * separationDistance;
            }
        }

        transform.position = newPos;

        // Animation
        float currentSpeed = (transform.position - lastPos).magnitude / Time.deltaTime;
        float normalized = moveSpeed > 0.01f ? currentSpeed / moveSpeed : 0f;

        if (!string.IsNullOrEmpty(walkBoolParam))
            anim.SetBool(walkBoolParam, normalized > 0.1f);

        lastPos = transform.position;
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Vector3 pos = transform.position;

        // Draw companion position
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(pos, 0.3f);

        if (target != null)
        {
            // Desired follow point
            Vector3 desired = target.TransformPoint(localOffset);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(desired, 0.25f);
            Gizmos.DrawLine(pos, desired);

            // Min distance to target (red circle around target)
            Gizmos.color = Color.red;
            DrawCircleXZ(target.position, minDistanceToTarget);

            // Safe distance (yellow circle around target)
            Gizmos.color = Color.yellow;
            DrawCircleXZ(target.position, safeDistance);

            // Follow distance (green circle around target)
            Gizmos.color = Color.green;
            DrawCircleXZ(target.position, followDistance);

            // Line to target
            float distToTarget = Vector3.Distance(pos, target.position);
            if (distToTarget < minDistanceToTarget)
                Gizmos.color = Color.red;
            else if (distToTarget < safeDistance)
                Gizmos.color = Color.yellow;
            else if (distToTarget > followDistance)
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.cyan;
            
            Gizmos.DrawLine(pos, target.position);
        }

        // Separation circles from other followers
        foreach (var other in otherFollowers)
        {
            if (other == null || other == this) continue;

            float d = Vector3.Distance(pos, other.transform.position);
            
            // Draw separation zone around this companion
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // orange
            DrawCircleXZ(pos, separationDistance);

            // Line to other follower
            if (d < separationDistance)
                Gizmos.color = Color.red; // too close!
            else
                Gizmos.color = Color.gray;
            
            Gizmos.DrawLine(pos, other.transform.position);
        }

        // Boundary box
        if (useBoundary)
        {
            Gizmos.color = new Color(0f, 0.7f, 1f, 0.25f);
            Gizmos.DrawCube(boundaryCenter, boundarySize);
            Gizmos.color = new Color(0f, 0.7f, 1f, 1f);
            Gizmos.DrawWireCube(boundaryCenter, boundarySize);
        }

        // Ground check ray
        if (enableGroundCheck && Application.isPlaying)
        {
            Vector3 rayOrigin = pos + Vector3.up * groundCheckOffset;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * (groundCheckOffset + groundCheckDistance));
        }
    }

    private void DrawCircleXZ(Vector3 center, float radius, int segments = 32)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * angleStep * i;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}
