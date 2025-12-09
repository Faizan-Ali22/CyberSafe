using UnityEngine;

public class ForceEnableGuardian : MonoBehaviour
{
     [Header("What should stay ON?")]
    // Drag your Player or Input GameObject here
    public GameObject objectToKeepActive;
    
    // OR Drag the specific Script (like PlayerInput) here
    public MonoBehaviour scriptToKeepEnabled;

    // We use LateUpdate to override anyone else who tried to disable it this frame
    void LateUpdate()
    {
        // 1. Force the Game Object to stay Active
        if (objectToKeepActive != null && !objectToKeepActive.activeSelf)
        {
            objectToKeepActive.SetActive(true);
            Debug.LogWarning("Guardian: I forced " + objectToKeepActive.name + " back ON!");
        }

        // 2. Force the Script Component to stay Enabled
        if (scriptToKeepEnabled != null && !scriptToKeepEnabled.enabled)
        {
            scriptToKeepEnabled.enabled = true;
            Debug.LogWarning("Guardian: I forced " + scriptToKeepEnabled.GetType().Name + " back ENABLED!");
        }
    }
}
