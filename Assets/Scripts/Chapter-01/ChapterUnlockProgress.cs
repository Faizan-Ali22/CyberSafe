using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ChapterUnlockProgress : MonoBehaviour
{
    private const string UnlockedChapterKey = "UnlockedChapterIndex";
    // 0 = Chapter 1, 1 = Chapter 2, 2 = Chapter 3 ...

    public static int GetUnlockedChapterIndex()
    {
        // By default Chapter 1 is unlocked
        return PlayerPrefs.GetInt(UnlockedChapterKey, 0);
    }

    public static bool IsChapterUnlocked(int chapterIndex)
    {
        return chapterIndex <= GetUnlockedChapterIndex();
    }

    public static void UnlockChapter(int chapterIndex)
    {
        int current = GetUnlockedChapterIndex();
        if (chapterIndex > current)
        {
            PlayerPrefs.SetInt(UnlockedChapterKey, chapterIndex);
            PlayerPrefs.Save();
            Debug.Log($"[ChapterUnlockProgress] Unlocked chapter index: {chapterIndex}");
        }
    }

    public static void ResetAll()
    {
        PlayerPrefs.DeleteKey(UnlockedChapterKey);
        PlayerPrefs.Save();
    }
}
