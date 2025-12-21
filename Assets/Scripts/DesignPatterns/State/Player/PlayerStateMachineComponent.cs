using UnityEngine;

/// <summary>
/// Player State Machine Component - Manages player states and behavior.
/// Add this component to the player to enable state-based behavior.
/// </summary>
public class PlayerStateMachineComponent : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private InputManager _inputManager;

    [Header("Initial State")]
    [SerializeField] private PlayerInitialState _initialState = PlayerInitialState.Idle;

    private StateMachine<PlayerStateMachineComponent> _stateMachine;
    private bool _movementEnabled = true;
    private bool _inputEnabled = true;

    /// <summary>
    /// Gets the underlying state machine.
    /// </summary>
    public StateMachine<PlayerStateMachineComponent> StateMachine => _stateMachine;

    /// <summary>
    /// Gets whether the player has movement input.
    /// </summary>
    public bool HasMovementInput
    {
        get
        {
            if (_inputManager == null) return false;
            return Mathf.Abs(_inputManager.horizontalInput) > 0.01f || 
                   Mathf.Abs(_inputManager.verticalInput) > 0.01f;
        }
    }

    /// <summary>
    /// Gets the movement input magnitude.
    /// </summary>
    public float MovementMagnitude
    {
        get
        {
            if (_inputManager == null) return 0f;
            return Mathf.Clamp01(Mathf.Abs(_inputManager.horizontalInput) + Mathf.Abs(_inputManager.verticalInput));
        }
    }

    /// <summary>
    /// Gets whether movement is enabled.
    /// </summary>
    public bool IsMovementEnabled => _movementEnabled;

    /// <summary>
    /// Gets whether input is enabled.
    /// </summary>
    public bool IsInputEnabled => _inputEnabled;

    private void Awake()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }

        if (_inputManager == null)
        {
            _inputManager = GetComponent<InputManager>();
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
        _stateMachine = new StateMachine<PlayerStateMachineComponent>(this);

        // Register all states
        _stateMachine.RegisterState(new PlayerIdleState());
        _stateMachine.RegisterState(new PlayerWalkingState());
        _stateMachine.RegisterState(new PlayerInDialogueState());
        _stateMachine.RegisterState(new PlayerInCutsceneState());

        // Set initial state
        switch (_initialState)
        {
            case PlayerInitialState.Walking:
                _stateMachine.SetInitialState<PlayerWalkingState>();
                break;
            case PlayerInitialState.InDialogue:
                _stateMachine.SetInitialState<PlayerInDialogueState>();
                break;
            case PlayerInitialState.InCutscene:
                _stateMachine.SetInitialState<PlayerInCutsceneState>();
                break;
            default:
                _stateMachine.SetInitialState<PlayerIdleState>();
                break;
        }
    }

    /// <summary>
    /// Sets a float animator value.
    /// </summary>
    public void SetAnimatorValue(string name, float value)
    {
        if (_animator != null)
        {
            _animator.SetFloat(name, value);
        }
    }

    /// <summary>
    /// Sets a trigger on the animator.
    /// </summary>
    public void SetAnimatorTrigger(string trigger)
    {
        if (_animator != null)
        {
            _animator.SetTrigger(trigger);
        }
    }

    /// <summary>
    /// Enables or disables movement.
    /// </summary>
    public void SetMovementEnabled(bool enabled)
    {
        _movementEnabled = enabled;
        if (_inputManager != null)
        {
            _inputManager.acceptInput = enabled;
        }
    }

    /// <summary>
    /// Enables or disables input.
    /// </summary>
    public void SetInputEnabled(bool enabled)
    {
        _inputEnabled = enabled;
        if (_inputManager != null)
        {
            _inputManager.acceptInput = enabled;
        }
    }

    /// <summary>
    /// Enters dialogue state.
    /// </summary>
    public void EnterDialogue()
    {
        _stateMachine?.ChangeState<PlayerInDialogueState>();
    }

    /// <summary>
    /// Exits dialogue state (returns to idle).
    /// </summary>
    public void ExitDialogue()
    {
        _stateMachine?.ChangeState<PlayerIdleState>();
    }

    /// <summary>
    /// Enters cutscene state.
    /// </summary>
    public void EnterCutscene()
    {
        _stateMachine?.ChangeState<PlayerInCutsceneState>();
    }

    /// <summary>
    /// Exits cutscene state (returns to idle).
    /// </summary>
    public void ExitCutscene()
    {
        _stateMachine?.ChangeState<PlayerIdleState>();
    }
}

/// <summary>
/// Enum for initial player state configuration.
/// </summary>
public enum PlayerInitialState
{
    Idle,
    Walking,
    InDialogue,
    InCutscene
}
