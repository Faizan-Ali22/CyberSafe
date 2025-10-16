using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    private Camera introCamera;
    private Camera mainCamera;

    void Start()
    {
        introCamera = GameObject.FindGameObjectWithTag("IntroCamera")?.GetComponent<Camera>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<Camera>();

        bool useMainCamera = true; // Default to main camera

        if (PlayerPrefs.HasKey("UseMainCamera"))
            useMainCamera = PlayerPrefs.GetInt("UseMainCamera") == 1;

        if (useMainCamera)
        {
            if (mainCamera != null) mainCamera.gameObject.SetActive(true);
            if (introCamera != null) introCamera.gameObject.SetActive(false);
        }
        else
        {
            if (introCamera != null) introCamera.gameObject.SetActive(true);
            if (mainCamera != null) mainCamera.gameObject.SetActive(false);
        }
    }
}
