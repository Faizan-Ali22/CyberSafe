using UnityEngine;

/// <summary>
/// Maze Loading State - Active while pre-warming the object pool and initializing.
/// </summary>
public class MazeLoadingState : MazeStateBase
{
    public override void Enter(MazeGameManager owner)
    {
        Debug.Log("[MazeLoadingState] Entering loading state...");
        // Could show a loading indicator here
    }

    public override void Update(MazeGameManager owner)
    {
        // Loading is handled by coroutines in MazeGameManager
        // This state just waits until loading is complete
    }

    public override void Exit(MazeGameManager owner)
    {
        Debug.Log("[MazeLoadingState] Loading complete.");
    }
}
