using UnityEngine;

/// <summary>
/// Player Interaction Component - Detects and interacts with IInteractable objects.
/// Uses the Command pattern for interaction execution.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float _interactionRadius = 2f;
    [SerializeField] private LayerMask _interactableLayer = -1;
    [SerializeField] private Transform _interactionOrigin;

    [Header("UI")]
    [SerializeField] private GameObject _interactPromptUI;
    [SerializeField] private TMPro.TextMeshProUGUI _promptText;

    private IInteractable _currentInteractable;
    private Collider[] _hitColliders = new Collider[10];

    /// <summary>
    /// Gets the current interactable object in range.
    /// </summary>
    public IInteractable CurrentInteractable => _currentInteractable;

    private void Awake()
    {
        if (_interactionOrigin == null)
        {
            _interactionOrigin = transform;
        }
    }

    private void Update()
    {
        DetectInteractables();
        UpdatePromptUI();
    }

    /// <summary>
    /// Attempts to interact with the current interactable.
    /// Called when the player presses the interact button.
    /// </summary>
    public void TryInteract()
    {
        if (_currentInteractable != null && _currentInteractable.CanInteract())
        {
            var command = new InteractCommand(_currentInteractable);
            CommandInvoker.Instance?.ExecuteCommand(command);
        }
    }

    private void DetectInteractables()
    {
        int count = Physics.OverlapSphereNonAlloc(
            _interactionOrigin.position,
            _interactionRadius,
            _hitColliders,
            _interactableLayer
        );

        IInteractable closest = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            var interactable = _hitColliders[i].GetComponent<IInteractable>();
            if (interactable != null && interactable.CanInteract())
            {
                float distance = Vector3.Distance(_interactionOrigin.position, _hitColliders[i].transform.position);
                if (distance < closestDistance)
                {
                    closest = interactable;
                    closestDistance = distance;
                }
            }
        }

        _currentInteractable = closest;
    }

    private void UpdatePromptUI()
    {
        if (_interactPromptUI == null) return;

        if (_currentInteractable != null && _currentInteractable.CanInteract())
        {
            _interactPromptUI.SetActive(true);
            if (_promptText != null)
            {
                _promptText.text = _currentInteractable.GetInteractPrompt();
            }
        }
        else
        {
            _interactPromptUI.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        var origin = _interactionOrigin != null ? _interactionOrigin : transform;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin.position, _interactionRadius);
    }
}
