using System;
using UnityEngine;

/// <summary>
/// Health Model - Data layer of the MVC Health system.
/// Contains only health data and raises events when data changes.
/// Does not contain any display or game logic.
/// </summary>
[Serializable]
public class HealthModel
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth;

    /// <summary>
    /// Event raised when health value changes.
    /// Parameters: (currentHealth, maxHealth, normalizedHealth)
    /// </summary>
    public event Action<float, float, float> OnHealthChanged;

    /// <summary>
    /// Event raised when health reaches zero.
    /// </summary>
    public event Action OnHealthDepleted;

    /// <summary>
    /// Event raised when health reaches max.
    /// </summary>
    public event Action OnHealthFull;

    /// <summary>
    /// Gets the maximum health value.
    /// </summary>
    public float MaxHealth => _maxHealth;

    /// <summary>
    /// Gets the current health value.
    /// </summary>
    public float CurrentHealth => _currentHealth;

    /// <summary>
    /// Gets the normalized health value (0-1).
    /// </summary>
    public float NormalizedHealth => _maxHealth > 0 ? _currentHealth / _maxHealth : 0f;

    /// <summary>
    /// Gets whether health is at zero.
    /// </summary>
    public bool IsDepleted => _currentHealth <= 0f;

    /// <summary>
    /// Gets whether health is at max.
    /// </summary>
    public bool IsFull => _currentHealth >= _maxHealth;

    /// <summary>
    /// Initializes the health model with specified max health.
    /// </summary>
    /// <param name="maxHealth">Maximum health value.</param>
    public void Initialize(float maxHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = maxHealth;
        RaiseHealthChanged();
    }

    /// <summary>
    /// Sets the current health to a specific value.
    /// </summary>
    /// <param name="health">New health value.</param>
    public void SetHealth(float health)
    {
        float previousHealth = _currentHealth;
        _currentHealth = Mathf.Clamp(health, 0f, _maxHealth);

        if (Mathf.Approximately(previousHealth, _currentHealth)) return;

        RaiseHealthChanged();

        if (_currentHealth <= 0f)
        {
            OnHealthDepleted?.Invoke();
        }
        else if (_currentHealth >= _maxHealth)
        {
            OnHealthFull?.Invoke();
        }
    }

    /// <summary>
    /// Reduces health by the specified amount.
    /// </summary>
    /// <param name="damage">Amount to reduce health by.</param>
    public void TakeDamage(float damage)
    {
        if (damage <= 0f) return;
        SetHealth(_currentHealth - damage);
    }

    /// <summary>
    /// Increases health by the specified amount.
    /// </summary>
    /// <param name="amount">Amount to increase health by.</param>
    public void Heal(float amount)
    {
        if (amount <= 0f) return;
        SetHealth(_currentHealth + amount);
    }

    /// <summary>
    /// Restores health to maximum.
    /// </summary>
    public void RestoreToFull()
    {
        SetHealth(_maxHealth);
    }

    /// <summary>
    /// Sets the maximum health value.
    /// </summary>
    /// <param name="maxHealth">New maximum health.</param>
    /// <param name="maintainPercentage">If true, maintains current health percentage.</param>
    public void SetMaxHealth(float maxHealth, bool maintainPercentage = true)
    {
        float percentage = NormalizedHealth;
        _maxHealth = Mathf.Max(0f, maxHealth);

        if (maintainPercentage)
        {
            _currentHealth = _maxHealth * percentage;
        }
        else
        {
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
        }

        RaiseHealthChanged();
    }

    private void RaiseHealthChanged()
    {
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth, NormalizedHealth);
    }
}
