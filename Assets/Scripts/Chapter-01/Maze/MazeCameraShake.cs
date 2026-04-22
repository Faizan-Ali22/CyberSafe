using UnityEngine;
using System.Collections;

public class MazeCameraShake : MonoBehaviour
{
    public static MazeCameraShake Instance;
    
    // Store the original local position so we don't permanently offset the camera
    private Vector3 originalLocalPos;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        originalLocalPos = transform.localPosition;
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Calculate a random offset
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // Apply the offset to the camera's local position
            transform.localPosition = new Vector3(originalLocalPos.x + x, originalLocalPos.y + y, originalLocalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset to the original position once done
        transform.localPosition = originalLocalPos;
    }
}
