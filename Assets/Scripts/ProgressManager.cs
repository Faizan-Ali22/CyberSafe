using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProgressManager : PersistentSingleton<ProgressManager>
{
    public const int ChapterCount = 8;

    public bool[] chapterCompleted = new bool[ChapterCount];
    public bool[] chapterUnlocked = new bool[ChapterCount];

    private const string SaveKeyCompleted = "ChapterCompleted";
    private const string SaveKeyUnlocked = "ChapterUnlocked";

    protected override void OnPersistentSingletonAwake()
    {
        InitializeDefaultUnlocks();
        LoadProgress();
    }

    private void InitializeDefaultUnlocks()
    {
        for (int i = 0; i < ChapterCount; i++)
        {
            chapterUnlocked[i] = (i == 0);
            chapterCompleted[i] = false;
        }
    }

    public bool IsChapterUnlocked(int chapterIndex)
    {
        if (!IsValidChapter(chapterIndex)) return false;
        return chapterUnlocked[chapterIndex];
    }

    public bool IsChapterCompleted(int chapterIndex)
    {
        if (!IsValidChapter(chapterIndex)) return false;
        return chapterCompleted[chapterIndex];
    }

    public void SetChapterCompleted(int chapterIndex, bool completed = true)
    {
        if (!IsValidChapter(chapterIndex))
        {
            Debug.LogWarning($"ProgressManager: Invalid chapter index {chapterIndex}");
            return;
        }

        chapterCompleted[chapterIndex] = completed;

        if (completed)
        {
            int nextIndex = chapterIndex + 1;
            if (IsValidChapter(nextIndex))
                chapterUnlocked[nextIndex] = true;
        }

        SaveProgress();
    }

    public void SetChapterCompletedOnly(int chapterIndex, bool completed = true)
    {
        if (!IsValidChapter(chapterIndex)) return;
        chapterCompleted[chapterIndex] = completed;
        SaveProgress();
    }

    public void SetChapterUnlocked(int chapterIndex, bool unlocked = true)
    {
        if (!IsValidChapter(chapterIndex)) return;
        chapterUnlocked[chapterIndex] = unlocked;
        SaveProgress();
    }

    public void ResetAllProgress()
    {
        InitializeDefaultUnlocks();
        PlayerPrefs.DeleteKey(SaveKeyCompleted);
        PlayerPrefs.DeleteKey(SaveKeyUnlocked);
        PlayerPrefs.Save();
        Debug.Log("ProgressManager: All progress reset.");
    }

    private void SaveProgress()
    {
        char[] completedBits = new char[ChapterCount];
        char[] unlockedBits = new char[ChapterCount];

        for (int i = 0; i < ChapterCount; i++)
        {
            completedBits[i] = chapterCompleted[i] ? '1' : '0';
            unlockedBits[i] = chapterUnlocked[i] ? '1' : '0';
        }

        PlayerPrefs.SetString(SaveKeyCompleted, new string(completedBits));
        PlayerPrefs.SetString(SaveKeyUnlocked, new string(unlockedBits));
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        if (!PlayerPrefs.HasKey(SaveKeyCompleted) || !PlayerPrefs.HasKey(SaveKeyUnlocked))
            return;

        string completedData = PlayerPrefs.GetString(SaveKeyCompleted);
        string unlockedData = PlayerPrefs.GetString(SaveKeyUnlocked);

        int limitCompleted = Mathf.Min(completedData.Length, ChapterCount);
        int limitUnlocked = Mathf.Min(unlockedData.Length, ChapterCount);

        for (int i = 0; i < limitCompleted; i++)
            chapterCompleted[i] = (completedData[i] == '1');

        for (int i = 0; i < limitUnlocked; i++)
            chapterUnlocked[i] = (unlockedData[i] == '1');
    }

    private bool IsValidChapter(int index)
    {
        return index >= 0 && index < ChapterCount;
    }

    [ContextMenu("Wipe Data Now")]
    public void WipeData()
    {
        SessionProgress.Reset();
        PlayerPrefs.DeleteKey("Task2Completed");
        PlayerPrefs.DeleteKey("Task2RewardShown");
        PlayerPrefs.DeleteKey("Task2Completed_v2");
        PlayerPrefs.DeleteKey("Task2RewardShown_v2");

        ResetAllProgress();
        PlayerPrefs.Save();

        Debug.Log("All progress fully wiped! Starting a fresh run.");
    }
}
