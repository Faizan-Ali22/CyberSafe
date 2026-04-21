using UnityEngine;
using System.Collections;

public class TeacherInteraction : MonoBehaviour
{
    [Header("Player Scripts to Disable")]
    public MonoBehaviour[] playerScriptsToDisable;

    [Header("UI to Disable During Interaction")]
    public GameObject[] uiElementsToDisable;

    [Header("UI to Enable After Interaction")]
    public GameObject[] uiElementsToEnableAfter;

    [Header("UI to Disable After Interaction")]
    public GameObject[] uiElementsToDisableAfter;

    [Header("Dialogue")]
    public DialogueLine[] dialogueLines;

    [Header("Characters")]
    public Animator teacherAnimator;
    public Transform teacherTransform;
    public Transform playerTransform;

    [Header("Camera Targets - Teacher Only")]
    public Transform teacherCloseUpTarget;
    public Transform teacherMediumShotTarget;

    [Header("Tap to Interact")]
    public TapToInteract tapToInteract;

    [Header("Cinematic Settings")]
    public float initialCameraTransitionTime = 1.5f;

    [Header("Animation Settings (Trigger Talk)")]
    public string talkTriggerName = "Talk";

    // Kept for compatibility, but not required in the optimized Animator setup
    public string idleTriggerName = "Idle";

    [Header("Idle State Settings (Hard Fix)")]
    [Tooltip("Must match the EXACT Animator state name (in your screenshots it's 'idle' lowercase).")]
    public string idleStateName = "idle";

    [Tooltip("Animator layer index (usually 0).")]
    public int animatorLayer = 0;

    [Tooltip("Small crossfade to snap out of last talk frame smoothly.")]
    public float idleCrossfadeTime = 0.1f;

    private bool hasInteracted = false;

    private void Start()
    {
        // Don't modify the initial UI layout if the player already finished this conversation
        if (LabReturnState.IsTeacherDone()) return; 

        SetupDialogueLines();

        // Ensure post-interaction UI to enable is disabled at start
        foreach (var ui in uiElementsToEnableAfter)
        {
            if (ui != null)
                ui.SetActive(false);
        }

        if (tapToInteract != null)
        {
            tapToInteract.OnInteract += OnPlayerTapped;
        }
    }

    private void OnDestroy()
    {
        if (tapToInteract != null)
        {
            tapToInteract.OnInteract -= OnPlayerTapped;
        }
    }

    private void SetupDialogueLines()
    {
        dialogueLines = new DialogueLine[]
        {
            new DialogueLine
            {
            speakerName = "Ayan",
            dialogue = "Sir! What just happened? Every screen just… glitched.",
            speakerAnimator = null,
            animationTrigger = "",
            cameraTarget = teacherMediumShotTarget,
            cameraTransitionTime = 0.5f
            },
            new DialogueLine
            {
            speakerName = "Teacher",
            dialogue = "We’ve been hit by a CyberAttack. From what I’m seeing… it looks like Malware.",
            speakerAnimator = teacherAnimator,
            animationTrigger = talkTriggerName,
            cameraTarget = teacherCloseUpTarget,
            cameraTransitionTime = 0.8f
            },
            new DialogueLine
            {
            speakerName = "Teacher",
            dialogue = "Malware is “Malicious Software” A program that sneaks into a device to spy, steal data, or damage files.",
            speakerAnimator = teacherAnimator,
            animationTrigger = talkTriggerName,
            cameraTarget = null,
            cameraTransitionTime = 0f
            },
            new DialogueLine
            {
            speakerName = "Ayan",
            dialogue = "So it’s not just a glitch… it’s something inside the PCs?",
            speakerAnimator = null,
            animationTrigger = "",
            cameraTarget = teacherMediumShotTarget,
            cameraTransitionTime = 0.5f
            },
            new DialogueLine
            {
            speakerName = "Teacher",
            dialogue = "Exactly. And the first rule is speed:\n the longer it runs, the more it can spread and the more data we can lose.",
            speakerAnimator = teacherAnimator,
            animationTrigger = talkTriggerName,
            cameraTarget = teacherCloseUpTarget,
            cameraTransitionTime = 0.8f
            },
            new DialogueLine
            {
            speakerName = "Teacher",
            dialogue = "The other students fixed their Pc's.\n Ayan, I need you to check your and your friends PC before this gets worse.",
            speakerAnimator = teacherAnimator,
            animationTrigger = talkTriggerName,
            cameraTarget = teacherMediumShotTarget,
            cameraTransitionTime = 1f
            },
            new DialogueLine
            {
            speakerName = "Teacher",
            dialogue = "Start with the basics:\nmake sure the firewall is ON, update the antivirus, and run a full scan.If a PC looks infected, isolate it immediately.",
            speakerAnimator = teacherAnimator,
            animationTrigger = talkTriggerName,
            cameraTarget = teacherCloseUpTarget,
            cameraTransitionTime = 0.8f
            },
            new DialogueLine
            {
            speakerName = "Ayan",
            dialogue = "Firewall on… update… scan… isolate if it’s infected. Got it. \nWhat am I looking for?",
            speakerAnimator = null,
            animationTrigger = "",
            cameraTarget = teacherMediumShotTarget,
            cameraTransitionTime = 0.5f
            },
            new DialogueLine
            {
            speakerName = "Teacher",
            dialogue = "Anything unusual unknown programs, viruses, disabled security, or that same NULL screen. Don’t panic,follow steps.",
            speakerAnimator = teacherAnimator,
            animationTrigger = talkTriggerName,
            cameraTarget = teacherMediumShotTarget,
            cameraTransitionTime = 0.8f
            },
            new DialogueLine
            {
            speakerName = "Teacher",
            dialogue = "We’ll run this like a drill. You’re about to do a quick Minigame learn the response steps, then use them to protect your PCs.",
            speakerAnimator = teacherAnimator,
            animationTrigger = talkTriggerName,
            cameraTarget = teacherCloseUpTarget,
            cameraTransitionTime = 0.8f
            }
        };
    }

