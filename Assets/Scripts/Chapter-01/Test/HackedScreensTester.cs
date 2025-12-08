using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteAlways]
public class HackedScreensTester : MonoBehaviour
{
    [Header("Enable only in Play Mode when testing")]
    public bool enableTesting = false;

    [Header("Simulated cleared state per hacked screen")]
    public bool hacked1;
    public bool hacked2;
    public bool hacked3;
    public bool hacked4;
    public bool hacked5;

    [Header("Optional reference (if not found automatically)")]
    public MonitorScreenManager monitorScreenManager;

    private bool lastH1, lastH2, lastH3, lastH4, lastH5;

    private void OnEnable()
    {
        if (!Application.isPlaying) return;

        // Load real progress at startup so toggles reflect it
        SessionProgress.Load();
        SyncFromSession();
        CacheLast();
        TryFindMonitorManager();
    }

    private void Update()
    {
        if (!Application.isPlaying) return;
        if (!enableTesting) return;

        // If any toggle changed in the Inspector, push to SessionProgress
        if (TogglesChanged())
        {
            ApplyToSession();
            CacheLast();
            RefreshScreens();
        }
    }

    private void TryFindMonitorManager()
    {
        if (!monitorScreenManager)
            monitorScreenManager = FindObjectOfType<MonitorScreenManager>();
    }

    private void SyncFromSession()
    {
        hacked1 = SessionProgress.IsScreenCleared(0);
        hacked2 = SessionProgress.IsScreenCleared(1);
        hacked3 = SessionProgress.IsScreenCleared(2);
        hacked4 = SessionProgress.IsScreenCleared(3);
        hacked5 = SessionProgress.IsScreenCleared(4);
    }

    private void ApplyToSession()
    {
        // Start from a clean mask
        SessionProgress.Reset();

        if (hacked1) SessionProgress.MarkScreenCleared(0);
        if (hacked2) SessionProgress.MarkScreenCleared(1);
        if (hacked3) SessionProgress.MarkScreenCleared(2);
        if (hacked4) SessionProgress.MarkScreenCleared(3);
        if (hacked5) SessionProgress.MarkScreenCleared(4);

        Debug.Log($"[HackedScreensTester] Applied test state. Count={SessionProgress.HackedClearedCount}");
    }

    private void RefreshScreens()
    {
        TryFindMonitorManager();
        if (monitorScreenManager)
        {
            monitorScreenManager.ApplyState();
        }
        else
        {
            Debug.LogWarning("[HackedScreensTester] MonitorScreenManager not found in scene.");
        }
    }

    private bool TogglesChanged()
    {
        return hacked1 != lastH1 ||
               hacked2 != lastH2 ||
               hacked3 != lastH3 ||
               hacked4 != lastH4 ||
               hacked5 != lastH5;
    }

    private void CacheLast()
    {
        lastH1 = hacked1;
        lastH2 = hacked2;
        lastH3 = hacked3;
        lastH4 = hacked4;
        lastH5 = hacked5;
    }
}
