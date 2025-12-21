using UnityEngine;

/// <summary>
/// NPC Follow State - NPC follows a target (usually the player).
/// </summary>
public class NPCFollowState : NPCStateBase
{
    private const float DEFAULT_STOPPING_DISTANCE = 2f;
    
    private float _stoppingDistance = DEFAULT_STOPPING_DISTANCE;
    private bool _isWalking = false;

    public void SetStoppingDistance(float distance)
    {
        _stoppingDistance = distance;
    }

    public override void Enter(NPCStateMachineComponent owner)
    {
        Debug.Log($"[NPCFollowState] {owner.name} starting to follow target.");
        owner.SetAnimationTrigger("Walk");
        _isWalking = true;
    }

    public override void Update(NPCStateMachineComponent owner)
    {
        if (owner.FollowTarget == null) return;

        Vector3 direction = owner.FollowTarget.position - owner.transform.position;
        float distance = direction.magnitude;

        if (distance > _stoppingDistance)
        {
            // Move towards target
            owner.transform.position = Vector3.MoveTowards(
                owner.transform.position,
                owner.FollowTarget.position,
                owner.MoveSpeed * Time.deltaTime
            );

            // Rotate towards target
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                owner.transform.rotation = Quaternion.Lerp(
                    owner.transform.rotation,
                    targetRotation,
                    owner.RotationSpeed * Time.deltaTime
                );
            }

            // Only set animation if state changed
            if (!_isWalking)
            {
                owner.SetAnimationTrigger("Walk");
                _isWalking = true;
            }
        }
        else
        {
            // Only set animation if state changed
            if (_isWalking)
            {
                owner.SetAnimationTrigger("Idle");
                _isWalking = false;
            }
        }
    }

    public override void Exit(NPCStateMachineComponent owner)
    {
        _isWalking = false;
        Debug.Log($"[NPCFollowState] {owner.name} stopped following.");
    }
}
