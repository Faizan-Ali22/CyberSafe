using UnityEngine;
using UnityEditor;
using System.Collections;

public class WaypointManager : EditorWindow
{
    [MenuItem("Waypoint/Waypoints Editor Tool")]
    public static void ShowWindow()
    {
        GetWindow<WaypointManager>("Waypoints Editor Tool");
    }

    public Transform waypointOrigin;
    void OnGUI()
    {
        SerializedObject obj = new SerializedObject(this);
        EditorGUILayout.PropertyField(obj.FindProperty("waypointOrigin"));

        if (waypointOrigin == null)
        {
            EditorGUILayout.HelpBox("Please assign a waypoint origin object.", MessageType.Warning);

        }
        else
        {
            EditorGUILayout.BeginVertical("box");
            CreateButtons();
            EditorGUILayout.EndVertical();
        }
        obj.ApplyModifiedProperties();
    }

    void CreateButtons()
    {
        if (GUILayout.Button("Create Waypoint"))
        {
            CreateWaypoint();
        }
    }
    void CreateWaypoint()
    {
        //overall this method creates a new Waypoint game object sets its parent retrieve its Waypoint component
        //and connect it to the wavepoint sequence if there is a previous wavepoint this process allow for dynamic creation and management of Waypoint in game environment
        GameObject waypointObject = new GameObject("Waypoint " + waypointOrigin.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointOrigin, false);
        Waypoint waypoint = waypointObject.GetComponent<Waypoint>();

        if (waypointOrigin.childCount > 1)
        {
            waypoint.previousWaypoint = waypointOrigin.GetChild(waypointOrigin.childCount - 2).GetComponent<Waypoint>();
            waypoint.previousWaypoint.nextWaypoint = waypoint;

            waypoint.transform.position = waypoint.previousWaypoint.transform.position;
            waypoint.transform.forward = waypoint.previousWaypoint.transform.forward;

        }
        Selection.activeGameObject = waypoint.gameObject;
    }
}