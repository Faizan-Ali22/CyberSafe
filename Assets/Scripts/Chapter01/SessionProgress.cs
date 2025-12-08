using UnityEngine;

public class SessionProgress : MonoBehaviour
{
    private const string HackedClearedKey = "HackedClearedCount";
    public static int HackedClearedCount { get; private set; }

    public static void Load()
    {
        HackedClearedCount = PlayerPrefs.GetInt(HackedClearedKey, 0);
    }

    public static void AddCleared(int amount = 1)
    {
        HackedClearedCount = Mathf.Clamp(HackedClearedCount + amount, 0, 8);
        PlayerPrefs.SetInt(HackedClearedKey, HackedClearedCount);
        PlayerPrefs.Save();
    }

    public static void Reset()
    {
        HackedClearedCount = 0;
        PlayerPrefs.DeleteKey(HackedClearedKey);
        PlayerPrefs.Save();
    }
}
