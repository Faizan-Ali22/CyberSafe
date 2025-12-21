using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// NPC Patrol State - NPC moves between waypoints.
/// </summary>
public class NPCPatrolState : NPCStateBase
{
    /// <summary>
    /// Distance threshold for rotation to prevent jitter at close range.
    /// </summary>
    private const float ROTATION_THRESHOLD = 0.1f;
    
    /// <summary>
    /// Distance threshold to consider waypoint reached.
    /// </summary>
    private const float WAYPOINT_REACHED_THRESHOLD = 0.05f;
    
    private int _currentWaypointIndex = 0;
    private bool _isMoving = true;

    public override void Enter(NPCStateMachineComponent owner)
    {
        Debug.Log($"[NPCPatrolState] {owner.name} starting patrol.");
        owner.SetAnimationTrigger("Walk");
        _currentWaypointIndex = 0;
        _isMoving = true;
    }

    public override void Update(NPCStateMachineComponent owner)
    {
        if (!_isMoving || owner.Waypoints == null || owner.Waypoints.Count == 0) return;

        if (_currentWaypointIndex < owner.Waypoints.Count)
        {
            Transform target = owner.Waypoints[_currentWaypointIndex];
            if (target == null) return;

            // Move towards waypoint
            owner.transform.position = Vector3.MoveTowards(
                owner.transform.position,
                target.position,
                owner.MoveSpeed * Time.deltaTime
            );

            // Rotate towards waypoint
            Vector3 direction = target.position - owner.transform.position;
            float distance = direction.magnitude;

            if (distance > ROTATION_THRESHOLD && direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                owner.transform.rotation = Quaternion.Lerp(
                    owner.transform.rotation,
                    targetRotation,
                    owner.RotationSpeed * Time.deltaTime
                );
            }

            // Check if reached waypoint
            if (distance < WAYPOINT_REACHED_THRESHOLD)
            {
                _currentWaypointIndex++;
                if (owner.IsLooping && _currentWaypointIndex >= owner.Waypoints.Count)
                {
                    _currentWaypointIndex = 0;
                }
            }
        }
    }

    public override void Exit(NPCStateMachineComponent owner)
    {
        _isMoving = false;
        Debug.Log($"[NPCPatrolState] {owner.name} stopping patrol.");
    }
}
