using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalPauseManager : MonoBehaviour
{
    public static GlobalPauseManager Instance { get; private set; }

    [Header("UI Root (inside this prefab)")]
    [SerializeField] private GameObject pauseCanvasRoot;   // full-screen canvas root
    [SerializeField] private GameObject pausePanel;        // modal panel
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button backToMenuButton;

    [Header("Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private bool useEscapeKey = true;

    [Header("Optional: Hide pause in these scenes")]
    [SerializeField] private string[] disableInScenes;

    private bool isPaused;

    public bool IsPaused => isPaused;

    private void Awake()
    {
        // Singleton + persist
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Wire buttons once
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveListener(ResumeGame);
            resumeButton.onClick.AddListener(ResumeGame);
        }

        if (backToMenuButton != null)
        {
            backToMenuButton.onClick.RemoveListener(BackToMainMenu);
            backToMenuButton.onClick.AddListener(BackToMainMenu);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;

        // Start hidden + unpaused
        SetPauseUI(false);
        ForceUnpause();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;

        // Safety
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    private void Update()
    {
        if (useEscapeKey && Input.GetKeyDown(KeyCode.Escape))
        {
            if (!CanPauseInCurrentScene()) return;

            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Always unpause on scene load to avoid stuck timescale issues
        ForceUnpause();

        bool canPause = CanPauseInCurrentScene();
        if (pauseCanvasRoot != null)
            pauseCanvasRoot.SetActive(canPause);
    }

    private bool CanPauseInCurrentScene()
    {
        string current = SceneManager.GetActiveScene().name;
        if (disableInScenes == null) return true;

        for (int i = 0; i < disableInScenes.Length; i++)
        {
            if (!string.IsNullOrEmpty(disableInScenes[i]) && disableInScenes[i] == current)
                return false;
        }
        return true;
    }

    // Hook this to top pause button in any scene
    public void TogglePause()
    {
        if (!CanPauseInCurrentScene()) return;

        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        if (isPaused) return;
        if (!CanPauseInCurrentScene()) return;

        isPaused = true;
        SetPauseUI(true);

        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void ResumeGame()
    {
        if (!isPaused) return;
        ForceUnpause();
    }

    public void BackToMainMenu()
    {
        ForceUnpause();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void ForceUnpause()
    {
        isPaused = false;
        SetPauseUI(false);

        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    private void SetPauseUI(bool visible)
    {
        if (pausePanel != null)
            pausePanel.SetActive(visible);
    }
}