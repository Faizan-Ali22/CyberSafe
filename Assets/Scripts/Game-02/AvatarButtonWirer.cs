using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AvatarButtonWirer : MonoBehaviour
{
    [SerializeField] private int avatarIndex = 0;
    
    // Tier 1: Inspector reference
    [SerializeField] private PasswordStrengthChecker checker; 

    private void Start()
    {
        Button myButton = GetComponent<Button>();
        myButton.onClick.RemoveAllListeners(); // Safe clean
        myButton.onClick.AddListener(OnAvatarClicked);
    }

    private void OnAvatarClicked()
    {
        // --- THE 190 IQ 3-TIER FAILSAFE ---

        // Tier 1: Use the Inspector assignment if it exists.
        PasswordStrengthChecker activeChecker = checker;

        // Tier 2: Try the Global Singleton from Memory.
        if (activeChecker == null) 
        {
            activeChecker = PasswordStrengthChecker.Instance;
        }

        // Tier 3: BRUTE FORCE. If Android wiped the memory or the object is inactive, force a scene sweep.
        if (activeChecker == null)
        {
            Debug.LogWarning($"[AvatarButtonWirer] Singleton lost! Brute-forcing scene search for Avatar {avatarIndex}...");
            activeChecker = FindFirstObjectByType<PasswordStrengthChecker>(FindObjectsInactive.Include);
        }

        // --- EXECUTION ---

        // If it is STILL null, the script literally does not exist in your scene hierarchy anymore.
        if (activeChecker == null)
        {
            Debug.LogError($"[AvatarButtonWirer] FATAL: PasswordStrengthChecker does not exist anywhere in this scene! Did you accidentally delete it?");
            return;
        }

        if (activeChecker.IsPasswordSet(avatarIndex)) 
        {
            Debug.Log($"[AvatarButtonWirer] Avatar {avatarIndex} is already locked.");
            return; 
        }

        Debug.Log($"[AvatarButtonWirer] Connection Successful! Opening password panel for avatar {avatarIndex}.");
        activeChecker.SelectAccount(avatarIndex);
    }
}