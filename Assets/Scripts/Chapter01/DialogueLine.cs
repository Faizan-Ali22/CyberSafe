using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DialogueLine : MonoBehaviour
{
    public string speakerName;
    [TextArea(2, 5)]
    public string dialogue;
    public AudioClip voiceClip;
    public Animator speakerAnimator;
    public string animationTrigger = "Talk";
    
    [Header("Camera Settings")]
    public Transform cameraTarget; // Optional: specific camera position for this line
    public float cameraTransitionTime = 1f;
}
