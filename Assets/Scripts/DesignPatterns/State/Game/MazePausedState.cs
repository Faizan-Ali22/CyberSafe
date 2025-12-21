using UnityEngine;

/// <summary>
/// Maze Paused State - Active when the game is paused.
/// </summary>
public class MazePausedState : MazeStateBase
{
    public override void Enter(MazeGameManager owner)
    {
        Debug.Log("[MazePausedState] Game paused.");
        Time.timeScale = 0f;
    }

    public override void Update(MazeGameManager owner)
    {
        // Waiting for unpause
    }

    public override void Exit(MazeGameManager owner)
    {
        Time.timeScale = 1f;
        Debug.Log("[MazePausedState] Game resumed.");
    }
}
