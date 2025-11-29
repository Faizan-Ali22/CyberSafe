using UnityEngine;

public class ProgressDebugTools : MonoBehaviour
{
     [ContextMenu("Reset All Progress")]
    public void ResetProgressContextMenu()
    {
        if (ProgressManager.Instance != null)
            ProgressManager.Instance.ResetAllProgress();
    }

    public void ResetProgressFromButton()
    {
        if (ProgressManager.Instance != null)
            ProgressManager.Instance.ResetAllProgress();
    }
}
