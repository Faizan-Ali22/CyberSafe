using UnityEngine;
using System.Collections;
public class CinematicCameraController : MonoBehaviour
{
    public static CinematicCameraController Instance;

    [Header("Cameras")]
    public Camera mainCamera;
    public Camera cinematicCamera;

    [Header("Transition Settings")]
    public float defaultTransitionDuration = 1.5f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Teacher Focus Settings")]
    public Transform teacherFocusTarget;
    public float focusDistance = 2.5f;
    public float focusHeight = 1.6f;

    private Coroutine currentTransition;
    private Vector3 originalCinematicPos;
    private Quaternion originalCinematicRot;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (cinematicCamera != null)
        {
            originalCinematicPos = cinematicCamera.transform.position;
            originalCinematicRot = cinematicCamera.transform.rotation;
            cinematicCamera.gameObject.SetActive(false);
        }
    }

    public void SwitchToCinematicCamera(Transform target = null, float duration = -1f)
    {
        if (duration < 0) duration = defaultTransitionDuration;

        mainCamera.gameObject. SetActive(false);
        cinematicCamera.gameObject.SetActive(true);

        if (target != null)
        {
            if (currentTransition != null)
                StopCoroutine(currentTransition);
            currentTransition = StartCoroutine(LerpCameraToTarget(target, duration));
        }
    }

    public void SwitchToMainCamera(float duration = -1f)
    {
        if (duration < 0) duration = defaultTransitionDuration;

        if (currentTransition != null)
            StopCoroutine(currentTransition);

        StartCoroutine(TransitionToMainCamera(duration));
    }

    private IEnumerator LerpCameraToTarget(Transform target, float duration)
    {
        Vector3 startPos = cinematicCamera.transform. position;
        Quaternion startRot = cinematicCamera. transform.rotation;

        Vector3 endPos = target.position;
        Quaternion endRot = target.rotation;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = transitionCurve. Evaluate(elapsed / duration);

            cinematicCamera.transform. position = Vector3. Lerp(startPos, endPos, t);
            cinematicCamera.transform. rotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        cinematicCamera.transform. position = endPos;
        cinematicCamera.transform.rotation = endRot;
    }

    public void MoveToTarget(Transform target, float duration = -1f)
    {
        if (target == null) return;
        
        if (duration < 0) duration = defaultTransitionDuration;

        if (currentTransition != null)
            StopCoroutine(currentTransition);
        currentTransition = StartCoroutine(LerpCameraToTarget(target, duration));
    }

    private IEnumerator TransitionToMainCamera(float duration)
    {
        Vector3 startPos = cinematicCamera.transform. position;
        Quaternion startRot = cinematicCamera. transform.rotation;

        float elapsed = 0f;

        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = transitionCurve.Evaluate(elapsed / (duration * 0.5f));

            cinematicCamera.transform.position = Vector3.Lerp(startPos, originalCinematicPos, t);
            cinematicCamera.transform.rotation = Quaternion.Slerp(startRot, originalCinematicRot, t);

            yield return null;
        }

        cinematicCamera.gameObject.SetActive(false);
        mainCamera.gameObject. SetActive(true);
    }

    public void FocusOnTeacher(Transform teacher, float duration = -1f)
    {
        if (duration < 0) duration = defaultTransitionDuration;

        if (currentTransition != null)
            StopCoroutine(currentTransition);
        currentTransition = StartCoroutine(FocusOnTeacherRoutine(teacher, duration));
    }

    private IEnumerator FocusOnTeacherRoutine(Transform teacher, float duration)
    {
        Vector3 startPos = cinematicCamera.transform. position;
        Quaternion startRot = cinematicCamera. transform.rotation;

        // Calculate position in front of teacher
        Vector3 offset = teacher.forward * (-focusDistance) + Vector3.up * focusHeight;
        Vector3 endPos = teacher.position + offset;

        // Look at teacher's face
        Vector3 lookTarget = teacher.position + Vector3.up * focusHeight;
        Quaternion endRot = Quaternion.LookRotation(lookTarget - endPos);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = transitionCurve.Evaluate(elapsed / duration);

            cinematicCamera. transform.position = Vector3.Lerp(startPos, endPos, t);
            cinematicCamera. transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        cinematicCamera.transform. position = endPos;
        cinematicCamera.transform.rotation = endRot;
    }

    // Subtle camera movements during dialogue
    public void SubtleMove(Vector3 offset, float duration = 2f)
    {
        if (currentTransition != null)
            StopCoroutine(currentTransition);
        currentTransition = StartCoroutine(SubtleMoveRoutine(offset, duration));
    }

    private IEnumerator SubtleMoveRoutine(Vector3 offset, float duration)
    {
        Vector3 startPos = cinematicCamera. transform.position;
        Vector3 endPos = startPos + offset;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = transitionCurve. Evaluate(elapsed / duration);

            cinematicCamera.transform. position = Vector3. Lerp(startPos, endPos, t);

            yield return null;
        }
    }
}
