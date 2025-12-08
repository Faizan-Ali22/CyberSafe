using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TapToInteract : MonoBehaviour
{
     [Header("UI Settings")]
    public GameObject interactCanvas; // World Space Canvas
    public TextMeshProUGUI interactText;
    public Button interactButton;
    
    [Header("Positioning")]
    public Transform targetTransform; // Teacher's transform
    public Vector3 offset = new Vector3(0, 2.2f, 0); // Above head
    
    [Header("Animation")]
    public float bobSpeed = 2f;
    public float bobAmount = 0.1f;
    public float fadeDistance = 8f;
    public float interactionDistance = 3f;
    
    [Header("Look At Camera")]
    public bool alwaysFaceCamera = true;
    
    private Transform player;
    private CanvasGroup canvasGroup;
    private Vector3 initialOffset;
    private bool isInteractable = false;
    
    public System.Action OnInteract; // Event for interaction
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        if (targetTransform == null)
            targetTransform = transform;
            
        initialOffset = offset;
        
        // Setup canvas group for fading
        canvasGroup = interactCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = interactCanvas.AddComponent<CanvasGroup>();
        
        // Setup button
        if (interactButton != null)
            interactButton.onClick.AddListener(HandleInteraction);
            
        interactCanvas.SetActive(false);
    }
    
    private void Update()
    {
        if (player == null) return;
        
        float distance = Vector3. Distance(player.position, targetTransform. position);
        
        // Show/hide based on distance
        if (distance <= fadeDistance && !DialogueManager.Instance. IsDialogueActive())
        {
            if (! interactCanvas.activeSelf)
                interactCanvas.SetActive(true);
            
            // Position above target's head with bobbing
            float bob = Mathf. Sin(Time.time * bobSpeed) * bobAmount;
            Vector3 newOffset = initialOffset + new Vector3(0, bob, 0);
            interactCanvas.transform. position = targetTransform.position + newOffset;
            
            // Face camera
            if (alwaysFaceCamera && Camera.main != null)
            {
                interactCanvas.transform. LookAt(
                    interactCanvas.transform. position + Camera.main.transform. forward
                );
            }
            
            // Fade based on distance
            float alpha = 1f - Mathf.Clamp01((distance - interactionDistance) / (fadeDistance - interactionDistance));
            canvasGroup.alpha = alpha;
            
            // Enable interaction only when close enough
            isInteractable = distance <= interactionDistance;
            canvasGroup.interactable = isInteractable;
            canvasGroup.blocksRaycasts = isInteractable;
            
            // Change text color based on interactability
            if (interactText != null)
            {
                interactText.color = isInteractable ? Color.white : new Color(1, 1, 1, 0.5f);
            }
        }
        else
        {
            if (interactCanvas.activeSelf)
                interactCanvas.SetActive(false);
        }
    }
    
    private void HandleInteraction()
    {
        if (! isInteractable) return;
        
        OnInteract?.Invoke();
        interactCanvas.SetActive(false);
    }
    
    public void DisableInteraction()
    {
        interactCanvas.SetActive(false);
        enabled = false;
    }
    
    public void EnableInteraction()
    {
        enabled = true;
    }
}
