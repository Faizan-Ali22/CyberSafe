/// <summary>
/// Command interface for the Command pattern.
/// Defines the contract for all executable commands.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Executes the command.
    /// </summary>
    void Execute();

    /// <summary>
    /// Undoes the command (reverts the action).
    /// </summary>
    void Undo();

    /// <summary>
    /// Returns whether the command can currently be executed.
    /// </summary>
    /// <returns>True if the command can be executed.</returns>
    bool CanExecute();
}
