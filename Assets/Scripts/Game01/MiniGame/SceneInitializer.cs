using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneInitializer : MonoBehaviour
{
   [Header("References")]
    public GameObject player;
    public Camera mainCamera;
    public Camera cutsceneCamera;

    void Start()
    {
        // Check if returning from Bilal scene
        if (PlayerPrefs.GetInt("ReturningFromRedLinks", 0) == 1)
        {
            HandleReturnFromBilal();
            PlayerPrefs.DeleteKey("ReturningFromRedLinks");
            PlayerPrefs.Save();
        }
        else
        {
            // Normal scene start - check if cutscene should play
            HandleNormalSceneStart();
        }
    }

    void HandleReturnFromBilal()
    {
        Debug.Log("Returning from RedLink scene - restoring player state");
        
        // Ensure player is active
        if (player != null)
        {
            player.SetActive(true);
            
            // Restore player position if saved
            if (PlayerPrefs.HasKey("PlayerPosX"))
            {
                Vector3 savedPosition = new Vector3(
                    PlayerPrefs.GetFloat("PlayerPosX"),
                    PlayerPrefs.GetFloat("PlayerPosY"),
                    PlayerPrefs.GetFloat("PlayerPosZ")
                );
                player.transform.position = savedPosition;
                Debug.Log($"Restored player to position: {savedPosition}");
            }
            
            // Enable player control
            PlayerManager pm = player.GetComponent<PlayerManager>();
            // if (pm != null)
            // {
            //     pm.isInCutscene = false;
            // }
        }

        // Disable cutscene camera, enable main camera
        if (cutsceneCamera != null)
        {
            cutsceneCamera.gameObject.SetActive(false);
        }
        
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
        }
        
        // Re-enable cursor lock for gameplay
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    void HandleNormalSceneStart()
    {
        // Your existing logic for first-time scene load
        // (cutscene logic, etc.)
        Debug.Log("Normal scene start");
    }
}
