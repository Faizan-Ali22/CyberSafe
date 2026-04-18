using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class PauseButtonSafeBindings : MonoBehaviour
{
    [SerializeField] public Button pauseButton;
    [SerializeField] public Button resumeButton;
    [SerializeField] public Button mainMenuButton;

    [SerializeField] private float bindRetrySeconds = 2f;
    [SerializeField] private float retryIntervalSeconds = 0.05f;

    private bool isBound;
    private Coroutine bindRoutine;

    private void Awake()
    {
        // Clear stale callbacks immediately so wrong persistent listeners cannot fire.
        ClearButtonEvents(pauseButton);
        ClearButtonEvents(resumeButton);
        ClearButtonEvents(mainMenuButton);
        SetButtonsInteractable(false);
    }

    private void OnEnable()
    {
        isBound = false;

        if (bindRoutine != null)
            StopCoroutine(bindRoutine);

        bindRoutine = StartCoroutine(BindWhenManagerReady());
    }

    private void OnDisable()
    {
        if (bindRoutine != null)
        {
            StopCoroutine(bindRoutine);
            bindRoutine = null;
        }
    }

    private IEnumerator BindWhenManagerReady()
    {
        float deadline = Time.unscaledTime + Mathf.Max(0.1f, bindRetrySeconds);
        GlobalPauseManager mgr = GlobalPauseManager.Instance;

        while (mgr == null && Time.unscaledTime < deadline)
        {
            mgr = FindObjectOfType<GlobalPauseManager>();
            if (mgr != null) break;
            yield return new WaitForSecondsRealtime(Mathf.Max(0.01f, retryIntervalSeconds));
        }

        if (mgr == null)
        {
            Debug.LogWarning("[PauseButtonSafeBindings] GlobalPauseManager not found. Buttons remain disabled to prevent wrong actions.", this);
            bindRoutine = null;
            yield break;
        }

        ApplyBindings(mgr);
        isBound = true;
        SetButtonsInteractable(true);
        bindRoutine = null;
    }

    private void ApplyBindings(GlobalPauseManager mgr)
    {
        if (mgr == null) return;

        if (HasDuplicateAssignments())
        {
            Debug.LogWarning("[PauseButtonSafeBindings] Duplicate button assignments detected. Fix references in Inspector.", this);
        }

        BindExclusive(pauseButton, mgr.OnPauseButtonPressed);
        BindExclusive(resumeButton, mgr.OnResumeButtonPressed);

        // Safety: if main menu is accidentally assigned to the same object as resume,
        // keep resume behavior and skip menu binding to prevent losing progress.
        if (mainMenuButton == resumeButton && mainMenuButton != null)
        {
            Debug.LogWarning("[PauseButtonSafeBindings] mainMenuButton matches resumeButton. Skipping main menu binding for safety.", this);
            return;
        }

        BindExclusive(mainMenuButton, mgr.OnMainMenuButtonPressed);
    }

    private void BindExclusive(Button button, UnityAction callback)
    {
        if (button == null || callback == null) return;

        // Replace the whole event to clear both runtime and persistent Inspector listeners.
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(callback);
    }

    private void ClearButtonEvents(Button button)
    {
        if (button == null) return;
        button.onClick = new Button.ButtonClickedEvent();
    }

    private void SetButtonsInteractable(bool value)
    {
        if (pauseButton != null) pauseButton.interactable = value;
        if (resumeButton != null) resumeButton.interactable = value;
        if (mainMenuButton != null) mainMenuButton.interactable = value;
    }

    private bool HasDuplicateAssignments()
    {
        if (pauseButton == null || resumeButton == null || mainMenuButton == null)
            return false;

        return pauseButton == resumeButton ||
               pauseButton == mainMenuButton ||
               resumeButton == mainMenuButton;
    }
}