using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;

    [Header("UI References")]
    [Tooltip("Assign 'Saved colleges 0/2' UI Text in the main scene.")]
    public TMP_Text savedText;

    [Header("Progress Settings")]
    public int totalToSave = 2;

    private int savedCount = 0;
    private HashSet<string> savedNPCs = new HashSet<string>();

    [Header("Testing & Debug")]
    [Tooltip("Check this box in the Inspector while playing to reset everything instantly.")]
    public bool resetNow = false;

    [Header("End Sequence Properties")]
    public GameObject badgePanel; 
    public TMP_Text badgeTitleText;         
    public TMP_Text nextInstructionText;    
    public Button continueButton;           
    public AudioSource badgeEarnedSource;
    [Tooltip("Type the exact name of the Chapter 3 Cutscene/Scene")]
    public string nextSceneName = "Chapter03"; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Hook up the continue button automatically
        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(CloseCompletionPopup);
            continueButton.onClick.AddListener(CloseCompletionPopup);
        }

        LoadProgress();
        UpdateUI();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (savedText == null)
        {
            var foundText = GameObject.Find("SavedText"); 
            if (foundText != null)
                savedText = foundText.GetComponent<TMP_Text>();
        }

        UpdateUI();
    }

    public void IncrementSaved(string studentID)
    {
        if (savedNPCs.Contains(studentID)) return;

        savedNPCs.Add(studentID);
        savedCount = Mathf.Clamp(savedCount + 1, 0, totalToSave);
        
        SaveProgress();
        UpdateUI();

        if (savedCount >= totalToSave)
            OnAllStudentsSaved();
    }

    private void UpdateUI()
    {
        if (savedText != null)
        {
            savedText.text = $"Saved colleges {savedCount}/{totalToSave}";
            savedText.ForceMeshUpdate(true);
        }
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt("SavedCount", savedCount);
        string savedNPCsString = string.Join(",", savedNPCs);
        PlayerPrefs.SetString("SavedNPCs", savedNPCsString);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        savedCount = PlayerPrefs.GetInt("SavedCount", 0);
        string savedNPCsString = PlayerPrefs.GetString("SavedNPCs", "");
        savedNPCs = new HashSet<string>(
            string.IsNullOrEmpty(savedNPCsString) 
                ? new string[0] 
                : savedNPCsString.Split(',')
        );
    }

    public int GetSavedCount() => savedCount;

    private void OnAllStudentsSaved()
    {
        Debug.Log("🎉 All students have been saved from Phishing Attacks! (2/2)");

        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.SetChapterCompleted(2, true); 
        }

        StartCoroutine(ShowBadgeWithDelay(5f));
    }

    private IEnumerator ShowBadgeWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Update popup text
        if (badgeTitleText != null) badgeTitleText.text = "Badge Earned: Phish Spotter";
        if (nextInstructionText != null) nextInstructionText.text = "Great job! Chapter 3 is now unlocked.";

        // Play the looping sound
        if (badgeEarnedSource != null)
        {
            badgeEarnedSource.loop = true;
            badgeEarnedSource.Play();
        }

        // Show panel
        if (badgePanel != null)
        {
            badgePanel.SetActive(true);
        }
    }

    // 🟩 Called when the Continue button is clicked
    public void CloseCompletionPopup()
    {
        // Stop the sound
        if (badgeEarnedSource != null)
        {
            badgeEarnedSource.Stop();
        }

        // Hide the panel
        if (badgePanel != null)
        {
            badgePanel.SetActive(false);
        }

        // Load the next scene
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    // --- Developer debug functions below ---
    [ContextMenu("Reset Progress (Count Only)")]
    public void ResetProgressCountOnly()
    {
        savedCount = 0;
        PlayerPrefs.DeleteKey("SavedCount");
        UpdateUI();
    }

    [ContextMenu("Reset All Progress")]
    public void ResetAllProgress()
    {
        savedCount = 0;
        savedNPCs.Clear();
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        UpdateUI();
        Debug.Log("🔄 All progress and PlayerPrefs have been reset!");
    }

    [ContextMenu("Reset Game Progress")]
    public void ResetProgress()
    {
        savedCount = 0;
        savedNPCs.Clear();
        PlayerPrefs.DeleteKey("SavedCount");
        PlayerPrefs.DeleteKey("SavedNPCs");
        PlayerPrefs.Save();
        UpdateUI();
        Debug.Log("🔄 Game progress has been reset!");
    }

    public bool WasNPCSaved(string npcName)
    {
        return savedNPCs.Contains(npcName);
    }

    void Update()
    {
        // --- ADD THIS TO BULLETPROOF THE TEXT ---
        // Constantly make sure the text object remains active on screen
        if (savedText != null && !savedText.gameObject.activeSelf)
        {
            savedText.gameObject.SetActive(true);
        }

        if (resetNow)
        {
            ResetAllProgress();
            
            if (ProgressManager.Instance != null)
            {
                ProgressManager.Instance.ResetAllProgress();
                ProgressManager.Instance.SetChapterCompletedOnly(0, true);
                ProgressManager.Instance.SetChapterCompletedOnly(1, true);
                ProgressManager.Instance.SetChapterUnlocked(0, true);
                ProgressManager.Instance.SetChapterUnlocked(1, true);
                ProgressManager.Instance.SetChapterUnlocked(2, true);
            }

            Debug.Log("🚧 TESTING TRIGGERED: Game Progress and Chapters reset.");
            resetNow = false; 
        }
    }
}
