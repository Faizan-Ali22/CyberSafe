using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic State Machine implementation.
/// Manages state transitions and delegates lifecycle methods to the current state.
/// </summary>
/// <typeparam name="T">The type of the state machine owner/context.</typeparam>
public class StateMachine<T>
{
    private T _owner;
    private IState<T> _currentState;
    private IState<T> _previousState;
    private Dictionary<Type, IState<T>> _states = new Dictionary<Type, IState<T>>();
    private bool _isTransitioning = false;

    /// <summary>
    /// Event raised when a state transition occurs.
    /// Parameters: (previousState, newState)
    /// </summary>
    public event Action<IState<T>, IState<T>> OnStateChanged;

    /// <summary>
    /// Gets the current state.
    /// </summary>
    public IState<T> CurrentState => _currentState;

    /// <summary>
    /// Gets the previous state.
    /// </summary>
    public IState<T> PreviousState => _previousState;

    /// <summary>
    /// Gets the type of the current state.
    /// </summary>
    public Type CurrentStateType => _currentState?.GetType();

    /// <summary>
    /// Returns true if currently transitioning between states.
    /// </summary>
    public bool IsTransitioning => _isTransitioning;

    /// <summary>
    /// Creates a new state machine for the specified owner.
    /// </summary>
    /// <param name="owner">The owner/context of the state machine.</param>
    public StateMachine(T owner)
    {
        _owner = owner;
    }

    /// <summary>
    /// Registers a state with the state machine.
    /// </summary>
    /// <typeparam name="TState">The type of state to register.</typeparam>
    /// <param name="state">The state instance.</param>
    public void RegisterState<TState>(TState state) where TState : IState<T>
    {
        var type = typeof(TState);
        if (!_states.ContainsKey(type))
        {
            _states[type] = state;
        }
        else
        {
            Debug.LogWarning($"[StateMachine] State {type.Name} is already registered.");
        }
    }

    /// <summary>
    /// Gets a registered state by type.
    /// </summary>
    /// <typeparam name="TState">The type of state to get.</typeparam>
    /// <returns>The state instance, or null if not found.</returns>
    public TState GetState<TState>() where TState : class, IState<T>
    {
        var type = typeof(TState);
        if (_states.TryGetValue(type, out var state))
        {
            return state as TState;
        }
        return null;
    }

    /// <summary>
    /// Checks if the current state is of the specified type.
    /// </summary>
    /// <typeparam name="TState">The state type to check.</typeparam>
    /// <returns>True if current state matches the type.</returns>
    public bool IsInState<TState>() where TState : IState<T>
    {
        return _currentState is TState;
    }

    /// <summary>
    /// Transitions to a new state by type.
    /// </summary>
    /// <typeparam name="TState">The type of state to transition to.</typeparam>
    public void ChangeState<TState>() where TState : IState<T>
    {
        var type = typeof(TState);
        if (_states.TryGetValue(type, out var state))
        {
            ChangeState(state);
        }
        else
        {
            Debug.LogError($"[StateMachine] State {type.Name} is not registered.");
        }
    }

    /// <summary>
    /// Transitions to a new state instance.
    /// </summary>
    /// <param name="newState">The state to transition to.</param>
    public void ChangeState(IState<T> newState)
    {
        if (newState == null)
        {
            Debug.LogError("[StateMachine] Cannot change to null state.");
            return;
        }

        if (_isTransitioning)
        {
            Debug.LogWarning("[StateMachine] Already transitioning. Ignoring state change request.");
            return;
        }

        _isTransitioning = true;

        _previousState = _currentState;

        // Exit current state
        _currentState?.Exit(_owner);

        // Change to new state
        _currentState = newState;

        // Enter new state
        _currentState.Enter(_owner);

        _isTransitioning = false;

        OnStateChanged?.Invoke(_previousState, _currentState);

        Debug.Log($"[StateMachine] State changed: {_previousState?.GetType().Name ?? "None"} -> {_currentState.GetType().Name}");
    }

    /// <summary>
    /// Returns to the previous state.
    /// </summary>
    public void ReturnToPreviousState()
    {
        if (_previousState != null)
        {
            ChangeState(_previousState);
        }
        else
        {
            Debug.LogWarning("[StateMachine] No previous state to return to.");
        }
    }

    /// <summary>
    /// Updates the current state. Call this from MonoBehaviour.Update().
    /// </summary>
    public void Update()
    {
        if (!_isTransitioning)
        {
            _currentState?.Update(_owner);
        }
    }

    /// <summary>
    /// Fixed update for the current state. Call this from MonoBehaviour.FixedUpdate().
    /// </summary>
    public void FixedUpdate()
    {
        if (!_isTransitioning)
        {
            _currentState?.FixedUpdate(_owner);
        }
    }

    /// <summary>
    /// Sets the initial state without triggering Exit on any previous state.
    /// </summary>
    /// <typeparam name="TState">The initial state type.</typeparam>
    public void SetInitialState<TState>() where TState : IState<T>
    {
        var type = typeof(TState);
        if (_states.TryGetValue(type, out var state))
        {
            _currentState = state;
            _currentState.Enter(_owner);
            Debug.Log($"[StateMachine] Initial state set: {type.Name}");
        }
        else
        {
            Debug.LogError($"[StateMachine] State {type.Name} is not registered.");
        }
    }
}
