using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCPatrol : MonoBehaviour
{
    //     public List<Transform> waypoints = new List<Transform>();
    //     public bool isMoving;
    //     public int waypointIndex;
    //     public float moveSpeed;
    //     public float rotationSpeed;
    //     public bool isLoop;
    //     public bool isRandom;

    //     void Start()
    //     {
    //         StartMoving();
    //     }

    //     public void StartMoving()
    //     {
    //         waypointIndex = 0;
    //         isMoving = true;
    //     }

    //     void Update()
    //     {
    //         if (!isMoving)
    //         {
    //             return;
    //         }

    //         if (waypointIndex < waypoints.Count)
    //         {
    //             // Move towards the waypoint
    //         transform.position = Vector3.MoveTowards(transform.position,waypoints[waypointIndex].position, moveSpeed * Time.deltaTime);

    //             // 🔥 FIXED: direction should be waypoint - current position (not the other way around)
    //             var direction = waypoints[waypointIndex].position - transform.position;

    //             if (direction != Vector3.zero) // Prevents LookRotation errors
    //             {
    //                 var targetRotation = Quaternion.LookRotation(direction, Vector3.up);
    //                 transform.rotation = Quaternion.Lerp(
    //                     transform.rotation,
    //                     targetRotation,
    //                     Time.deltaTime * rotationSpeed
    //                 );
    //             }

    //             // Check distance to switch waypoint
    //             var distance = Vector3.Distance(transform.position, waypoints[waypointIndex].position);
    //             if (distance < 0.05f)
    //             {
    //                 if (isRandom)
    //                 {
    //                     waypointIndex = Random.Range(0, waypoints.Count);
    //                 }
    //                 else
    //                 {
    //                     waypointIndex++;
    //                     if (isLoop && waypointIndex >= waypoints.Count)
    //                     {
    //                         waypointIndex = 0;
    //                     }
    //                 }
    //             }
    //         }
    //     }
  public List<Transform> waypoints = new List<Transform>();
    public bool isMoving;
    public int waypointIndex;
    public float moveSpeed = 0.06f;
    public float rotationSpeed = 15f;
    public bool isLoop;
    public bool isRandom;

    void Start()
    {
        StartMoving();
    }

    public void StartMoving()
    {
        waypointIndex = 0;
        isMoving = true;
    }

    void Update()
    {
        if (!isMoving || waypoints.Count == 0)
        {
            return;
        }

        if (waypointIndex < waypoints.Count)
        {
            // Move towards the waypoint
            transform.position = Vector3.MoveTowards(
                transform.position,
                waypoints[waypointIndex].position,
                moveSpeed * Time.deltaTime
            );

            // Calculate direction
            var direction = waypoints[waypointIndex].position - transform.position;

            // 🔥 New: Only rotate if far enough from waypoint
            var distance = Vector3.Distance(transform.position, waypoints[waypointIndex].position);
            if (direction != Vector3.zero && distance > 0.1f) // prevent jitter at close range
            {
                var targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime * rotationSpeed
                );
            }

            // Switch waypoint when close enough
            if (distance < 0.05f)
            {
                if (isRandom)
                {
                    waypointIndex = Random.Range(0, waypoints.Count);
                }
                else
                {
                    waypointIndex++;
                    if (isLoop && waypointIndex >= waypoints.Count)
                    {
                        waypointIndex = 0;
                    }
                }
            }
        }
    }
}
