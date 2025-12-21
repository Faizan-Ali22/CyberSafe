using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Command Invoker - Executes and tracks command history for undo/redo.
/// </summary>
public class CommandInvoker : MonoBehaviour
{
    private static CommandInvoker _instance;
    
    /// <summary>
    /// Gets the singleton instance of the CommandInvoker.
    /// </summary>
    public static CommandInvoker Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<CommandInvoker>();
                if (_instance == null)
                {
                    var go = new GameObject("CommandInvoker");
                    _instance = go.AddComponent<CommandInvoker>();
                }
            }
            return _instance;
        }
    }

    [Header("Settings")]
    [SerializeField] private int _maxHistorySize = 100;

    private Stack<ICommand> _undoStack = new Stack<ICommand>();
    private Stack<ICommand> _redoStack = new Stack<ICommand>();

    /// <summary>
    /// Gets the number of commands that can be undone.
    /// </summary>
    public int UndoCount => _undoStack.Count;

    /// <summary>
    /// Gets the number of commands that can be redone.
    /// </summary>
    public int RedoCount => _redoStack.Count;

    /// <summary>
    /// Returns true if there are commands to undo.
    /// </summary>
    public bool CanUndo => _undoStack.Count > 0;

    /// <summary>
    /// Returns true if there are commands to redo.
    /// </summary>
    public bool CanRedo => _redoStack.Count > 0;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    /// <summary>
    /// Executes a command and adds it to the history.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <returns>True if the command was executed.</returns>
    public bool ExecuteCommand(ICommand command)
    {
        if (command == null) return false;

        if (!command.CanExecute())
        {
            Debug.LogWarning("[CommandInvoker] Command cannot be executed.");
            return false;
        }

        command.Execute();
        
        // Add to undo stack
        _undoStack.Push(command);
        
        // Clear redo stack (new action breaks redo chain)
        _redoStack.Clear();

        // Limit history size
        if (_undoStack.Count > _maxHistorySize)
        {
            // Remove oldest command (convert to array, remove first, rebuild stack)
            var commands = _undoStack.ToArray();
            _undoStack.Clear();
            for (int i = commands.Length - 2; i >= 0; i--)
            {
                _undoStack.Push(commands[i]);
            }
        }

        Debug.Log($"[CommandInvoker] Executed: {command.GetType().Name}");
        return true;
    }

    /// <summary>
    /// Undoes the last executed command.
    /// </summary>
    /// <returns>True if a command was undone.</returns>
    public bool Undo()
    {
        if (_undoStack.Count == 0)
        {
            Debug.LogWarning("[CommandInvoker] Nothing to undo.");
            return false;
        }

        var command = _undoStack.Pop();
        command.Undo();
        _redoStack.Push(command);

        Debug.Log($"[CommandInvoker] Undone: {command.GetType().Name}");
        return true;
    }

    /// <summary>
    /// Redoes the last undone command.
    /// </summary>
    /// <returns>True if a command was redone.</returns>
    public bool Redo()
    {
        if (_redoStack.Count == 0)
        {
            Debug.LogWarning("[CommandInvoker] Nothing to redo.");
            return false;
        }

        var command = _redoStack.Pop();
        
        if (command.CanExecute())
        {
            command.Execute();
            _undoStack.Push(command);
            Debug.Log($"[CommandInvoker] Redone: {command.GetType().Name}");
            return true;
        }

        Debug.LogWarning($"[CommandInvoker] Cannot redo: {command.GetType().Name}");
        return false;
    }

    /// <summary>
    /// Clears all command history.
    /// </summary>
    public void ClearHistory()
    {
        _undoStack.Clear();
        _redoStack.Clear();
        Debug.Log("[CommandInvoker] History cleared.");
    }
}
