using System;
using UnityEngine;

/// <summary>
/// Health Controller - Logic layer of the MVC Health system.
/// Connects the Model and View, handles game logic for health manipulation.
/// </summary>
public class HealthController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _healthDrainPerSecond = 0f;
    [SerializeField] private bool _enablePassiveDrain = false;

    [Header("View Reference")]
    [SerializeField] private HealthView _view;

    [Header("Legacy UI Support")]
    [SerializeField] private PCHealthUI _legacyHealthUI;

    private HealthModel _model;
    private bool _isActive = true;

    /// <summary>
    /// Event raised when health reaches zero.
    /// </summary>
    public event Action OnDeath;

    /// <summary>
    /// Gets the health model for external access.
    /// </summary>
    public HealthModel Model => _model;

    /// <summary>
    /// Gets the current health value.
    /// </summary>
    public float CurrentHealth => _model?.CurrentHealth ?? 0f;

    /// <summary>
    /// Gets the maximum health value.
    /// </summary>
    public float MaxHealth => _model?.MaxHealth ?? _maxHealth;

    /// <summary>
    /// Gets the normalized health (0-1).
    /// </summary>
    public float NormalizedHealth => _model?.NormalizedHealth ?? 0f;

    /// <summary>
    /// Gets whether health is depleted.
    /// </summary>
    public bool IsDepleted => _model?.IsDepleted ?? false;

    /// <summary>
    /// Sets whether the controller is active (processing updates).
    /// </summary>
    public bool IsActive
    {
        get => _isActive;
        set => _isActive = value;
    }

    private void Awake()
    {
        InitializeModel();
    }

    private void Start()
    {
        // Subscribe to model events
        _model.OnHealthChanged += HandleHealthChanged;
        _model.OnHealthDepleted += HandleHealthDepleted;

        // Initial UI update
        UpdateView();
    }

    private void Update()
    {
        if (!_isActive || !_enablePassiveDrain) return;

        if (_healthDrainPerSecond > 0f && !_model.IsDepleted)
        {
            _model.TakeDamage(_healthDrainPerSecond * Time.deltaTime);
        }
    }

    private void OnDestroy()
    {
        if (_model != null)
        {
            _model.OnHealthChanged -= HandleHealthChanged;
            _model.OnHealthDepleted -= HandleHealthDepleted;
        }
    }

    /// <summary>
    /// Initializes the health controller with custom settings.
    /// </summary>
    /// <param name="maxHealth">Maximum health value.</param>
    /// <param name="drainPerSecond">Passive drain per second.</param>
    public void Initialize(float maxHealth, float drainPerSecond = 0f)
    {
        _maxHealth = maxHealth;
        _healthDrainPerSecond = drainPerSecond;
        _enablePassiveDrain = drainPerSecond > 0f;

        InitializeModel();
        UpdateView();
    }

    /// <summary>
    /// Deals damage to the health.
    /// </summary>
    /// <param name="damage">Damage amount.</param>
    public void TakeDamage(float damage)
    {
        if (!_isActive) return;
        _model?.TakeDamage(damage);
        _view?.PlayDamageEffect();
    }

    /// <summary>
    /// Heals the health by the specified amount.
    /// </summary>
    /// <param name="amount">Heal amount.</param>
    public void Heal(float amount)
    {
        if (!_isActive) return;
        _model?.Heal(amount);
        _view?.PlayHealEffect();
    }

    /// <summary>
    /// Sets the health to a specific value.
    /// </summary>
    /// <param name="health">Health value.</param>
    public void SetHealth(float health)
    {
        _model?.SetHealth(health);
    }

    /// <summary>
    /// Restores health to full.
    /// </summary>
    public void RestoreToFull()
    {
        _model?.RestoreToFull();
        _view?.PlayHealEffect();
    }

    /// <summary>
    /// Sets the passive drain rate.
    /// </summary>
    /// <param name="drainPerSecond">Drain rate per second.</param>
    public void SetDrainRate(float drainPerSecond)
    {
        _healthDrainPerSecond = drainPerSecond;
        _enablePassiveDrain = drainPerSecond > 0f;
    }

    /// <summary>
    /// Enables or disables passive health drain.
    /// </summary>
    /// <param name="enabled">Whether drain is enabled.</param>
    public void SetDrainEnabled(bool enabled)
    {
        _enablePassiveDrain = enabled;
    }

    /// <summary>
    /// Calculates heal amount based on remaining shields (for MazeGameManager compatibility).
    /// </summary>
    /// <param name="remainingShieldsIncludingCurrent">Number of remaining shields including current. Must be greater than 0.</param>
    /// <returns>Heal amount, or 0 if no shields remaining.</returns>
    public float CalculateShieldHeal(int remainingShieldsIncludingCurrent)
    {
        // Guard against division by zero - only calculate if we have shields
        if (remainingShieldsIncludingCurrent < 1) return 0f;

        float missing = _model.MaxHealth - _model.CurrentHealth;
        return missing / remainingShieldsIncludingCurrent;
    }

    /// <summary>
    /// Applies shield heal based on remaining shields.
    /// </summary>
    /// <param name="remainingShieldsIncludingCurrent">Number of remaining shields. Must be greater than 0.</param>
    public void ApplyShieldHeal(int remainingShieldsIncludingCurrent)
    {
        if (remainingShieldsIncludingCurrent < 1) return;
        
        float healAmount = CalculateShieldHeal(remainingShieldsIncludingCurrent);
        Heal(healAmount);
    }

    private void InitializeModel()
    {
        _model = new HealthModel();
        _model.Initialize(_maxHealth);
    }

    private void HandleHealthChanged(float current, float max, float normalized)
    {
        UpdateView();
    }

    private void HandleHealthDepleted()
    {
        OnDeath?.Invoke();
    }

    private void UpdateView()
    {
        if (_view != null)
        {
            _view.UpdateDisplay(_model.CurrentHealth, _model.MaxHealth, _model.NormalizedHealth);
            _view.SetHealthText("PC Health {0}", _model.CurrentHealth, _model.MaxHealth);
        }

        // Support for legacy PCHealthUI
        if (_legacyHealthUI != null)
        {
            _legacyHealthUI.SetValue(_model.NormalizedHealth, $"PC Health {Mathf.CeilToInt(_model.CurrentHealth)}");
        }
    }
}
