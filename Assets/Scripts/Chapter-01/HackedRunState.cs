using UnityEngine;

public static class HackedRunState
{
    private const string LastHackedIndexKey = "LastHackedIndex";

    public static void SetLastHackedIndex(int index)
    {
        PlayerPrefs.SetInt(LastHackedIndexKey, index);
        PlayerPrefs.Save();
    }

    public static int GetLastHackedIndex()
    {
        return PlayerPrefs.GetInt(LastHackedIndexKey, -1);
    }

    public static void Reset()
    {
        PlayerPrefs.DeleteKey(LastHackedIndexKey);
        PlayerPrefs.Save();
    }
}