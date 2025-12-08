using UnityEngine;
using System.Collections;
public class MonitorScreenManager : MonoBehaviour
{
    [Tooltip("Order: Hacked-1 .. Hacked-8")]
    public GameObject[] hackedScreens;

    private void Awake()
    {
        SessionProgress.Load();
        ApplyState();
    }

    public void ApplyState()
    {
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
        SessionProgress.MarkScreenCleared(index);
        ApplyState();
    }
}
