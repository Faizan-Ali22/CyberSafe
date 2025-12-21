using UnityEngine;

/// <summary>
/// NPC Talking State - NPC is in a dialogue/conversation.
/// </summary>
public class NPCTalkingState : NPCStateBase
{
    public override void Enter(NPCStateMachineComponent owner)
    {
        Debug.Log($"[NPCTalkingState] {owner.name} is talking.");
        owner.SetAnimationTrigger("Talk");
    }

    public override void Update(NPCStateMachineComponent owner)
    {
        // Face the player during conversation
        if (owner.FollowTarget != null)
        {
            Vector3 direction = owner.FollowTarget.position - owner.transform.position;
            direction.y = 0; // Keep upright
            
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                owner.transform.rotation = Quaternion.Lerp(
                    owner.transform.rotation,
                    targetRotation,
                    owner.RotationSpeed * 2f * Time.deltaTime
                );
            }
        }
    }

    public override void Exit(NPCStateMachineComponent owner)
    {
        owner.SetAnimationTrigger("Idle");
        Debug.Log($"[NPCTalkingState] {owner.name} finished talking.");
    }
}
