using UnityEngine;

public class ResetAllProgress : MonoBehaviour
{
     private void Start()
    {
        SessionProgress.Reset();    // clears hacked screens
        LabReturnState.Reset();     // clears saved pose + selected screen + teacher flag
        // If you have it: ProgressManager.Instance?.ResetAllProgress();

        Debug.Log("All progress reset. You can now remove this component.");
    }
}
