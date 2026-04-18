using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentCanvasOrderFix : MonoBehaviour
{
    [SerializeField] private Canvas pauseCanvas;   // drag your persistent pause canvas
    [SerializeField] private int sortingOrder = 500;

    private void Awake()
    {
        Apply();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Apply(); // re-apply after every scene load
    }

    private void Apply()
    {
        if (pauseCanvas == null)
            pauseCanvas = GetComponentInChildren<Canvas>(true);

        if (pauseCanvas == null) return;

        pauseCanvas.overrideSorting = true;
        pauseCanvas.sortingOrder = sortingOrder; // higher than minimap canvas
    }
}