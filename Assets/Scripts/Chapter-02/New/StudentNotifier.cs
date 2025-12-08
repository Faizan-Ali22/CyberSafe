using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(AudioSource))]
public class StudentNotifier : MonoBehaviour
{
   [Header("Settings")]
    public string playerTag = "Player";
    public float pingInterval = 2.0f; // How often the sound plays (in seconds)

    [Header("References")]
    public GameObject interactCanvas; // The "Tap" button over the head

    private AudioSource audioSource;
    private bool isPlayerInside = false;
    private float timer;
    private bool interactionActive = false; // To stop sound after interaction starts

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Ensure collider is a trigger
        GetComponent<BoxCollider>().isTrigger = true;
        
        // Ensure UI is hidden at start
        if (interactCanvas != null) interactCanvas.SetActive(false);
    }

    private void Update()
    {
        // Don't play sounds if we are already interacting
        if (interactionActive || !isPlayerInside) return;

        timer += Time.deltaTime;
        if (timer >= pingInterval)
        {
            PlayNotificationSound();
            timer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInside = true;
            timer = pingInterval; // Play immediately upon entering
            
            if (interactCanvas != null && !interactionActive) 
                interactCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInside = false;
            
            if (interactCanvas != null) 
                interactCanvas.SetActive(false);
        }
    }

    private void PlayNotificationSound()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    // Called by the Interaction script to stop the noise
    public void StopNotifications()
    {
        interactionActive = true;
        if (interactCanvas != null) interactCanvas.SetActive(false);
        if (audioSource != null) audioSource.Stop();
    }
}
