using UnityEngine;

/// <summary>
/// Maze Intro State - Active while showing the intro popup.
/// </summary>
public class MazeIntroState : MazeStateBase
{
    public override void Enter(MazeGameManager owner)
    {
        Debug.Log("[MazeIntroState] Showing intro...");
    }

    public override void Update(MazeGameManager owner)
    {
        // Intro popup duration is handled by coroutine
    }

    public override void Exit(MazeGameManager owner)
    {
        Debug.Log("[MazeIntroState] Intro complete.");
    }
}
