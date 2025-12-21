using UnityEngine;

/// <summary>
/// Player In Cutscene State - Player is in a cutscene, all input disabled.
/// </summary>
public class PlayerInCutsceneState : PlayerStateBase
{
    public override void Enter(PlayerStateMachineComponent owner)
    {
        Debug.Log("[PlayerInCutsceneState] Cutscene started.");
        owner.SetAnimatorValue("Speed", 0f);
        owner.SetMovementEnabled(false);
        owner.SetInputEnabled(false);
    }

    public override void Update(PlayerStateMachineComponent owner)
    {
        // Waiting for cutscene to end
    }

    public override void Exit(PlayerStateMachineComponent owner)
    {
        owner.SetMovementEnabled(true);
        owner.SetInputEnabled(true);
        Debug.Log("[PlayerInCutsceneState] Cutscene ended.");
    }
}
