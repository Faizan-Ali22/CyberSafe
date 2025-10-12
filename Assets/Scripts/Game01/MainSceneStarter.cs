using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainSceneStarter : MonoBehaviour
{
    [Header("Cameras")]
    public Animator introCameraAnimator;   // Animator for IntroCam (not auto-playing)
    public Camera introCamera;             // Intro cam object
    public Camera cutsceneCamera;          // cutscene cam
    public Camera mainCamera;              // gameplay cam

    [Header("Timing")]
    public float introCameraAnimLength = 8f; // duration in seconds (same as animation)
    public float postAnimDelay = 0.4f;

    [Header("Cutscene")]
    public CutsceneController cutsceneController;

    IEnumerator Start()
    {
        // ensure cameras are set
        if (introCamera) introCamera.gameObject.SetActive(true);
        if (cutsceneCamera) cutsceneCamera.gameObject.SetActive(false);
        if (mainCamera) mainCamera.gameObject.SetActive(false);

        // ensure animator is reset (so it doesn't auto-play)
        if (introCameraAnimator) {
            introCameraAnimator.Rebind();
            introCameraAnimator.Update(0f);
            introCameraAnimator.ResetTrigger("Play");
            yield return null; // one frame to apply reset
            introCameraAnimator.SetTrigger("Play");
        }

        // wait for animation to finish
        yield return new WaitForSeconds(introCameraAnimLength + postAnimDelay);

        // switch to cutscene camera
        if (introCamera) introCamera.gameObject.SetActive(false);
        if (cutsceneCamera) cutsceneCamera.gameObject.SetActive(true);

        // slight delay for blends
        yield return new WaitForSeconds(0.2f);

        // begin cutscene
        if (cutsceneController) {
            yield return StartCoroutine(cutsceneController.PlayCutsceneSequence());
        }

        // after cutscene, CutsceneController enables main camera and player control
    }
}
