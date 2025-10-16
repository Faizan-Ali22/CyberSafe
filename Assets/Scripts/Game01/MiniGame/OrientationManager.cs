using UnityEngine;

public class OrientationManager : MonoBehaviour
{
   void Start()
    {
        // Check if we're in RedLinksScene
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "RedLinksScene")
        {
            // Set to portrait
            Screen.orientation = ScreenOrientation.Portrait;
        }
        else
        {
            // Set to landscape for other scenes
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
    }

    void OnDestroy()
    {
        // Reset to landscape when leaving the scene
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }
}
