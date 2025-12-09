using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))] // For the sound trigger area
[RequireComponent(typeof(AudioSource))] // For the "Ting" sound
public class StudentInteraction : MonoBehaviour
{
   [Header("Minigame Configuration")]
    public GameObject minigameCanvas; // Drag the Canvas containing "Bilal.cs" here
    
    [Header("Sound Trigger Settings")]
    public AudioClip notificationSound;
    public float soundInterval = 2.0f;
    
    [Header("Player Scripts to Disable")]
    public MonoBehaviour[] playerScriptsToDisable;
    public Rigidbody playerRigidbody; // Drag Player Rigidbody here for extra safety

    [Header("UI to Disable During Interaction")]
    public GameObject[] uiElementsToDisable;

    [Header("UI to Enable After Interaction")]
    public GameObject[] uiElementsToEnableAfter; 

    [Header("UI to Disable After Interaction")]
    public GameObject[] uiElementsToDisableAfter; 

    [Header("Dialogue Configuration")]
    public StudentDialogueLine[] dialogueLines;

    [Header("Characters")]
    public Animator studentAnimator;
    public Transform studentTransform;
    public Transform playerTransform;

    [Header("Camera Targets")]
    public Transform camCloseUp;
    public Transform camMediumShot;

    [Header("Tap to Interact")]
    public TapToInteract tapToInteract;

    [Header("Cinematic Settings")]
    public float initialCameraTransitionTime = 1.5f;

    [Header("Animation Settings")]
    public string talkTriggerName = "Talk";
    public string idleTriggerName = "IdleSit";

    [Header("Scene Transition")]
    [SerializeField] public GameObject MiniMap;
    [SerializeField] public string nextSceneName = "";

    private bool hasInteracted = false;
    private bool isPlayerInZone = false;
    private float soundTimer = 0f;
    private AudioSource audioSource;
    private RigidbodyConstraints previousConstraints;

    private void Start()
    {
        
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = notificationSound;
        audioSource.playOnAwake = false;
        GetComponent<BoxCollider>().isTrigger = true;

        foreach (var ui in uiElementsToEnableAfter) if (ui != null) ui.SetActive(false);
        if (tapToInteract != null) tapToInteract.gameObject.SetActive(false);
        if (tapToInteract != null) tapToInteract.OnInteract += OnPlayerTapped;
        
        // Ensure Minigame is hidden at start
        if (minigameCanvas != null) minigameCanvas.SetActive(false);
    }

    private void OnDestroy() { if (tapToInteract != null) tapToInteract.OnInteract -= OnPlayerTapped; }

    private void Update()
    {
        if (isPlayerInZone && !hasInteracted)
        {
            soundTimer += Time.deltaTime;
            if (soundTimer >= soundInterval)
            {
                soundTimer = 0f;
                if (audioSource != null) audioSource.Play();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            soundTimer = soundInterval; 
            if (tapToInteract != null && !hasInteracted) tapToInteract.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            if (tapToInteract != null) tapToInteract.gameObject.SetActive(false);
        }
    }

    private void OnPlayerTapped()
    {
        if (hasInteracted) return;
        StartCoroutine(StartCinematicInteraction());
    }

    private IEnumerator StartCinematicInteraction()
    {
        hasInteracted = true;
        if (tapToInteract != null) tapToInteract.DisableInteraction();
        if (audioSource != null) audioSource.Stop();

        // Disable Player
        foreach (var script in playerScriptsToDisable) if (script != null) script.enabled = false;
        foreach (var ui in uiElementsToDisable) if (ui != null) ui.SetActive(false);

        // Look At
        if (playerTransform != null && studentTransform != null)
            StartCoroutine(SmoothLookAt(playerTransform, studentTransform.position, 0.5f));

        // Camera
        if (StudentCinematicManager.Instance != null)
            StudentCinematicManager.Instance.SwitchToCinematicCamera(camMediumShot, initialCameraTransitionTime);

        yield return new WaitForSeconds(initialCameraTransitionTime);

        // Dialogue
        ForceStudentIdle();
        StudentDialogueManager.Instance.onDialogueEnd.AddListener(EndInteraction);
        StudentDialogueManager.Instance.StartDialogue(dialogueLines);
    }

    // --- TRANSITION LOGIC ---
    private void EndInteraction()
    {
        StudentDialogueManager.Instance.onDialogueEnd.RemoveListener(EndInteraction);
        ForceStudentIdle();

        // CHECK: If we have a minigame, go to Portrait Mode. If not, finish normally.
        if (minigameCanvas != null)
        {
            StartCoroutine(SwitchToPortraitAndShowMinigame());
        }
        else
        {
            StartCoroutine(FinishInteractionRoutine());
        }
    }

    private IEnumerator SwitchToPortraitAndShowMinigame()
    {
        yield return new WaitForSeconds(0.5f);

        // 1. Rotate Screen
        Screen.orientation = ScreenOrientation.Portrait;

        // 2. Freeze Physics (Optional extra safety)
        if (playerRigidbody != null)
        {
            previousConstraints = playerRigidbody.constraints;
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
            playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        // 3. Show Minigame
        if (minigameCanvas != null)
        {
            minigameCanvas.SetActive(true);
            MiniMap.SetActive(false);
        }
    }

    // Called by Bilal.cs when "Give Phone Back" is pressed
    public void CloseMinigameAndFinish()
    {
        StartCoroutine(FinishInteractionRoutine());
        StartCoroutine(LoadNextSceneAfterDelay(5f));
    }

    private IEnumerator LoadNextSceneAfterDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);

        if (!string.IsNullOrWhiteSpace(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
            yield break;
        }

        // Fallback to next build index if no name is provided
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
    }

    private IEnumerator FinishInteractionRoutine()
    {
        // 1. Hide Minigame & Reset Orientation
        if (minigameCanvas != null) minigameCanvas.SetActive(false);
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        // 2. Unfreeze Physics
        if (playerRigidbody != null)
        {
            playerRigidbody.constraints = previousConstraints;
        }

        // 3. Reset Camera
        if (StudentCinematicManager.Instance != null)
            StudentCinematicManager.Instance.SwitchToMainCamera(1.5f);

        yield return new WaitForSeconds(1.5f);

        // 4. Re-enable Player & UI
        foreach (var script in playerScriptsToDisable) if (script != null) script.enabled = true;
        foreach (var ui in uiElementsToDisable) if (ui != null) ui.SetActive(true);
        foreach (var ui in uiElementsToDisableAfter) if (ui != null) ui.SetActive(false);
        foreach (var ui in uiElementsToEnableAfter) if (ui != null) ui.SetActive(true);
    }

    private IEnumerator SmoothLookAt(Transform obj, Vector3 targetPos, float duration)
    {
        Quaternion startRot = obj.rotation;
        Vector3 direction = (targetPos - obj.position).normalized;
        direction.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(direction);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            obj.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }
        obj.rotation = targetRot;
    }

    private void ForceStudentIdle()
    {
        if (studentAnimator == null) return;
        studentAnimator.ResetTrigger(talkTriggerName);
        studentAnimator.SetTrigger(idleTriggerName);
    }
}
