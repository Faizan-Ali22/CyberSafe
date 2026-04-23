using UnityEngine;

public class ResetAllProgress : MonoBehaviour
{
    [Tooltip("Check this box to wipe data on start. UNCHECK it when playing normally!")]
    public bool wipeOnStart = false;

    private void Start()
    {
        // Only wipes if you explicitly check the box!
        if (wipeOnStart)
        {
            WipeData();
        }
    }

    [ContextMenu("Wipe Data Now")]
    public void WipeData()
    {
        // 1. Clears hacked screens count
        SessionProgress.Reset();    

        // 2. Clears saved pose, teacher dialogue flag, etc.
        LabReturnState.Reset();     

        // 3. Wipes the Task 2 completion keys we made
        PlayerPrefs.DeleteKey("Task2Completed");
        PlayerPrefs.DeleteKey("Task2RewardShown");
        PlayerPrefs.DeleteKey("Task2Completed_v2");
        PlayerPrefs.DeleteKey("Task2RewardShown_v2");

        // 4. Save the wiped state
        PlayerPrefs.Save();

        Debug.Log("✅ All progress fully wiped! Starting a fresh run.");
    }
}
