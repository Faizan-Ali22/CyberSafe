using UnityEngine;

public class KeyboardGizmo : MonoBehaviour
{
#if UNITY_EDITOR
    [Range(0.1f, 0.9f)]
    public float keyboardHeightPercent = 0.5f; // 50% of screen = typical Android keyboard

    private void OnDrawGizmos()
    {
        RectTransform canvasRect = GetComponent<RectTransform>();
        if (canvasRect == null) return;

        float keyboardHeight = canvasRect.rect.height * keyboardHeightPercent;
        float canvasWidth    = canvasRect.rect.width;

        Gizmos.matrix = transform.localToWorldMatrix;

        // Filled blue box
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.4f);
        Gizmos.DrawCube(
            new Vector3(0, -(canvasRect.rect.height / 2f) + (keyboardHeight / 2f), 0),
            new Vector3(canvasWidth, keyboardHeight, 0)
        );

        // Cyan outline
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(
            new Vector3(0, -(canvasRect.rect.height / 2f) + (keyboardHeight / 2f), 0),
            new Vector3(canvasWidth, keyboardHeight, 0)
        );
    }
#endif
}