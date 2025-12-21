using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Health View - UI layer of the MVC Health system.
/// Responsible only for displaying health data.
/// Does not contain any game logic or data manipulation.
/// </summary>
public class HealthView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private Image _fillImage;

    [Header("Color Settings")]
    [SerializeField] private bool _useColorGradient = true;
    [SerializeField] private Color _fullHealthColor = Color.green;
    [SerializeField] private Color _lowHealthColor = Color.red;
    [SerializeField] private float _lowHealthThreshold = 0.3f;

    [Header("Animation Settings")]
    [SerializeField] private bool _animateChanges = true;
    [SerializeField] private float _animationSpeed = 5f;

    private float _targetValue;
    private float _displayValue;

    /// <summary>
    /// Updates the health display with the specified values.
    /// </summary>
    /// <param name="currentHealth">Current health value.</param>
    /// <param name="maxHealth">Maximum health value.</param>
    /// <param name="normalizedHealth">Normalized health (0-1).</param>
    public void UpdateDisplay(float currentHealth, float maxHealth, float normalizedHealth)
    {
        _targetValue = normalizedHealth;

        if (!_animateChanges)
        {
            _displayValue = _targetValue;
            ApplyDisplayValue(currentHealth);
        }
    }

    /// <summary>
    /// Updates the health text format.
    /// </summary>
    /// <param name="format">Format string (e.g., "Health: {0}/{1}").</param>
    /// <param name="currentHealth">Current health value.</param>
    /// <param name="maxHealth">Maximum health value.</param>
    public void SetHealthText(string format, float currentHealth, float maxHealth)
    {
        if (_healthText != null)
        {
            _healthText.text = string.Format(format, Mathf.CeilToInt(currentHealth), Mathf.CeilToInt(maxHealth));
        }
    }

    /// <summary>
    /// Sets a simple label for the health display.
    /// </summary>
    /// <param name="label">Label text.</param>
    public void SetLabel(string label)
    {
        if (_healthText != null)
        {
            _healthText.text = label;
        }
    }

    /// <summary>
    /// Sets the health display immediately without animation.
    /// </summary>
    /// <param name="normalizedHealth">Normalized health (0-1).</param>
    /// <param name="label">Optional label text.</param>
    public void SetValueImmediate(float normalizedHealth, string label = null)
    {
        _targetValue = normalizedHealth;
        _displayValue = normalizedHealth;

        if (_healthSlider != null)
        {
            _healthSlider.value = normalizedHealth;
        }

        UpdateFillColor(normalizedHealth);

        if (!string.IsNullOrEmpty(label) && _healthText != null)
        {
            _healthText.text = label;
        }
    }

    private void Update()
    {
        if (!_animateChanges) return;

        if (!Mathf.Approximately(_displayValue, _targetValue))
        {
            _displayValue = Mathf.MoveTowards(_displayValue, _targetValue, _animationSpeed * Time.deltaTime);
            ApplyDisplayValue(_displayValue * 100f);
        }
    }

    private void ApplyDisplayValue(float currentHealth)
    {
        if (_healthSlider != null)
        {
            _healthSlider.value = _displayValue;
        }

        UpdateFillColor(_displayValue);
    }

    private void UpdateFillColor(float normalizedHealth)
    {
        if (!_useColorGradient || _fillImage == null) return;

        if (normalizedHealth <= _lowHealthThreshold)
        {
            _fillImage.color = _lowHealthColor;
        }
        else
        {
            _fillImage.color = Color.Lerp(_lowHealthColor, _fullHealthColor, 
                (normalizedHealth - _lowHealthThreshold) / (1f - _lowHealthThreshold));
        }
    }

    /// <summary>
    /// Plays a damage flash effect.
    /// </summary>
    public void PlayDamageEffect()
    {
        // Can be extended to add visual damage feedback
    }

    /// <summary>
    /// Plays a heal effect.
    /// </summary>
    public void PlayHealEffect()
    {
        // Can be extended to add visual heal feedback
    }
}
