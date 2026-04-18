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

        // Keep teacher done state
        if (LabReturnState.IsTeacherDone())
        {
            var mgr = FindFirstObjectByType<MonitorScreenManager>();
            if (mgr) mgr.ApplyState();

            var teacher = FindFirstObjectByType<TeacherInteraction>();
            if (teacher) teacher.enabled = false;
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