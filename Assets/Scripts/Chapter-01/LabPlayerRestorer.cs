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
            var teacher = FindFirstObjectByType<TeacherInteraction>();
            if (teacher)
            {
                // Force the Task 1 vs Task 2 UI to match the "post-conversation" state
                teacher.ApplyPostInteractionState();
                teacher.enabled = false;
            }

            // Important: Apply hacked screen statuses AFTER the teacher script turns their parent objects ON
            var mgr = FindFirstObjectByType<MonitorScreenManager>();
            if (mgr) mgr.ApplyState();
        }

        // Sync Task2 UI and completion check after returning from Game01
        var task2 = FindFirstObjectByType<Task2Controller>();
        if (task2 != null)
        {
            task2.RefreshTask2UI();
            task2.TryCompleteTask2();
        }
    }
}