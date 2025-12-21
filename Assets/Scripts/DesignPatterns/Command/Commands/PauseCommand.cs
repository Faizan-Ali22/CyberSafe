using UnityEngine;

/// <summary>
/// Pause Command - Pauses or unpauses the game.
/// </summary>
public class PauseCommand : ICommand
{
    private bool _wasPaused;

    /// <summary>
    /// Executes the pause toggle.
    /// </summary>
    public void Execute()
    {
        _wasPaused = Time.timeScale == 0f;
        
        if (_wasPaused)
        {
            Time.timeScale = 1f;
            Debug.Log("[PauseCommand] Game resumed.");
        }
        else
        {
            Time.timeScale = 0f;
            Debug.Log("[PauseCommand] Game paused.");
        }
    }

    /// <summary>
    /// Undoes the pause (reverts to previous state).
    /// </summary>
    public void Undo()
    {
        Time.timeScale = _wasPaused ? 0f : 1f;
        Debug.Log($"[PauseCommand] Reverted to {(_wasPaused ? "paused" : "playing")} state.");
    }

    /// <summary>
    /// Pause can always be executed.
    /// </summary>
    public bool CanExecute()
    {
        return true;
    }
}
