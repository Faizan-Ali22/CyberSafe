using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class RansomwareEndScreen : MonoBehaviour
{
   [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;
    public Button actionButton;
    public Text actionButtonText;

    [Header("Scene Transition")]
    public string nextLevelSceneName = "NULL";
    public int nextLevelBuildIndex = 20;

    private void OnEnable() 
    {
        // When this panel turns on, configure it based on the game results
        ConfigureEndScreen();
    }

    public void ConfigureEndScreen()
    {
        GameManager gm = GameManager.Instance;
        bool isWin = gm.HasPlayerWon();

        // 1. Remove any previous button clicks to prevent bugs
        actionButton.onClick.RemoveAllListeners();

        if (gm.State == GameManager.GameState.GameOverTrap)
        {
            // FAILED BY TRAP
            titleText.text = "SYSTEM COMPROMISED";
            titleText.color = Color.red;
            subtitleText.text = "You paid the ransom. The scammers took your money anyway.";
            
            actionButtonText.text = "REBOOT SYSTEM & TRY AGAIN";
            actionButton.onClick.AddListener(RestartGame);
        }
        else if (isWin)
        {
            // WON THE GAME
            titleText.text = "THREAT MITIGATED";
            titleText.color = Color.green;
            subtitleText.text = "You successfully secured the critical data and survived the ransomware tide!";
            
            actionButtonText.text = "CLAIM BADGE & CONTINUE";
            actionButton.onClick.AddListener(ClaimBadgeAndContinue);
        }
        else
        {
            // FAILED BY WAVE (Didn't save enough files)
            titleText.text = "ENCRYPTION COMPLETE";
            titleText.color = Color.red;
            subtitleText.text = "Too much critical data was lost. Your system cannot recover.";
            
            actionButtonText.text = "REBOOT SYSTEM & TRY AGAIN";
            actionButton.onClick.AddListener(RestartGame);
        }
    }

    private void RestartGame()
    {
        // Hide the end screen and restart the GameManager loop
        gameObject.SetActive(false);
        GameManager.Instance.StartGame();
    }

    private void ClaimBadgeAndContinue()
    {
    PlayerPrefs.SetInt("Badge_DeadDrop", 1);
    PlayerPrefs.Save();
    Debug.Log("Badge Awarded! Proceeding to next level...");

    if (nextLevelBuildIndex >= 0)
    {
        SceneManager.LoadScene(nextLevelBuildIndex);
    }
    else if (!string.IsNullOrEmpty(nextLevelSceneName) && nextLevelSceneName != "NULL")
    {
        SceneManager.LoadScene(nextLevelSceneName);
    }
    else
    {
        Debug.LogError("Next level not set. Set nextLevelBuildIndex or nextLevelSceneName in the inspector.");
    }
    }
}
