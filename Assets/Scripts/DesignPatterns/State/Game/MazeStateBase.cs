using UnityEngine;

/// <summary>
/// Base class for all Maze game states.
/// Provides common functionality for maze-specific states.
/// </summary>
public abstract class MazeStateBase : IState<MazeGameManager>
{
    /// <summary>
    /// Called when entering this state.
    /// </summary>
    public abstract void Enter(MazeGameManager owner);

    /// <summary>
    /// Called every frame while in this state.
    /// </summary>
    public abstract void Update(MazeGameManager owner);

    /// <summary>
    /// Called at fixed intervals while in this state.
    /// </summary>
    public virtual void FixedUpdate(MazeGameManager owner) { }

    /// <summary>
    /// Called when exiting this state.
    /// </summary>
    public abstract void Exit(MazeGameManager owner);
}
