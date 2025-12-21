using UnityEngine;

/// <summary>
/// Base class for all NPC states.
/// </summary>
public abstract class NPCStateBase : IState<NPCStateMachineComponent>
{
    public abstract void Enter(NPCStateMachineComponent owner);
    public abstract void Update(NPCStateMachineComponent owner);
    public virtual void FixedUpdate(NPCStateMachineComponent owner) { }
    public abstract void Exit(NPCStateMachineComponent owner);
}
