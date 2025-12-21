using UnityEngine;

/// <summary>
/// Player Walking State - Player is moving around.
/// </summary>
public class PlayerWalkingState : PlayerStateBase
{
    public override void Enter(PlayerStateMachineComponent owner)
    {
        Debug.Log("[PlayerWalkingState] Player is now walking.");
    }

    public override void Update(PlayerStateMachineComponent owner)
    {
        // Update animation based on movement
        owner.SetAnimatorValue("Speed", owner.MovementMagnitude);

        // Check if player stops moving
        if (!owner.HasMovementInput)
        {
            owner.StateMachine?.ChangeState<PlayerIdleState>();
        }
    }

    public override void Exit(PlayerStateMachineComponent owner)
    {
        // Cleanup if needed
    }
}