    private void OnPlayerTapped()
    {
        if (hasInteracted) return;
        StartCoroutine(StartCinematicInteraction());
    }

    private IEnumerator StartCinematicInteraction()
    {
        hasInteracted = true;

        if (tapToInteract != null)
            tapToInteract.DisableInteraction();

        foreach (var script in playerScriptsToDisable)
        {
            if (script != null)
                script.enabled = false;
        }

        foreach (var ui in uiElementsToDisable)
        {
            if (ui != null)
                ui.SetActive(false);
        }

        if (playerTransform != null && teacherTransform != null)
        {
            StartCoroutine(SmoothLookAt(playerTransform, teacherTransform.position, 0.5f));
        }

        if (CinematicCameraController.Instance != null)
        {
            CinematicCameraController.Instance.SwitchToCinematicCamera(
                teacherMediumShotTarget,
                initialCameraTransitionTime
            );
        }

        yield return new WaitForSeconds(initialCameraTransitionTime);

        // Ensure we start dialogue from a clean idle pose
        ForceTeacherIdleHard();

        DialogueManager.Instance.onDialogueEnd.AddListener(EndInteraction);
        DialogueManager.Instance.StartDialogue(dialogueLines);
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

    private void EndInteraction()
    {
        StartCoroutine(EndInteractionRoutine());
    }

    private IEnumerator EndInteractionRoutine()
    {
        DialogueManager.Instance.onDialogueEnd.RemoveListener(EndInteraction);

        // Force teacher back to idle when dialogue ends
        ForceTeacherIdleHard();

        yield return null;

        if (CinematicCameraController.Instance != null)
        {
            CinematicCameraController.Instance.SwitchToMainCamera(1.5f);
        }

        yield return new WaitForSeconds(1.5f);

        // Re-enable player scripts
        foreach (var script in playerScriptsToDisable)
        {
            if (script != null)
                script.enabled = true;
        }

        // Re-enable original UI elements
        foreach (var ui in uiElementsToDisable)
        {
            if (ui != null)
                ui.SetActive(true);
        }

        // Disable UI after interaction (Task1)
        foreach (var ui in uiElementsToDisableAfter)
        {
            if (ui != null)
                ui.SetActive(false);
        }

        // Enable UI after interaction (Task2)
        foreach (var ui in uiElementsToEnableAfter)
        {
            if (ui != null)
                ui.SetActive(true);
        }

        // Safety: one more frame later to beat any animator transition timing
        yield return null;
        ForceTeacherIdleHard();
    }

    /// <summary>
    /// Hard force idle pose. This fixes "stuck on last talk frame" even if Animator transitions are imperfect.
    /// In the optimized Animator setup, the teacher returns to idle automatically via Exit Time,
    /// but this guarantees it at key moments (start/end).
    /// </summary>
    private void ForceTeacherIdleHard()
    {
        if (teacherAnimator == null) return;

        // Clear triggers that might keep or re-enter talk
        teacherAnimator.ResetTrigger(talkTriggerName);
        teacherAnimator.ResetTrigger(idleTriggerName);

        // Hard return to Idle state
        if (!string.IsNullOrWhiteSpace(idleStateName))
        {
            teacherAnimator.CrossFadeInFixedTime(idleStateName, idleCrossfadeTime, animatorLayer);
        }

        Debug.Log("Teacher forced to Idle state (hard)");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (teacherCloseUpTarget != null)
        {
            Gizmos.DrawWireSphere(teacherCloseUpTarget.position, 0.3f);
            Gizmos.DrawLine(teacherCloseUpTarget.position, teacherCloseUpTarget.position + teacherCloseUpTarget.forward);
        }

        Gizmos.color = Color.yellow;
        if (teacherMediumShotTarget != null)
        {
            Gizmos.DrawWireSphere(teacherMediumShotTarget.position, 0.3f);
            Gizmos.DrawLine(teacherMediumShotTarget.position, teacherMediumShotTarget.position + teacherMediumShotTarget.forward);
        }
    }

    /// <summary>
    /// Called by LabPlayerRestorer to instantly fast-forward the UI 
    /// to the Post-Teacher Conversation state (Task 1 OFF, Task 2 ON).
    /// </summary>
    public void ApplyPostInteractionState()
    {
        foreach (var ui in uiElementsToDisableAfter)
        {
            if (ui != null) ui.SetActive(false);
        }

        foreach (var ui in uiElementsToEnableAfter)
        {
            if (ui != null) ui.SetActive(true);
        }

        if (tapToInteract != null)
        {
            // Remove the floating interact button over the teacher completely
            tapToInteract.gameObject.SetActive(false);
        }
    }
}