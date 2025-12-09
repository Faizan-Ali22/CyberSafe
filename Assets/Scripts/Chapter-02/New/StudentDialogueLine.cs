using UnityEngine;
using System.Collections;
 [System.Serializable] 
public class StudentDialogueLine
{
   
  public string speakerName;
    [TextArea(3, 10)] 
    public string dialogue; // Write your text here in the Inspector
    
    [Header("Visuals")]
    public Animator speakerAnimator;      // Drag the Student's Animator here if they are talking
    public string animationTrigger;       // e.g. "Talk"
    
    [Header("Camera")]
    public Transform cameraTarget;        // Drag the CloseUp or MediumShot transform here
    public float cameraTransitionTime = 1.0f;
}
