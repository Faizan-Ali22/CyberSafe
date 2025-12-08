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

        // Keep teacher idle if already done
        if (LabReturnState.IsTeacherDone())
        {
            var mgr = FindFirstObjectByType<MonitorScreenManager>();
            if (mgr) mgr.ApplyState();

            var teacher = FindFirstObjectByType<TeacherInteraction>();
            if (teacher) teacher.enabled = false; // or set a flag if you prefer
        }
    }
}
