using UnityEngine;
using System.Collections;
public class StudentCinematicManager : MonoBehaviour
{
     public static StudentCinematicManager Instance;

    public Camera mainCamera;
    public Camera cinematicCamera; // Assign your extra camera here

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (cinematicCamera != null) cinematicCamera.gameObject.SetActive(false);
    }

    public void SwitchToCinematicCamera(Transform target, float duration)
    {
        if (mainCamera != null) mainCamera.gameObject.SetActive(false);
        if (cinematicCamera != null)
        {
            cinematicCamera.gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(MoveCamera(target, duration));
        }
    }

    public void SwitchToMainCamera(float duration)
    {
        // Simple delay before switching back
        StartCoroutine(SwitchBackRoutine(duration));
    }

    private IEnumerator SwitchBackRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (cinematicCamera != null) cinematicCamera.gameObject.SetActive(false);
        if (mainCamera != null) mainCamera.gameObject.SetActive(true);
    }

    private IEnumerator MoveCamera(Transform target, float duration)
    {
        Transform camTrans = cinematicCamera.transform;
        Vector3 startPos = camTrans.position;
        Quaternion startRot = camTrans.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Smooth easing
            t = t * t * (3f - 2f * t);

            camTrans.position = Vector3.Lerp(startPos, target.position, t);
            camTrans.rotation = Quaternion.Slerp(startRot, target.rotation, t);
            yield return null;
        }
        camTrans.position = target.position;
        camTrans.rotation = target.rotation;
    }
}
