using UnityEngine;
using UnityEngine.UI;

public class UIElementBounce : MonoBehaviour
{
   private RectTransform rectTransform;
    private Vector2 startPosition;
    public float speed = 5f;
    public float height = 20f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        // Simple vertical bounce
        rectTransform.anchoredPosition = new Vector2(startPosition.x, startPosition.y + Mathf.Sin(Time.time * speed) * height);
    }
}
