using UnityEngine;

/// <summary>
/// Maze Playing State - Active during normal gameplay.
/// Handles timer countdown and health drain.
/// </summary>
public class MazePlayingState : MazeStateBase
{
    public override void Enter(MazeGameManager owner)
    {
        Debug.Log("[MazePlayingState] Gameplay started!");
        Time.timeScale = 1f;
    }

    public override void Update(MazeGameManager owner)
    {
        // Timer and health drain are handled by MazeGameManager
        // State just marks that we're in playing mode
    }

    public override void Exit(MazeGameManager owner)
    {
        Debug.Log("[MazePlayingState] Gameplay ended.");
    }
}
