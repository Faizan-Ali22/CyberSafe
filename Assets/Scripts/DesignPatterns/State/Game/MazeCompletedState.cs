using UnityEngine;

/// <summary>
/// Maze Completed State - Active when all shields are collected.
/// </summary>
public class MazeCompletedState : MazeStateBase
{
    public override void Enter(MazeGameManager owner)
    {
        Debug.Log("[MazeCompletedState] Maze completed! All shields collected.");
    }

    public override void Update(MazeGameManager owner)
    {
        // Waiting for return to lab coroutine
    }

    public override void Exit(MazeGameManager owner)
    {
        Debug.Log("[MazeCompletedState] Transitioning to next scene.");
    }
}
