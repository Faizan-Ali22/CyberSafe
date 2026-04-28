using UnityEngine;
using System.Collections;   

[RequireComponent(typeof(Collider))]
public class VirusDamageDealer : MonoBehaviour
{
    public float damageAmount = 5.0f;
    
    [Header("Camera Shake Settings")]
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MazeGameManager.Instance?.DamagePlayer(damageAmount);
            
            // Trigger the camera shake safely
            if (MazeCameraShake.Instance != null)
            {
                MazeCameraShake.Instance.ShakeCamera(shakeDuration, shakeMagnitude);
            }
        }
    }

    // Change this inside VirusDamageDealer.cs if your Virus is solid!
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            MazeGameManager.Instance?.DamagePlayer(damageAmount);
            
            // Trigger the camera shake safely
            if (MazeCameraShake.Instance != null)
            {
                MazeCameraShake.Instance.ShakeCamera(shakeDuration, shakeMagnitude);
            }
        }
    }
}
