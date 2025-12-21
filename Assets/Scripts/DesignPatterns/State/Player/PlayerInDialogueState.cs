using UnityEngine;

/// <summary>
/// Player In Dialogue State - Player is engaged in dialogue, movement disabled.
/// </summary>
public class PlayerInDialogueState : PlayerStateBase
{
    public override void Enter(PlayerStateMachineComponent owner)
    {
        Debug.Log("[PlayerInDialogueState] Player entered dialogue.");
        owner.SetAnimatorValue("Speed", 0f);
        owner.SetMovementEnabled(false);
    }

    public override void Update(PlayerStateMachineComponent owner)
    {
        // Waiting for dialogue to end
        // DialogueManager will call EndDialogue when done
    }

    public override void Exit(PlayerStateMachineComponent owner)
    {
        owner.SetMovementEnabled(true);
        Debug.Log("[PlayerInDialogueState] Player exited dialogue.");
    }
}
