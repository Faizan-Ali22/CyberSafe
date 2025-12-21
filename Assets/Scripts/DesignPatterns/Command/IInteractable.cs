using UnityEngine;

/// <summary>
/// Interface for objects that can be interacted with.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Called when the player interacts with this object.
    /// </summary>
    void Interact();

    /// <summary>
    /// Returns whether interaction is currently possible.
    /// </summary>
    bool CanInteract();

    /// <summary>
    /// Gets the interaction prompt text to display.
    /// </summary>
    string GetInteractPrompt();
}
