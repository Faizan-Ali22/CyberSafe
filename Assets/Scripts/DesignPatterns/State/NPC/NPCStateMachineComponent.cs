using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC State Machine Component - Manages NPC states and behavior.
/// Add this component to NPCs to enable state-based behavior.
/// </summary>
public class NPCStateMachineComponent : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _rotationSpeed = 10f;

    [Header("Patrol Settings")]
    [SerializeField] private List<Transform> _waypoints = new List<Transform>();
    [SerializeField] private bool _isLooping = true;

    [Header("Follow Settings")]
    [SerializeField] private Transform _followTarget;

    [Header("Animation")]
    [SerializeField] private Animator _animator;

    [Header("Initial State")]
    [SerializeField] private NPCInitialState _initialState = NPCInitialState.Idle;

    private StateMachine<NPCStateMachineComponent> _stateMachine;

    // Public accessors for states
    public float MoveSpeed => _moveSpeed;
    public float RotationSpeed => _rotationSpeed;
    public List<Transform> Waypoints => _waypoints;
    public bool IsLooping => _isLooping;
    public Transform FollowTarget => _followTarget;

    /// <summary>
    /// Gets the underlying state machine.
    /// </summary>
    public StateMachine<NPCStateMachineComponent> StateMachine => _stateMachine;

    private void Awake()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }

        InitializeStateMachine();
    }

    private void Update()
    {
        _stateMachine?.Update();
    }

    private void FixedUpdate()
    {
        _stateMachine?.FixedUpdate();
    }

    private void InitializeStateMachine()
    {
        _stateMachine = new StateMachine<NPCStateMachineComponent>(this);

        // Register all states
        _stateMachine.RegisterState(new NPCIdleState());
        _stateMachine.RegisterState(new NPCPatrolState());
        _stateMachine.RegisterState(new NPCFollowState());
        _stateMachine.RegisterState(new NPCTalkingState());

        // Set initial state
        switch (_initialState)
        {
            case NPCInitialState.Patrol:
                _stateMachine.SetInitialState<NPCPatrolState>();
                break;
            case NPCInitialState.Follow:
                _stateMachine.SetInitialState<NPCFollowState>();
                break;
            case NPCInitialState.Talking:
                _stateMachine.SetInitialState<NPCTalkingState>();
                break;
            default:
                _stateMachine.SetInitialState<NPCIdleState>();
                break;
        }
    }

    /// <summary>
    /// Sets an animation trigger.
    /// </summary>
    /// <param name="trigger">Trigger name.</param>
    public void SetAnimationTrigger(string trigger)
    {
        if (_animator != null && !string.IsNullOrEmpty(trigger))
        {
            _animator.SetTrigger(trigger);
        }
    }

    /// <summary>
    /// Sets the follow target.
    /// </summary>
    /// <param name="target">Target transform to follow.</param>
    public void SetFollowTarget(Transform target)
    {
        _followTarget = target;
    }

    /// <summary>
    /// Changes to idle state.
    /// </summary>
    public void GoIdle()
    {
        _stateMachine?.ChangeState<NPCIdleState>();
    }

    /// <summary>
    /// Changes to patrol state.
    /// </summary>
    public void StartPatrol()
    {
        _stateMachine?.ChangeState<NPCPatrolState>();
    }

    /// <summary>
    /// Changes to follow state.
    /// </summary>
    /// <param name="target">Target to follow.</param>
    public void StartFollowing(Transform target)
    {
        _followTarget = target;
        _stateMachine?.ChangeState<NPCFollowState>();
    }

    /// <summary>
    /// Changes to talking state.
    /// </summary>
    public void StartTalking()
    {
        _stateMachine?.ChangeState<NPCTalkingState>();
    }

    /// <summary>
    /// Stops talking and returns to previous state.
    /// </summary>
    public void StopTalking()
    {
        _stateMachine?.ReturnToPreviousState();
    }
}

/// <summary>
/// Enum for initial NPC state configuration.
/// </summary>
public enum NPCInitialState
{
    Idle,
    Patrol,
    Follow,
    Talking
}
