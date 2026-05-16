using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class AvatarButtonWirer : MonoBehaviour
{
    [SerializeField] private int avatarIndex = 0;
    
    // Tier 1: Inspector reference
    [SerializeField] private PasswordStrengthChecker checker; 

    private void Start()
    {
        StartCoroutine(SetupButton());
    }

    private IEnumerator SetupButton()
    {
        Button myButton = GetComponent<Button>();
        myButton.onClick.RemoveAllListeners(); // Safe clean

        PasswordStrengthChecker activeChecker = null;

        // Wait for the checker to be available (handles timing issues on mobile)
        while (activeChecker == null)
        {
            // Tier 1: Use the Inspector assignment if it exists.
            activeChecker = checker;

            // Tier 2: Try the Global Singleton from Memory.
            if (activeChecker == null) 
            {
                activeChecker = PasswordStrengthChecker.Instance;
            }

            // Tier 3: BRUTE FORCE. If Android wiped the memory or the object is inactive, force a scene sweep.
            if (activeChecker == null)
            {
                activeChecker = FindFirstObjectByType<PasswordStrengthChecker>(FindObjectsInactive.Include);
            }

            if (activeChecker == null)
            {
                yield return null; // Wait one frame and try again
            }
        }

        // Now that we have the checker, add the listener
        myButton.onClick.AddListener(() => OnAvatarClicked(activeChecker));
    }

    private void OnAvatarClicked(PasswordStrengthChecker activeChecker)
    {
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