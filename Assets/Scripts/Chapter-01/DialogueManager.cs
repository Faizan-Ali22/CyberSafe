using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// Manages dialogue display and progression in the game.
/// Uses Singleton pattern for scene-scoped single instance.
/// </summary>
public class DialogueManager : Singleton<DialogueManager>
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public Button continueButton;
    public Button skipButton;
    //public Toggle autoPlayToggle;

    [Header("Auto-Play Settings")]
    public float autoPlayDelay = 2.5f;
    public float delayPerCharacter = 0.05f;

    [Header("Typing Settings")]
    public float typingSpeed = 0.03f;
    //public AudioClip typingSound;
    public float typingSoundInterval = 0.1f;

    [Header("Visual Effects")]
    public Animator panelAnimator;
    public string showTrigger = "Show";
    public string hideTrigger = "Hide";

    [Header("Events")]
    public UnityEvent onDialogueStart;
    public UnityEvent onDialogueEnd;
    public UnityEvent onLineStart;
    public UnityEvent onLineEnd;

    private Queue<DialogueLine> dialogueQueue = new Queue<DialogueLine>();
    private bool isTyping = false;
    private bool isAutoPlaying = false;
    private string currentFullText;
    private Coroutine typingCoroutine;
    private Coroutine autoPlayCoroutine;
   // private AudioSource audioSource;
    private DialogueLine currentLine;

    private void Start()
    {
        dialoguePanel.SetActive(false);

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);

        if (skipButton != null)
            skipButton.onClick. AddListener(SkipAllDialogue);

        // if (autoPlayToggle != null)
        // {
        //     autoPlayToggle.onValueChanged.AddListener(OnAutoPlayToggled);
        //     isAutoPlaying = autoPlayToggle.isOn;
        // }
    }

    public void StartDialogue(DialogueLine[] lines)
    {
        dialogueQueue.Clear();

        foreach (DialogueLine line in lines)
        {
            dialogueQueue.Enqueue(line);
        }

        dialoguePanel.SetActive(true);

        // if (panelAnimator != null)
        //     panelAnimator.SetTrigger(showTrigger);

        onDialogueStart?. Invoke();
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (autoPlayCoroutine != null)
        {
            StopCoroutine(autoPlayCoroutine);
            autoPlayCoroutine = null;
        }

        if (isTyping)
        {
            CompleteTyping();
            return;
        }

        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        onLineEnd?.Invoke();

        currentLine = dialogueQueue.Dequeue();
        speakerNameText. text = currentLine. speakerName;
        currentFullText = currentLine.dialogue;

        // Move camera if target specified
        if (currentLine.cameraTarget != null && CinematicCameraController.Instance != null)
        {
            CinematicCameraController.Instance. MoveToTarget(
                currentLine.cameraTarget,
                currentLine. cameraTransitionTime
            );
        }

        // Trigger animation
        if (currentLine.speakerAnimator != null && ! string.IsNullOrEmpty(currentLine. animationTrigger))
        {
            currentLine.speakerAnimator.SetTrigger(currentLine.animationTrigger);
        }

        // Play voice clip
        // if (currentLine.voiceClip != null)
        // {
        //     audioSource.PlayOneShot(currentLine.voiceClip);
        // }

        onLineStart?.Invoke();
        typingCoroutine = StartCoroutine(TypeDialogue(currentLine. dialogue));
    }

    private IEnumerator TypeDialogue(string text)
    {
        isTyping = true;
        dialogueText.text = "";

       // float lastSoundTime = 0f;

        foreach (char letter in text. ToCharArray())
        {
            dialogueText.text += letter;

            // if (typingSound != null && Time.time - lastSoundTime >= typingSoundInterval)
            // {
            //    // audioSource.PlayOneShot(typingSound, 0.5f);
            //     lastSoundTime = Time. time;
            // }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        OnTypingComplete();
    }

    private void CompleteTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = currentFullText;
        isTyping = false;
        OnTypingComplete();
    }

    private void OnTypingComplete()
{
    // Reset the speaker's animation to idle
    if (currentLine != null && currentLine.speakerAnimator != null)
    {
        // Reset the talk trigger first
        currentLine. speakerAnimator.ResetTrigger("Talk");
        
        // Then set idle
        currentLine. speakerAnimator.SetTrigger("Idle");
        
        // Alternative: Force play idle directly
        // currentLine.speakerAnimator.Play("Idle", 0, 0f);
    }

    if (isAutoPlaying && dialogueQueue.Count > 0)
    {
        float delay = autoPlayDelay + (currentFullText.Length * delayPerCharacter);
        autoPlayCoroutine = StartCoroutine(AutoPlayNextLine(delay));
    }
}

    private IEnumerator AutoPlayNextLine(float delay)
    {
        yield return new WaitForSeconds(delay);
        DisplayNextLine();
    }

    private void OnContinueClicked()
    {
        DisplayNextLine();
    }

    private void OnAutoPlayToggled(bool isOn)
    {
        isAutoPlaying = isOn;

        if (isAutoPlaying && ! isTyping && dialogueQueue.Count > 0)
        {
            float delay = autoPlayDelay + (currentFullText.Length * delayPerCharacter);
            autoPlayCoroutine = StartCoroutine(AutoPlayNextLine(delay));
        }
        else if (!isAutoPlaying && autoPlayCoroutine != null)
        {
            StopCoroutine(autoPlayCoroutine);
            autoPlayCoroutine = null;
        }
    }

    public void SkipAllDialogue()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        if (autoPlayCoroutine != null)
            StopCoroutine(autoPlayCoroutine);

        dialogueQueue.Clear();
        EndDialogue();
    }

    private void EndDialogue()
    {
        onLineEnd?.Invoke();

        if (panelAnimator != null)
        {
            panelAnimator.SetTrigger(hideTrigger);
            StartCoroutine(HidePanelAfterAnimation());
         }
        else
        {
            dialoguePanel. SetActive(false);
            onDialogueEnd?. Invoke();
        }
    }

    private IEnumerator HidePanelAfterAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        dialoguePanel.SetActive(false);
        onDialogueEnd?.Invoke();
    }

    public bool IsDialogueActive()
    {
        return dialoguePanel.activeSelf;
    }
}
