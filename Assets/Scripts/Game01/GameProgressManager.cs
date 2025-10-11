using UnityEngine;
using UnityEngine.UI;

public class GameProgressManager : MonoBehaviour
{
     public static GameProgressManager Instance;
     public Text savedText;          // assign "Saved colleges 0/5" UI Text
     public int totalToSave = 5;

    int savedCount = 0;

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else Destroy(gameObject);

        UpdateUI();
    }

    public void IncrementSaved()
    {
        savedCount = Mathf.Clamp(savedCount + 1, 0, totalToSave);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (savedText != null)
            savedText.text = $"Saved colleges {savedCount}/{totalToSave}";
    }

    public int GetSavedCount() => savedCount;
}
