using UnityEngine;

/// <summary>
/// Interact Command - Executes an interaction with an IInteractable object.
/// </summary>
public class InteractCommand : ICommand
{
    private IInteractable _target;
    private bool _wasExecuted = false;

    /// <summary>
    /// Creates a new interact command.
    /// </summary>
    /// <param name="target">The object to interact with.</param>
    public InteractCommand(IInteractable target)
    {
        _target = target;
    }

    /// <summary>
    /// Executes the interaction.
    /// </summary>
    public void Execute()
    {
        if (_target != null && _target.CanInteract())
        {
            _target.Interact();
            _wasExecuted = true;
            Debug.Log($"[InteractCommand] Interacted with {_target}");
        }
    }

    /// <summary>
    /// Undo is not typically supported for interactions.
    /// </summary>
    public void Undo()
    {
        // Most interactions cannot be undone
        Debug.Log("[InteractCommand] Undo not supported for interactions.");
    }

    /// <summary>
    /// Returns whether the interaction can be executed.
    /// </summary>
    public bool CanExecute()
    {
        return _target != null && _target.CanInteract();
    }
}
