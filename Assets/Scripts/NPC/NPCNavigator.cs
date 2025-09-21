using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NPCNavigator : MonoBehaviour
{
    [Header("NPC Info")]
    public float movingSpeed= 1.2f;
    public float turningSpeed = 300f;
    public float stopspeed = 1f;

    [Header("Destination Vars")]
    public Vector3 destination;
    public bool destinationReached;
    public Animator animator;
    private float waypointTimeout = 30f;
    public float currentTimeout = 0f;

    void Update()
    {
       Walk();
    }
    public void Walk()
    {
        if (transform.position != destination)
        {
            Vector3 direction = destination - transform.position;
            direction.y = 0;
            float distance = direction.magnitude;
            if (distance >= stopspeed)
            {
                destinationReached = false;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turningSpeed * Time.deltaTime);
                transform.Translate(Vector3.forward * movingSpeed * Time.deltaTime);
            }
            else
            {
                destinationReached = true;
            }
        }

    }
    public void LocateDestination(Vector3 destination)
    {
        this.destination = destination;
        destinationReached = false;
        currentTimeout = 0f;
    }
}