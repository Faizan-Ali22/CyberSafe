using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour
{
    [Header("Waypoint Status")]
    public Waypoint previousWaypoint;
    public Waypoint nextWaypoint;

    [Range(0f, 10f)]
    public float waypointWidth = 0f;

    public Vector3 GetPosition()
    {
        Vector3 minBound = transform.position + (transform.right * waypointWidth / 2f);
        Vector3 maxBound = transform.position - (transform.right * waypointWidth / 2f);
        return Vector3.Lerp(minBound, maxBound, Random.Range(0f, 1f)); 
    }

    
}
