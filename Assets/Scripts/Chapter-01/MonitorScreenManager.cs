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
                bool cleared = SessionProgress.IsScreenCleared(i);
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
    }
}