using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

    void Awake()
    {
        // Singleton pattern for persistence across scenes
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

        LoadProgress();
        UpdateUI();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 🟩 Automatically re-hook UI if scene changes
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Try finding the savedText again when returning to main campus scene
        if (savedText == null)
        {
            var foundText = GameObject.Find("SavedText"); // <--- This looks for a game object named EXACTLY "SavedText"
            if (foundText != null)
                savedText = foundText.GetComponent<TMP_Text>();
        }

        UpdateUI();
    }

    // 🟩 Called from RightChoiceController (after player saves a student)
    public void IncrementSaved(string studentID)
    {
        // Don't count the same student twice!
        if (savedNPCs.Contains(studentID)) return;

        savedNPCs.Add(studentID);
        savedCount = Mathf.Clamp(savedCount + 1, 0, totalToSave);
        
        SaveProgress();
        UpdateUI();

        if (savedCount >= totalToSave)
            OnAllStudentsSaved();
    }

    // 🟩 Update UI text
    private void UpdateUI()
    {
        if (savedText != null)
        {
            savedText.text = $"Saved colleges {savedCount}/{totalToSave}";
            
            // Force TMP to update its mesh (fixes the invisible text bug)
            savedText.ForceMeshUpdate(true);
        }
    }

    // 🟩 Save & Load PlayerPrefs
    private void SaveProgress()
    {
        PlayerPrefs.SetInt("SavedCount", savedCount);
        // Convert saved NPCs to string and save
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

    // 🟩 What happens when all are saved
    private void OnAllStudentsSaved()
    {
        Debug.Log("🎉 All students have been saved from Phishing Attacks! (2/2)");

        // 1. Tell ProgressManager that Chapter Index 2 (which is Element 2) is Completed!
        // By default, SetChapterCompleted automatically unlocks the next chapter (Element 3).
        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.SetChapterCompleted(2, true); 
        }

        // 2. TODO: Trigger your Badge PopUp UI here.
        // Something like:
        // if (badgePanel != null) badgePanel.SetActive(true);
    }

    // 🟩 Optional manual reset for debugging or new game
    [ContextMenu("Reset Progress (Count Only)")]
    public void ResetProgressCountOnly()
    {
        savedCount = 0;
        PlayerPrefs.DeleteKey("SavedCount");
        UpdateUI();
    }

    // 🟩 Reset all progress and PlayerPrefs
    [ContextMenu("Reset All Progress")]
    public void ResetAllProgress()
    {
        // Reset local variables
        savedCount = 0;
        savedNPCs.Clear();

        // Clear all PlayerPrefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Update UI
        UpdateUI();
        Debug.Log("🔄 All progress and PlayerPrefs have been reset!");
    }

    // 🟩 Reset only game progress
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

    // Add this Update method
    void Update()
    {
        if (resetNow)
        {
            // Reset this script's progress (0/2 students)
            ResetAllProgress();
            
            // Reset the global ProgressManager (Chapters)
            if (ProgressManager.Instance != null)
            {
                ProgressManager.Instance.ResetAllProgress();
                
                // You mentioned you want it to default to this specific state for testing:
                // Completed: 0, 1. Unlocked: 0, 1, 2.
                ProgressManager.Instance.SetChapterCompletedOnly(0, true);
                ProgressManager.Instance.SetChapterCompletedOnly(1, true);
                
                ProgressManager.Instance.SetChapterUnlocked(0, true);
                ProgressManager.Instance.SetChapterUnlocked(1, true);
                ProgressManager.Instance.SetChapterUnlocked(2, true);
            }

            Debug.Log("🚧 TESTING TRIGGERED: Game Progress and Chapters reset to testing defaults.");
            resetNow = false; // Uncheck the box automatically
        }
    }
}
