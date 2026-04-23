using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonitorScreenManager : MonoBehaviour
{
    [Tooltip("Assign ONLY the 2 hacked overlays to clear (index 0 and 1).")]
    public GameObject[] hackedScreens;

    private void Awake()
    {
        SessionProgress.Load();
        ApplyState();
    }

    public void ApplyState()
    {
        if (hackedScreens == null) return;

        for (int i = 0; i < hackedScreens.Length; i++)
        {
            if (hackedScreens[i] != null)
            {
                // If it IS cleared, this returns true. We want SetActive(false) if true.
                bool cleared = SessionProgress.IsScreenCleared(i);
                
                // This turns off the Hacked visual AND the button inside it!
                hackedScreens[i].SetActive(!cleared);
            }
        }
    }

    public void ClearScreen(int index)
    {
        if (index < 0 || hackedScreens == null || index >= hackedScreens.Length)
        {
            Debug.LogWarning($"[MonitorScreenManager] ClearScreen index out of range: {index}");
            return;
        }

        SessionProgress.MarkScreenCleared(index);
        ApplyState();

        // Notify Task 2 to update UI and check for completion
        var task2 = Object.FindFirstObjectByType<Task2Controller>(FindObjectsInactive.Include);
        if (task2 != null)
        {
            task2.RefreshTask2UI();
            task2.TryCompleteTask2();
        }
    }
}