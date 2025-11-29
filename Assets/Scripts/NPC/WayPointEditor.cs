using UnityEngine;
using UnityEditor;

[InitializeOnLoad()]
public class WayPointEditor
{
    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmos(Waypoint waypoint, GizmoType gizmoType)
    {
        if ((gizmoType & GizmoType.Selected) != 0)
        {
            Gizmos.color = Color.blue;

        }
        else
        {
            Gizmos.color = Color.cyan * 0.8f;
        }
        Gizmos.DrawWireSphere(waypoint.transform.position, 0.5f);

        Gizmos.color = Color.black;

        Gizmos.DrawLine(
            waypoint.transform.position + (waypoint.transform.right * waypoint.waypointWidth / 2f),
            waypoint.transform.position - (waypoint.transform.right * waypoint.waypointWidth / 2f)
        );

        if (waypoint.previousWaypoint != null)
        {
            Gizmos.color = Color.red;
            Vector3 offset = waypoint.transform.right * waypoint.waypointWidth / 2f;
            Vector3 offsetTo = waypoint.previousWaypoint.transform.right * waypoint.previousWaypoint.waypointWidth / 2f;

            Gizmos.DrawLine(waypoint.transform.position + offset, waypoint.previousWaypoint.transform.position + offsetTo);
        }
        if (waypoint.nextWaypoint != null)
        {
            Gizmos.color = Color.green;
            Vector3 offset = waypoint.transform.right * -waypoint.waypointWidth / 2f;
            Vector3 offsetTo = waypoint.previousWaypoint.transform.right * -waypoint.previousWaypoint.waypointWidth / 2f;
            
            Gizmos.DrawLine(waypoint.transform.position + offset, waypoint.previousWaypoint.transform.position + offsetTo);
        }

    }
}
