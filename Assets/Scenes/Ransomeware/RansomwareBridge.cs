using UnityEngine;
using UnityEngine.SceneManagement;

public class RansomwareBridge : MonoBehaviour
{
    // React will call this function and pass 'true' for win, 'false' for loss
    public void EndMinigame(bool playerWon)
    {
        if (playerWon)
        {
            Debug.Log("BRIDGE TRIGGERED: Player Won! Returning to CyberSafe...");
            // Remove the // below and put your actual next scene name when ready
            // SceneManager.LoadScene("CyberSafe_MainLevel"); 
        }
        else
        {
            Debug.Log("BRIDGE TRIGGERED: Player Lost! System Compromised...");
            // Remove the // below and put your Game Over scene name when ready
            // SceneManager.LoadScene("CyberSafe_GameOver");
        }
    }
}