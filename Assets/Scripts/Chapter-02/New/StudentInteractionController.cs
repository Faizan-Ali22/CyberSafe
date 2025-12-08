using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class StudentInteractionController : MonoBehaviour
{
   [Header("Player Scripts to Disable")]
    public MonoBehaviour[] scriptsToDisable; // Movement, Camera control, etc.

    [Header("UI Control")]
    public GameObject[] uiToHideDuring;      // HUD, Joystick, etc.
    public GameObject[] uiToShowAfter;       // "Take Phone" button (Task 2)
    public GameObject[] uiToHideAfter;       // Any prompt that should vanish (Task 1)

    [Header("Conversation Data")]
    // We use the NEW class name here so it shows up correctly in the Inspector
    public NpcDialogueEntry[] conversationLines;

    [Header("Character References")]
    public Animator studentAnimator;
    public string sitIdleTrigger = "IdleSit";
    public string talkTrigger = "Talk";

    [Header("Camera Setup")]
    public Transform camCloseUp;
    public Transform camMediumShot;
    public float startTransitionTime = 1.2f;
    public float endTransitionTime = 1.0f;

    [Header("Dependencies")]
    public StudentNotifier notifier;         // The script handling the "Ting" sound
    public TapToInteract tapInput;           // The script handling the input tap

    private bool _hasInteracted = false;

    private void Start()
    {
        // 1. Ensure "After" UI is hidden at start
        foreach (var ui in uiToShowAfter)
            if (ui != null) ui.SetActive(false);

        // 2. Listen for the tap event
        if (tapInput != null)
            tapInput.OnInteract += HandleTap;

        // 3. Ensure the prompt is hidden initially
        if (notifier != null && notifier.interactCanvas != null)
            notifier.interactCanvas.SetActive(false);
    }

    private void OnDestroy()
    {
        if (tapInput != null)
            tapInput.OnInteract -= HandleTap;
    }

    // --- TRIGGER ---
    private void HandleTap()
    {
        if (_hasInteracted) return; // Prevent double triggering
        StartCoroutine(StartCinematicRoutine());
    }

    // --- MAIN LOGIC ---
    private IEnumerator StartCinematicRoutine()
    {
        _hasInteracted = true;

        // STEP 1: LOCK EVERYTHING
        if (tapInput != null) tapInput.DisableInteraction(); // Stop further taps
        if (notifier != null) notifier.StopNotifications(); // Stop the "Ting" sound

        foreach (var script in scriptsToDisable)
            if (script != null) script.enabled = false;

        foreach (var ui in uiToHideDuring)
            if (ui != null) ui.SetActive(false);

        // STEP 2: CAMERA SWITCH
        // (Assuming you have a CinematicCameraController singleton)
        if (CinematicCameraController.Instance != null)
        {
            // Prefer medium shot, fallback to close up
            Transform target = camMediumShot != null ? camMediumShot : camCloseUp;
            CinematicCameraController.Instance.SwitchToCinematicCamera(target, startTransitionTime);
        }

        // STEP 3: WAIT FOR CAMERA
        yield return new WaitForSeconds(startTransitionTime);

        // STEP 4: PREPARE ANIMATOR
        ForceIdle();

        // STEP 5: CONVERT DATA & START DIALOGUE
        // We convert our NpcDialogueEntry[] to the DialogueLine[] your Manager expects.
        DialogueLine[] convertedLines = new DialogueLine[conversationLines.Length];
        
        for (int i = 0; i < conversationLines.Length; i++)
        {
            convertedLines[i] = new DialogueLine
            {
                speakerName = conversationLines[i].speakerName,
                dialogue = conversationLines[i].dialogueText,
                speakerAnimator = conversationLines[i].speakerAnimator,
                animationTrigger = conversationLines[i].animationTrigger,
                cameraTarget = conversationLines[i].cameraTarget,
                cameraTransitionTime = conversationLines[i].cameraTransitionTime
            };
        }

        DialogueManager.Instance.onDialogueEnd.AddListener(OnDialogueFinished);
        DialogueManager.Instance.StartDialogue(convertedLines);
    }

    // --- CLEANUP ---
    private void OnDialogueFinished()
    {
        DialogueManager.Instance.onDialogueEnd.RemoveListener(OnDialogueFinished);
        StartCoroutine(EndCinematicRoutine());
    }

    private IEnumerator EndCinematicRoutine()
    {
        // 1. Reset NPC
        ForceIdle();

        // 2. Reset Camera
        if (CinematicCameraController.Instance != null)
        {
            CinematicCameraController.Instance.SwitchToMainCamera(endTransitionTime);
        }

        yield return new WaitForSeconds(endTransitionTime);

        // 3. Re-enable Player
        foreach (var script in scriptsToDisable)
            if (script != null) script.enabled = true;

        // 4. Restore HUD
        foreach (var ui in uiToHideDuring)
            if (ui != null) ui.SetActive(true);

        // 5. Hide "Task 1" UI
        foreach (var ui in uiToHideAfter)
            if (ui != null) ui.SetActive(false);

        // 6. Show "Task 2" UI (Take Phone Button)
        foreach (var ui in uiToShowAfter)
            if (ui != null) ui.SetActive(true);
    }

    private void ForceIdle()
    {
        if (studentAnimator == null) return;
        studentAnimator.ResetTrigger(talkTrigger);
        studentAnimator.SetTrigger(sitIdleTrigger);
    }
}
