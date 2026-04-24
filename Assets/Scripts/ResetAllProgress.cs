using UnityEngine;

public class ResetAllProgress : MonoBehaviour
{
    [Tooltip("Check this box to wipe data on start. UNCHECK it when playing normally!")]
    public bool wipeOnStart = false;

    private void Start()
    {
        if (wipeOnStart)
        {
            WipeData();
        }
    }

    [ContextMenu("Wipe Data Now")]
    public void WipeData()
    {
        SessionProgress.Reset();

        PlayerPrefs.DeleteKey("Task2Completed");
        PlayerPrefs.DeleteKey("Task2RewardShown");
        PlayerPrefs.DeleteKey("Task2Completed_v2");
        PlayerPrefs.DeleteKey("Task2RewardShown_v2");

        PlayerPrefs.DeleteKey("ChapterCompleted");
        PlayerPrefs.DeleteKey("ChapterUnlocked");

        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.ResetAllProgress();
        }

        PlayerPrefs.Save();
        Debug.Log("All progress fully wiped! Starting a fresh run.");
    }
}
