using UnityEngine;

/// <summary>
/// Player Idle State - Player is standing still.
/// </summary>
public class PlayerIdleState : PlayerStateBase
{
    public override void Enter(PlayerStateMachineComponent owner)
    {
        Debug.Log("[PlayerIdleState] Player is now idle.");
        owner.SetAnimatorValue("Speed", 0f);
    }

    public override void Update(PlayerStateMachineComponent owner)
    {
        // Check if player starts moving
        if (owner.HasMovementInput)
        {
            owner.StateMachine?.ChangeState<PlayerWalkingState>();
        }
    }

    public override void Exit(PlayerStateMachineComponent owner)
    {
        // Cleanup if needed
    }
}
