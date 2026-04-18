using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Task2Controller : MonoBehaviour
{
    [Header("Goal")]
    [SerializeField] private int requiredClears = 2;

    [Header("Task2 UI")]
    [SerializeField] private GameObject task2Panel;           // Task2 panel root
    [SerializeField] private TMP_Text task2ProgressText;      // "Task 2: Check PCs (x/2)"
    [SerializeField] private GameObject task2CompletedTick;   // optional check icon

    [Header("Reward Popup")]
    [SerializeField] private GameObject completionPopupRoot;  // popup panel root
    [SerializeField] private TMP_Text badgeTitleText;         // "Badge Earned: Malware Defender"
    [SerializeField] private TMP_Text nextInstructionText;    // next instruction text
    [SerializeField] private Button continueButton;           // close popup button

    [Header("Unlock Settings")]
    [Tooltip("Chapter 2 = index 1")]
    [SerializeField] private int unlockChapterIndex = 1;

    [Header("PlayerPrefs Keys")]
    [SerializeField] private string task2CompleteKey = "Task2Completed";
    [SerializeField] private string task2RewardShownKey = "Task2RewardShown";

    private void Awake()
    {
        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(CloseCompletionPopup);
            continueButton.onClick.AddListener(CloseCompletionPopup);
        }

        if (completionPopupRoot != null)
            completionPopupRoot.SetActive(false);

        if (task2CompletedTick != null)
            task2CompletedTick.SetActive(false);
    }

    private void Start()
    {
        RefreshTask2UI();

        if (IsTask2Completed())
        {
            ApplyCompletedStateUI();
        }
    }

    private void OnEnable()
    {
        RefreshTask2UI();
        TryCompleteTask2();
    }

    public void RefreshTask2UI()
    {
        int cleared = Mathf.Min(SessionProgress.HackedClearedCount, requiredClears);

        if (task2ProgressText != null)
            task2ProgressText.text = $"Task 2: Check PCs ({cleared}/{requiredClears})";

        if (task2Panel != null)
            task2Panel.SetActive(true);
    }

    public void TryCompleteTask2()
    {
        if (IsTask2Completed()) return;

        if (SessionProgress.HackedClearedCount >= requiredClears)
        {
            CompleteTask2();
        }
    }

    private void CompleteTask2()
    {
        SetTask2Completed(true);

        // Unlock Chapter 2
        ChapterUnlockProgress.UnlockChapter(unlockChapterIndex);

        ApplyCompletedStateUI();

        bool alreadyShown = PlayerPrefs.GetInt(task2RewardShownKey, 0) == 1;
        if (!alreadyShown)
        {
            ShowCompletionPopup();
            PlayerPrefs.SetInt(task2RewardShownKey, 1);
            PlayerPrefs.Save();
        }

        Debug.Log("[Task2Controller] Task2 complete. Badge + Chapter2 unlocked.");
    }

    private void ApplyCompletedStateUI()
    {
        if (task2ProgressText != null)
            task2ProgressText.text = "Task 2: Check PCs (Complete)";

        if (task2CompletedTick != null)
            task2CompletedTick.SetActive(true);
    }

    private void ShowCompletionPopup()
    {
        if (completionPopupRoot != null)
            completionPopupRoot.SetActive(true);

        if (badgeTitleText != null)
            badgeTitleText.text = "Badge Earned: Malware Defender";

        if (nextInstructionText != null)
            nextInstructionText.text = "Great job! Chapter 2 is now unlocked. Go to Main Menu > Chapter Select to continue.";
    }

    public void CloseCompletionPopup()
    {
        if (completionPopupRoot != null)
            completionPopupRoot.SetActive(false);
    }

    private bool IsTask2Completed()
    {
        return PlayerPrefs.GetInt(task2CompleteKey, 0) == 1;
    }

    private void SetTask2Completed(bool completed)
    {
        PlayerPrefs.SetInt(task2CompleteKey, completed ? 1 : 0);
        PlayerPrefs.Save();
    }
}