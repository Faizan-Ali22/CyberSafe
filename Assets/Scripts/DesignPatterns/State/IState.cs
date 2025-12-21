/// <summary>
/// Generic State interface for the State pattern.
/// Defines the contract for all states in a state machine.
/// </summary>
/// <typeparam name="T">The type of the state machine owner/context.</typeparam>
public interface IState<T>
{
    /// <summary>
    /// Called when entering this state.
    /// Use for initialization and setup.
    /// </summary>
    /// <param name="owner">The state machine owner.</param>
    void Enter(T owner);

    /// <summary>
    /// Called every frame while in this state.
    /// Use for state-specific logic and transitions.
    /// </summary>
    /// <param name="owner">The state machine owner.</param>
    void Update(T owner);

    /// <summary>
    /// Called at fixed intervals while in this state.
    /// Use for physics-related updates.
    /// </summary>
    /// <param name="owner">The state machine owner.</param>
    void FixedUpdate(T owner);

    /// <summary>
    /// Called when exiting this state.
    /// Use for cleanup.
    /// </summary>
    /// <param name="owner">The state machine owner.</param>
    void Exit(T owner);
}
