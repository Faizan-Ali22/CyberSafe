using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class Task2Controller : MonoBehaviour
{
    [Header("Goal")]
    [SerializeField] private int requiredClears = 2;

    [Header("Task2 UI")]
    [SerializeField] private GameObject task2Panel;           
    [SerializeField] private TMP_Text task2ProgressText;     
    [SerializeField] private GameObject task2CompletedTick;   

    [Header("Reward Popup")]
    [SerializeField] private GameObject completionPopupRoot;  
    [SerializeField] private TMP_Text badgeTitleText;         
    [SerializeField] private TMP_Text nextInstructionText;    
    [SerializeField] private Button continueButton;           

    [Header("Scene Transition")]
    [Tooltip("Type the exact name of the Scene you want to load (e.g., MainMenu or Reward01)")]
    [SerializeField] private string nextSceneName = "Reward01";

    [Header("Scene Audio")]
    [Tooltip("The AudioSource that plays the 4-second looping background music.")]
    [SerializeField] private AudioSource backgroundMusicSource;
    [Tooltip("The AudioSource that plays the 1.2-second badge earned sound.")]
    [SerializeField] private AudioSource badgeEarnedSource;
    [Tooltip("The AudioSource that plays the 54-second school interior sound on loop.")]
    [SerializeField] private AudioSource schoolInteriorSource;

    [Header("Unlock Settings")]
    [Tooltip("Index of the Chapter to unlock when completed. (Chapter 2 = index 1)")]
    [SerializeField] private int unlockChapterIndex = 1;

    [Header("PlayerPrefs Keys")]
    [SerializeField] private string task2CompleteKey = "Task2Completed";

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
            
            // If they returned to the level via select menu and it's already done, 
            // open the escape door (continuing point) immediately!
            ShowCompletionPopup(); 
        }
        else
        {
            // Only play background music if the task is NOT already completed
            if (backgroundMusicSource != null)
            {
                backgroundMusicSource.loop = true;
                if (!backgroundMusicSource.isPlaying)
                {
                    backgroundMusicSource.Play();
                }
            }

            // ADD THIS: Play the school interior background sound
            if (schoolInteriorSource != null)
            {
                schoolInteriorSource.loop = true;
                if (!schoolInteriorSource.isPlaying)
                {
                    schoolInteriorSource.Play();
                }
            }
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
            task2ProgressText.text = $"{cleared}/{requiredClears}";

        // Force the tick OFF whenever we refresh initially
        if (task2CompletedTick != null)
            task2CompletedTick.SetActive(false);

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

        // Tell ProgressManager exactly which elements to Complete and Unlock
        if (ProgressManager.Instance != null) 
        {
            // Set Chapter 0 and 1 as Completed
            ProgressManager.Instance.SetChapterCompletedOnly(0, true);
            ProgressManager.Instance.SetChapterCompletedOnly(1, true);

            // Set Chapter 0 and 1 as Unlocked
            ProgressManager.Instance.SetChapterUnlocked(0, true);
            ProgressManager.Instance.SetChapterUnlocked(1, true);
            
            // Explicitly lock Chapter 2 in case it was accidentally unlocked before
            ProgressManager.Instance.SetChapterUnlocked(2, false); 
        }

        ApplyCompletedStateUI();

        // Immediately kill the looping beep sound 
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Stop();
        }

        // ADD THIS: Stop the school interior sound
        if (schoolInteriorSource != null)
        {
            schoolInteriorSource.Stop();
        }

        // Show popup instantly! 
        StartCoroutine(ShowPopupWithDelay(1.5f));
        
        Debug.Log("[Task2Controller] Task2 complete. Badge showing + exact Chapters Updated!");
    }

    private IEnumerator ShowPopupWithDelay(float delay)
    {
        // Wait extremely brief 0.5s so text says "Complete" first
        yield return new WaitForSeconds(delay);

        if (badgeEarnedSource != null)
        {
            badgeEarnedSource.loop = true; // Make it loop here
            badgeEarnedSource.Play();
        }

        ShowCompletionPopup();
    }

    private void ApplyCompletedStateUI()
    {
        if (task2ProgressText != null)
            task2ProgressText.text = "Complete";

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
            nextInstructionText.text = "Great job! Chapter 2 is now unlocked.";
    }

    public void CloseCompletionPopup()
    {
        if (completionPopupRoot != null)
            completionPopupRoot.SetActive(false);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
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

    public static Task2Controller GetInstance()
    {
        return Object.FindFirstObjectByType<Task2Controller>();
    }
}