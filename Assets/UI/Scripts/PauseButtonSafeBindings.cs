using UnityEngine;
using UnityEngine.UI;

public class PauseButtonSafeBindings : MonoBehaviour
{
    [SerializeField] public Button pauseButton;
    [SerializeField] public Button resumeButton;
    [SerializeField] public Button mainMenuButton;

    private void Awake()
    {
        var mgr = GlobalPauseManager.Instance;
        if (mgr == null) return;

        if (pauseButton != null)
        {
            pauseButton.onClick.RemoveAllListeners();
            pauseButton.onClick.AddListener(mgr.TogglePause);
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(mgr.ResumeGame);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(mgr.BackToMainMenu);
        }
    }
}