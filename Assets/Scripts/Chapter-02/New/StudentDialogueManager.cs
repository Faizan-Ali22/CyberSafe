using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
public class StudentDialogueManager : MonoBehaviour
{
   public static StudentDialogueManager Instance;

    [Header("UI References")]
    public GameObject dialoguePanel; // The UI Panel to turn ON
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI bodyText;
    public Button nextButton;

    [HideInInspector]
    public UnityEvent onDialogueEnd;

    private StudentDialogueLine[] currentLines;
    private int index;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Auto-hide at start
        if (dialoguePanel != null) 
            dialoguePanel.SetActive(false);
        else 
            Debug.LogError("StudentDialogueManager: You forgot to assign the 'Dialogue Panel' in the Inspector!");
            
        if (nextButton != null) 
            nextButton.onClick.AddListener(DisplayNextLine);
    }

    public void StartDialogue(StudentDialogueLine[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            Debug.LogError("StudentDialogueManager: No dialogue lines were sent! Check the Student's Inspector.");
            EndDialogue();
            return;
        }

        currentLines = lines;
        index = 0;

        Debug.Log("StudentDialogueManager: Starting Dialogue...");

        if (dialoguePanel != null) 
        {
            dialoguePanel.SetActive(true); // <--- This turns on the UI
            Debug.Log("StudentDialogueManager: Panel set to ACTIVE.");
        }
        
        ShowLine();
    }

    private void ShowLine()
    {
        if (index >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        StudentDialogueLine line = currentLines[index];
        
        // Update UI
        if (nameText != null) nameText.text = line.speakerName;
        if (bodyText != null) bodyText.text = line.dialogue;

        // Play Animation
        if (line.speakerAnimator != null && !string.IsNullOrEmpty(line.animationTrigger))
        {
            line.speakerAnimator.SetTrigger(line.animationTrigger);
        }

        // Camera Move (Optional)
        if (StudentCinematicManager.Instance != null && line.cameraTarget != null)
        {
            StudentCinematicManager.Instance.SwitchToCinematicCamera(line.cameraTarget, line.cameraTransitionTime);
        }

        index++;
    }

    public void DisplayNextLine()
    {
        ShowLine();
    }

    private void EndDialogue()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        onDialogueEnd.Invoke();
    }
}
