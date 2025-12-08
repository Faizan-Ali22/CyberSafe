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
    public GameObject[] uiElementsToDisableAfter; // NEW: For Task1

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

    [Header("Animation Settings")]
    public string talkTriggerName = "Talk";
    public string idleTriggerName = "Idle";

    private bool hasInteracted = false;

    private void Start()
    {
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
                dialogue = "Sir, what happened?",
                speakerAnimator = null,
                animationTrigger = "",
                cameraTarget = teacherMediumShotTarget,
                cameraTransitionTime = 0.5f
            },
            new DialogueLine
            {
                speakerName = "Teacher",
                dialogue = "Looks like we have been hit by a Cyber Attack.",
                speakerAnimator = teacherAnimator,
                animationTrigger = talkTriggerName,
                cameraTarget = teacherCloseUpTarget,
                cameraTransitionTime = 0.8f
            },
            new DialogueLine
            {
                speakerName = "Teacher",
                dialogue = "I have advised the students to leave the lab while we fix this mess.",
                speakerAnimator = teacherAnimator,
                animationTrigger = talkTriggerName,
                cameraTarget = null,
                cameraTransitionTime = 0f
            },
            new DialogueLine
            {
                speakerName = "Teacher",
                dialogue = "Ayan, I want you to check each and every PC and see what's the matter.",
                speakerAnimator = teacherAnimator,
                animationTrigger = talkTriggerName,
                cameraTarget = teacherMediumShotTarget,
                cameraTransitionTime = 1f
            },
            new DialogueLine
            {
                speakerName = "Teacher",
                dialogue = "If there is some kind of Malware, fix it.",
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
            tapToInteract. DisableInteraction();

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
            StartCoroutine(SmoothLookAt(playerTransform, teacherTransform. position, 0.5f));
        }

        if (CinematicCameraController.Instance != null)
        {
            CinematicCameraController. Instance.SwitchToCinematicCamera(
                teacherMediumShotTarget, 
                initialCameraTransitionTime
            );
        }

        yield return new WaitForSeconds(initialCameraTransitionTime);

        ForceTeacherIdle();

        DialogueManager.Instance. onDialogueEnd. AddListener(EndInteraction);
        DialogueManager.Instance. StartDialogue(dialogueLines);
    }

    private IEnumerator SmoothLookAt(Transform obj, Vector3 targetPos, float duration)
    {
        Quaternion startRot = obj.rotation;
        Vector3 direction = (targetPos - obj.position).normalized;
        direction. y = 0;
        Quaternion targetRot = Quaternion. LookRotation(direction);

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
        DialogueManager.Instance. onDialogueEnd.RemoveListener(EndInteraction);

        // Force teacher back to idle
        ForceTeacherIdle();

        yield return null;

        if (CinematicCameraController.Instance != null)
        {
            CinematicCameraController.Instance. SwitchToMainCamera(1.5f);
        }

        yield return new WaitForSeconds(1.5f);

        // Re-enable player scripts
        foreach (var script in playerScriptsToDisable)
        {
            if (script != null)
                script.enabled = true;
        }

        // Re-enable original UI elements (that were disabled during interaction)
        foreach (var ui in uiElementsToDisable)
        {
            if (ui != null)
                ui.SetActive(true);
        }

        // DISABLE these UI elements after interaction (Task1)
        foreach (var ui in uiElementsToDisableAfter)
        {
            if (ui != null)
                ui.SetActive(false);
        }

        // ENABLE these UI elements after interaction (Task2)
        foreach (var ui in uiElementsToEnableAfter)
        {
            if (ui != null)
                ui.SetActive(true);
        }
    }

    private void ForceTeacherIdle()
    {
        if (teacherAnimator == null) return;

        teacherAnimator.ResetTrigger(talkTriggerName);
        teacherAnimator. SetTrigger(idleTriggerName);
        
        // Uncomment if triggers don't work:
        // teacherAnimator.Play("Idle", 0, 0f);

        Debug.Log("Teacher forced to Idle state");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (teacherCloseUpTarget != null)
        {
            Gizmos.DrawWireSphere(teacherCloseUpTarget.position, 0.3f);
            Gizmos. DrawLine(teacherCloseUpTarget.position, teacherCloseUpTarget.position + teacherCloseUpTarget.forward);
        }
        
        Gizmos.color = Color.yellow;
        if (teacherMediumShotTarget != null)
        {
            Gizmos. DrawWireSphere(teacherMediumShotTarget. position, 0.3f);
            Gizmos. DrawLine(teacherMediumShotTarget.position, teacherMediumShotTarget.position + teacherMediumShotTarget.forward);
        }
    }
}