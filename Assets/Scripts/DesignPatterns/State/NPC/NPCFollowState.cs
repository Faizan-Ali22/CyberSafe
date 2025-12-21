using UnityEngine;

/// <summary>
/// NPC Follow State - NPC follows a target (usually the player).
/// </summary>
public class NPCFollowState : NPCStateBase
{
    private float _stoppingDistance = 2f;

    public void SetStoppingDistance(float distance)
    {
        _stoppingDistance = distance;
    }

    public override void Enter(NPCStateMachineComponent owner)
    {
        Debug.Log($"[NPCFollowState] {owner.name} starting to follow target.");
        owner.SetAnimationTrigger("Walk");
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

            owner.SetAnimationTrigger("Walk");
        }
        else
        {
            owner.SetAnimationTrigger("Idle");
        }
    }

    public override void Exit(NPCStateMachineComponent owner)
    {
        Debug.Log($"[NPCFollowState] {owner.name} stopped following.");
    }
}
