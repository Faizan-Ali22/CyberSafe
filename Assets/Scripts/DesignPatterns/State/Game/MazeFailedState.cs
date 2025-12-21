using UnityEngine;

/// <summary>
/// Maze Failed State - Active when the player fails (time up or health depleted).
/// </summary>
public class MazeFailedState : MazeStateBase
{
    private string _failReason;

    public void SetFailReason(string reason)
    {
        _failReason = reason;
    }

    public override void Enter(MazeGameManager owner)
    {
        Debug.Log($"[MazeFailedState] Game failed: {_failReason}");
        Time.timeScale = 0f;
    }

    public override void Update(MazeGameManager owner)
    {
        // Waiting for player to retry or quit
    }

    public override void Exit(MazeGameManager owner)
    {
        Time.timeScale = 1f;
        Debug.Log("[MazeFailedState] Exiting failed state.");
    }
}
