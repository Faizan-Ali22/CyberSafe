using UnityEngine;

[System.Serializable]
public class NpcDialogueEntry
{
    [Header("Who is speaking?")]
    public string speakerName;
    
    [TextArea(3, 10)] 
    public string dialogueText;

    [Header("Visuals")]
    public Animator speakerAnimator;      // Assign the student's animator here if they are talking
    public string animationTrigger = "";  // e.g., "Talk"
    
    [Header("Camera Control")]
    public Transform cameraTarget;        // Where the camera should look (CloseUp or Medium)
    public float cameraTransitionTime = 0.5f;
}
