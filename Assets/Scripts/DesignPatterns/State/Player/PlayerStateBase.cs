using UnityEngine;

/// <summary>
/// Base class for all Player states.
/// </summary>
public abstract class PlayerStateBase : IState<PlayerStateMachineComponent>
{
    public abstract void Enter(PlayerStateMachineComponent owner);
    public abstract void Update(PlayerStateMachineComponent owner);
    public virtual void FixedUpdate(PlayerStateMachineComponent owner) { }
    public abstract void Exit(PlayerStateMachineComponent owner);
}
