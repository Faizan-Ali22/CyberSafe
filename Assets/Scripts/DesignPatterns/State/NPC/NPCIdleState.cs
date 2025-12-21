using UnityEngine;

/// <summary>
/// NPC Idle State - NPC stands still and does nothing.
/// </summary>
public class NPCIdleState : NPCStateBase
{
    public override void Enter(NPCStateMachineComponent owner)
    {
        Debug.Log($"[NPCIdleState] {owner.name} is now idle.");
        owner.SetAnimationTrigger("Idle");
    }

    public override void Update(NPCStateMachineComponent owner)
    {
        // Check for conditions to transition to other states
    }

    public override void Exit(NPCStateMachineComponent owner)
    {
        Debug.Log($"[NPCIdleState] {owner.name} leaving idle state.");
    }
}
