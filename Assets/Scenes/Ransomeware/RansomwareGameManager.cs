using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Handles the gameplay loop, wave progression, and file interactions.
/// </summary>
public class RansomwareGameManager : MonoBehaviour
{
    public float waveSpeed = 150f; // Pixels per second
    public bool isPlaying = false;

    private RectTransform waveRect;
    private RectTransform driveRect;
    private List<DraggableFile> files = new List<DraggableFile>();
    private RansomwareUISetup uiSetup;

    private int importantTotal = 0;
    private int importantSaved = 0;

    public void Initialize(RansomwareUISetup setup, RectTransform wave, RectTransform drive)
    {
        uiSetup = setup;
        waveRect = wave;
        driveRect = drive;
        isPlaying = true;
    }

    public void RegisterFile(DraggableFile file)
    {
        files.Add(file);
        if (file.isImportant) importantTotal++;
    }

    private void Update()
    {
        if (!isPlaying || waveRect == null) return;

        // Advance Wave
        Vector2 size = waveRect.sizeDelta;
        size.x += waveSpeed * Time.deltaTime;
        waveRect.sizeDelta = size;

        CheckWaveCollisions();

        // Check Win/Loss Condition (Wave engulfs screen)
        if (waveRect.sizeDelta.x >= Screen.width)
        {
            EndGame(false);
        }

        // Randomly spawn traps
        if (Random.Range(0f, 100f) < 0.1f) // Roughly 1 popup every few seconds at 60fps
        {
            uiSetup.SpawnPopupTrap();
        }
    }

    private void CheckWaveCollisions()
    {
        float waveRightEdge = waveRect.sizeDelta.x;

        foreach (var file in files)
        {
            if (file.isSafe || file.isEncrypted) continue;

            // If the wave passes the file's anchored position (adjusted for center pivot)
            if (waveRightEdge >= file.rect.anchoredPosition.x + (Screen.width / 2))
            {
                file.Encrypt();
            }
        }
    }

    public void EndGame(bool scammed)
    {
        isPlaying = false;
        uiSetup.ShowEndScreen(scammed, importantSaved, importantTotal);
    }

    public void HandleFileDrop(DraggableFile file)
    {
        // Simple AABB collision check with the drive panel
        if (RectTransformUtility.RectangleContainsScreenPoint(driveRect, file.rect.position, null))
        {
            file.isSafe = true;
            file.canvasGroup.alpha = 0f; // Hide it (it's "in" the drive)
            if (file.isImportant) importantSaved++;
        }
    }
}

/// <summary>
/// Component attached programmatically to each generated file UI element.
/// </summary>
public class DraggableFile : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isImportant;
    public bool isSafe = false;
    public bool isEncrypted = false;
    public RectTransform rect;
    public CanvasGroup canvasGroup;
    
    private RansomwareGameManager manager;
    private Image bgImage;
    private TextMeshProUGUI textMesh;

    public void Setup(bool important, RansomwareGameManager mgr)
    {
        isImportant = important;
        manager = mgr;
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        bgImage = GetComponent<Image>();
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isEncrypted || isSafe || !manager.isPlaying) return;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.7f;
        transform.SetAsLastSibling(); // Bring to front
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isEncrypted || isSafe || !manager.isPlaying) return;
        rect.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isEncrypted || isSafe || !manager.isPlaying) return;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        manager.HandleFileDrop(this);
    }

    public void Encrypt()
    {
        isEncrypted = true;
        bgImage.color = new Color(0.8f, 0.1f, 0.1f); // Turn Red
        textMesh.text = "LOCKED";
        textMesh.color = Color.white;
    }
}