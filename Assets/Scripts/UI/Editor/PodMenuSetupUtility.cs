using UnityEngine;
using UnityEngine.UI;
using CyberSafe.UI;

#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// Editor utility to automatically set up the Pod Menu Animation system
/// Menu: GameObject > UI > Setup Pod Menu Animation
/// </summary>
public class PodMenuSetupUtility : MonoBehaviour
{
    [MenuItem("GameObject/UI/Setup Pod Menu Animation", false, 0)]
    public static void CreatePodMenuSetup()
    {
        // Ensure we have a Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            Debug.Log("✅ Created Canvas");
        }

        // Create main pod menu structure
        GameObject podMenuRoot = new GameObject("PodMenuRoot");
        podMenuRoot.transform.SetParent(canvas.transform, false);
        
        RectTransform rootRect = podMenuRoot.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.sizeDelta = Vector2.zero;

        // Add PodMenuAnimator
        PodMenuAnimator animator = podMenuRoot.AddComponent<PodMenuAnimator>();
        
        // Add CanvasGroup to root for initial hiding
        CanvasGroup rootCanvasGroup = podMenuRoot.AddComponent<CanvasGroup>();

        // Create Background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(podMenuRoot.transform, false);
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.15f, 1f); // Dark blue-gray
        
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Create Left Pod
        GameObject leftPod = new GameObject("LeftPod");
        leftPod.transform.SetParent(podMenuRoot.transform, false);
        Image leftImage = leftPod.AddComponent<Image>();
        leftImage.color = new Color(0.5f, 0.2f, 0.8f, 1f); // Purple
        
        RectTransform leftRect = leftPod.GetComponent<RectTransform>();
        leftRect.anchorMin = new Vector2(0f, 0.5f);
        leftRect.anchorMax = new Vector2(0f, 0.5f);
        leftRect.pivot = new Vector2(1f, 0.5f); // Right edge pivot
        leftRect.anchoredPosition = new Vector2(0f, 0f);
        leftRect.sizeDelta = new Vector2(400f, 800f);

        // Create Right Pod
        GameObject rightPod = new GameObject("RightPod");
        rightPod.transform.SetParent(podMenuRoot.transform, false);
        Image rightImage = rightPod.AddComponent<Image>();
        rightImage.color = new Color(0.5f, 0.2f, 0.8f, 1f); // Purple
        
        RectTransform rightRect = rightPod.GetComponent<RectTransform>();
        rightRect.anchorMin = new Vector2(1f, 0.5f);
        rightRect.anchorMax = new Vector2(1f, 0.5f);
        rightRect.pivot = new Vector2(0f, 0.5f); // Left edge pivot
        rightRect.anchoredPosition = new Vector2(0f, 0f);
        rightRect.sizeDelta = new Vector2(400f, 800f);

        // Create Menu Content Container
        GameObject menuContent = new GameObject("MenuContent");
        menuContent.transform.SetParent(podMenuRoot.transform, false);
        CanvasGroup contentGroup = menuContent.AddComponent<CanvasGroup>();
        contentGroup.alpha = 0f;
        contentGroup.interactable = false;
        contentGroup.blocksRaycasts = false;
        
        RectTransform contentRect = menuContent.GetComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.sizeDelta = Vector2.zero;

        // Create sample title text
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(menuContent.transform, false);
        TMPro.TextMeshProUGUI titleText = titleObj.AddComponent<TMPro.TextMeshProUGUI>();
        titleText.text = "CYBERSAFE MENU";
        titleText.fontSize = 48;
        titleText.alignment = TMPro.TextAlignmentOptions.Center;
        titleText.color = Color.cyan;
        
        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.8f);
        titleRect.anchorMax = new Vector2(0.5f, 0.8f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.sizeDelta = new Vector2(600f, 100f);
        titleRect.anchoredPosition = Vector2.zero;

        // Create sample button
        GameObject buttonObj = new GameObject("StartButton");
        buttonObj.transform.SetParent(menuContent.transform, false);
        Button button = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.5f, 0.8f, 1f);
        
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.sizeDelta = new Vector2(200f, 60f);
        buttonRect.anchoredPosition = Vector2.zero;

        GameObject buttonTextObj = new GameObject("Text");
        buttonTextObj.transform.SetParent(buttonObj.transform, false);
        TMPro.TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TMPro.TextMeshProUGUI>();
        buttonText.text = "START GAME";
        buttonText.fontSize = 24;
        buttonText.alignment = TMPro.TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        
        RectTransform buttonTextRect = buttonText.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.sizeDelta = Vector2.zero;
        buttonTextRect.anchoredPosition = Vector2.zero;

        // Wire up the animator
        animator.leftPod = leftRect;
        animator.rightPod = rightRect;
        animator.menuContent = contentGroup;
        animator.backgroundImage = bgImage;

        // Select the created object
        Selection.activeGameObject = podMenuRoot;

        Debug.Log("✅ Pod Menu Animation setup complete!");
        Debug.Log("📝 Next steps:");
        Debug.Log("1. Replace pod placeholder colors with your LeftPod Menu.png and RightPod Menu.png sprites");
        Debug.Log("2. Replace background with Empty Menu.png or Main Menu.png");
        Debug.Log("3. Add your menu buttons and UI elements to MenuContent");
        Debug.Log("4. Adjust pod positions and sizes to match your design");
        Debug.Log("5. Test by entering Play mode!");
    }
}
#endif
