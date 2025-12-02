using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

[System.Serializable]
public class DialogueLine
{
    public enum Speaker { Player, NPC }
    public Speaker speaker;
    [TextArea] public string text;
    public float duration = 2.0f;
}

public class NPCInteraction : MonoBehaviour
{
    [Header("UI / Audio")]
    public GameObject tapToInteractUI;
    public AudioClip notificationClip;
    public AudioSource audioSource;

    [Header("Cutscene")]
    public Camera cutsceneCamera;
    public Camera mainFollowCamera;
    public Animator npcAnimator;
    public Animator playerAnimator;
    public DialogueLine[] dialogueSequence;

    [Header("Minigame / AI")]
    public GameObject minigameCanvas;  // the portrait-mode mobile UI
    public string aiPatrolTag = "";

    private List<NPCPatrol> pausedPatrols = new List<NPCPatrol>();
    private List<bool> pausedPatrolsPrevState = new List<bool>();
    private RigidbodyConstraints previousPlayerConstraints = RigidbodyConstraints.None;
    private PlayerMovement cachedPlayerMovement;
    private bool minigameActive = false;

    bool playerInRange = false;
    PlayerManager playerManager;
    private bool isInteractionComplete = false;

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (tapToInteractUI != null) tapToInteractUI.SetActive(false);
        if (cutsceneCamera != null) cutsceneCamera.gameObject.SetActive(false);
        if (minigameCanvas != null) minigameCanvas.SetActive(false);

        playerManager = FindFirstObjectByType<PlayerManager>();
        CheckIfAlreadySaved();
    }

    void CheckIfAlreadySaved()
    {
        if (GameProgressManager.Instance != null)
        {
            if (GameProgressManager.Instance.WasNPCSaved(gameObject.name))
            {
                DisableInteraction();
            }
        }
    }

    public void DisableInteraction()
    {
        isInteractionComplete = true;
        if (tapToInteractUI != null) tapToInteractUI.SetActive(false);
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isInteractionComplete)
        {
            playerInRange = true;
            if (tapToInteractUI != null) tapToInteractUI.SetActive(true);
            StartCoroutine(PlayNotificationTwice());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (tapToInteractUI != null) tapToInteractUI.SetActive(false);
        }
    }

    IEnumerator PlayNotificationTwice()
    {
        if (audioSource != null && notificationClip != null)
        {
            audioSource.PlayOneShot(notificationClip);
            yield return new WaitForSeconds(0.25f);
            audioSource.PlayOneShot(notificationClip);
        }
    }

    // Called when tapping “Tap to Interact” UI
    public void OnTapInteract()
    {
        if (!playerInRange) return;
        if (tapToInteractUI != null) tapToInteractUI.SetActive(false);
        StartCoroutine(PlayInteractionCutscene());
    }

    IEnumerator PlayInteractionCutscene()
    {
        //if (playerManager != null) playerManager.isInCutscene = true;

        if (mainFollowCamera != null) mainFollowCamera.gameObject.SetActive(false);
        if (cutsceneCamera != null) cutsceneCamera.gameObject.SetActive(true);

        Rigidbody playerRb = playerManager ? playerManager.GetComponent<Rigidbody>() : null;
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        // Dialogue sequence
        foreach (DialogueLine ln in dialogueSequence)
        {
            Animator activeAnim = (ln.speaker == DialogueLine.Speaker.Player) ? playerAnimator : npcAnimator;
            Animator otherAnim = (ln.speaker == DialogueLine.Speaker.Player) ? npcAnimator : playerAnimator;

            if (activeAnim != null) activeAnim.SetBool("isTalking", true);
            if (otherAnim != null) otherAnim.SetBool("isTalking", false);

            DialogueManager.Instance.ShowSubtitle(ln.text);
            yield return new WaitForSeconds(ln.duration);
            DialogueManager.Instance.HideSubtitle();

            if (activeAnim != null) activeAnim.SetBool("isTalking", false);
        }

        // After dialogue → show phone minigame (portrait)
        if (cutsceneCamera != null) cutsceneCamera.gameObject.SetActive(false);
        StartCoroutine(SwitchToPortraitAndShowMinigame());
    }

    // ------------------------------ PHONE MODE SEQUENCE ------------------------------

    private IEnumerator SwitchToPortraitAndShowMinigame()
    {
        yield return new WaitForSeconds(0.5f);

        // 🔄 Rotate to portrait (feels like player pulls out phone)
        Screen.orientation = ScreenOrientation.Portrait;

        // 🎮 Show minigame
        if (minigameCanvas != null)
        {
            minigameCanvas.SetActive(true);
            minigameActive = true;
        }

        // Freeze player movement
        Rigidbody playerRb = playerManager ? playerManager.GetComponent<Rigidbody>() : null;
        if (playerRb != null)
        {
            previousPlayerConstraints = playerRb.constraints;
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.constraints = RigidbodyConstraints.FreezeAll;
        }

        cachedPlayerMovement = playerManager?.GetComponent<PlayerMovement>();
        if (cachedPlayerMovement != null) cachedPlayerMovement.enabled = false;

        // Pause all NPCs
        //
    }

    // Called from the “Give Phone Back” button on the minigame canvas
    public void OnReturnFromPhone()
    {
        StartCoroutine(ReturnToLandscapeAndResume());
    }

    private IEnumerator ReturnToLandscapeAndResume()
    {
        yield return new WaitForSeconds(0.5f);

        // 🔄 Rotate back to landscape
        Screen.orientation = ScreenOrientation.LandscapeRight;

        // Hide phone canvas
        if (minigameCanvas != null)
            minigameCanvas.SetActive(false);

        // Resume everything
        ResumeFromMinigame();
    }

    // ------------------------------ AI PAUSE/RESUME ------------------------------

    // private void PauseAllAI()
    // {
    //     pausedPatrols.Clear();
    //     pausedPatrolsPrevState.Clear();

    //     var all = string.IsNullOrEmpty(aiPatrolTag)
    //         ? Object.FindObjectsByType<NPCPatrol>(FindObjectsSortMode.None)
    //         : GameObject.FindGameObjectsWithTag(aiPatrolTag).Select(go => go.GetComponent<NPCPatrol>())
    //             .Where(p => p != null)
    //             .ToArray();

    //     foreach (var p in all)
    //     {
    //         pausedPatrols.Add(p);
    //         pausedPatrolsPrevState.Add(p.isMoving);
    //         p.isMoving = false;
    //     }
    // }

    public void ResumeFromMinigame()
    {
        // Restore player physics
        Rigidbody playerRb = playerManager ? playerManager.GetComponent<Rigidbody>() : null;
        if (playerRb != null)
        {
            playerRb.constraints = previousPlayerConstraints;
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        // Enable player movement again
        if (cachedPlayerMovement != null) cachedPlayerMovement.enabled = true;

        // Resume all AI
        // for (int i = 0; i < pausedPatrols.Count; i++)
        // {
        //     if (pausedPatrols[i] != null)
        //         pausedPatrols[i].isMoving = pausedPatrolsPrevState[i];
        // }
        // pausedPatrols.Clear();
        // pausedPatrolsPrevState.Clear();

        // Player regains control
       // if (playerManager != null) playerManager.isInCutscene = false;

        // Switch back to follow camera
        if (mainFollowCamera != null) mainFollowCamera.gameObject.SetActive(true);

        minigameActive = false;
    }

    void OnDestroy()
    {
        if (minigameActive) ResumeFromMinigame();
    }
}
