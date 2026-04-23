using UnityEngine;

public class LabPlayerRestorer : MonoBehaviour
{
    [SerializeField] private Transform playerTransform; // assign player root

    private void Start()
    {
        if (playerTransform && LabReturnState.HasSavedPose)
        {
            LabReturnState.RestorePlayerPose(playerTransform);
        }

        // Keep teacher done state and sync the UI properly
        if (LabReturnState.IsTeacherDone())
        {
            // Find teacher (even if inactive)
            var teacher = Object.FindFirstObjectByType<TeacherInteraction>(FindObjectsInactive.Include);
            if (teacher)
            {
                // Force Task 1 OFF, Task 2 UI ON, interact icons OFF
                teacher.ApplyPostInteractionState();
                teacher.enabled = false;
            }

            // Apply hacked screen statuses out in the Lab
            var mgr = Object.FindFirstObjectByType<MonitorScreenManager>(FindObjectsInactive.Include);
            if (mgr) mgr.ApplyState();
        }

        // Find Task2Controller EVEN IF IT IS DISABLED in the inspector
        var task2 = Object.FindFirstObjectByType<Task2Controller>(FindObjectsInactive.Include);
        if (task2 != null)
        {
            // Note: If the teacher is done, task2 GameObject should be made active by teacher.ApplyPostInteractionState(). 
            // We just ask it to refresh its numbers here.
            task2.RefreshTask2UI();
            task2.TryCompleteTask2();
        }
    }
}