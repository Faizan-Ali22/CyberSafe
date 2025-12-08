using UnityEngine;

public class SessionProgress : MonoBehaviour
{
    private const string HackedClearedKey = "HackedClearedCount";   // legacy count (kept for completeness)
    private const string HackedMaskKey   = "HackedClearedMask";     // bitmask of cleared screens

    public static int HackedClearedCount { get; private set; }
    public static int HackedClearedMask  { get; private set; }

    public static void Load()
    {
        HackedClearedCount = PlayerPrefs.GetInt(HackedClearedKey, 0);
        HackedClearedMask  = PlayerPrefs.GetInt(HackedMaskKey, 0);
    }

    public static bool IsScreenCleared(int index)
    {
        if (index < 0 || index >= 31) return false;
        return (HackedClearedMask & (1 << index)) != 0;
    }

    public static void MarkScreenCleared(int index)
    {
        if (index < 0 || index >= 31) return;

        int beforeMask = HackedClearedMask;
        HackedClearedMask |= (1 << index);
        if (HackedClearedMask != beforeMask)
        {
            HackedClearedCount = CountBits(HackedClearedMask);
            Save();
        }
    }

    public static void Reset()
    {
        HackedClearedCount = 0;
        HackedClearedMask  = 0;
        PlayerPrefs.DeleteKey(HackedClearedKey);
        PlayerPrefs.DeleteKey(HackedMaskKey);
        PlayerPrefs.Save();
    }

    private static void Save()
    {
        PlayerPrefs.SetInt(HackedClearedKey, HackedClearedCount);
        PlayerPrefs.SetInt(HackedMaskKey, HackedClearedMask);
        PlayerPrefs.Save();
    }

    private static int CountBits(int v)
    {
        
        int c = 0;
        while (v != 0) { v &= (v - 1); c++; }
        return c;
    }
}
