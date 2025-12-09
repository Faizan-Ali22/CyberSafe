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
    public Transform targetTransform; // Teacher/Student transform
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
        // Safe finding of player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        
        if (targetTransform == null)
            targetTransform = transform;
            
        initialOffset = offset;
        
        // Setup canvas group for fading
        if (interactCanvas != null)
        {
            canvasGroup = interactCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = interactCanvas.AddComponent<CanvasGroup>();
            
            interactCanvas.SetActive(false);
        }
        
        // Setup button
        if (interactButton != null)
            interactButton.onClick.AddListener(HandleInteraction);
    }
    
    private void Update()
    {
        if (player == null || interactCanvas == null) return;
        
        float distance = Vector3.Distance(player.position, targetTransform.position);
        
        // --- THE FIX IS HERE ---
        // We check if the Instance exists BEFORE asking if dialogue is active.
        // This prevents the "NullReferenceException" when there is no Teacher Manager.
        bool isDialogueOpen = false;
        
        if (DialogueManager.Instance != null)
        {
            isDialogueOpen = DialogueManager.Instance.IsDialogueActive();
        }

        // Only show button if close enough AND no dialogue is playing
        if (distance <= fadeDistance && !isDialogueOpen)
        {
            if (!interactCanvas.activeSelf)
                interactCanvas.SetActive(true);
            
            // Position above target's head with bobbing
            float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
            Vector3 newOffset = initialOffset + new Vector3(0, bob, 0);
            interactCanvas.transform.position = targetTransform.position + newOffset;
            
            // Face camera
            if (alwaysFaceCamera)
            {
                Camera cam = Camera.main;
                if (cam != null)
                {
                    interactCanvas.transform.LookAt(
                        interactCanvas.transform.position + cam.transform.forward
                    );
                }
            }
            
            // Fade based on distance
            float alpha = 1f - Mathf.Clamp01((distance - interactionDistance) / (fadeDistance - interactionDistance));
            if (canvasGroup != null) canvasGroup.alpha = alpha;
            
            // Enable interaction only when close enough
            isInteractable = distance <= interactionDistance;
            
            if (canvasGroup != null)
            {
                canvasGroup.interactable = isInteractable;
                canvasGroup.blocksRaycasts = isInteractable;
            }
            
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
        if (!isInteractable) return;
        
        OnInteract?.Invoke();
        if (interactCanvas != null) interactCanvas.SetActive(false);
    }
    
    public void DisableInteraction()
    {
        if (interactCanvas != null) interactCanvas.SetActive(false);
        enabled = false;
    }
    
    public void EnableInteraction()
    {
        enabled = true;
    }
}
