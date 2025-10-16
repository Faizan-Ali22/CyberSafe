using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;

    [Header("UI References")]
    [Tooltip("Assign 'Saved colleges 0/5' UI Text in the main scene.")]
    public Text savedText;

    [Header("Progress Settings")]
    public int totalToSave = 5;

    private int savedCount = 0;
    private HashSet<string> savedNPCs = new HashSet<string>();

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
            var foundText = GameObject.Find("SavedText");
            if (foundText != null)
                savedText = foundText.GetComponent<Text>();
        }

        UpdateUI();
    }

    // 🟩 Called from RightChoiceController (after player saves a student)
    public void IncrementSaved()
    {
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
            savedText.text = $"Saved colleges {savedCount}/{totalToSave}";
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
        Debug.Log("🎉 All students have been saved from cyberattacks!");
        // TODO: Trigger a win screen, credits, or unlock next chapter.
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
}
