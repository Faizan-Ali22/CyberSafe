using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Handles the gameplay loop, wave progression, and file interactions.
/// </summary>
public class CyberRansomwareManager : MonoBehaviour
{
    public float waveSpeed = 60f; // Pixels per second upwards
    public bool isPlaying = false;

    private Canvas mainCanvas;
    private RectTransform waveRect;
    private RectTransform driveRect;
    private TextMeshProUGUI progressText;
    
    private List<CyberFile> files = new List<CyberFile>();
    private int importantTotal = 8; // Based on the UI setup
    private int importantSaved = 0;

    public void Initialize(Canvas canvas, RectTransform wave, RectTransform drive, TextMeshProUGUI progText)
    {
        mainCanvas = canvas;
        waveRect = wave;
        driveRect = drive;
        progressText = progText;
        isPlaying = true;
    }

    public void RegisterFile(CyberFile file)
    {
        files.Add(file);
    }

    private void Update()
    {
        if (!isPlaying || waveRect == null) return;

        // Advance Wave Upwards
        Vector2 size = waveRect.sizeDelta;
        size.y += waveSpeed * Time.deltaTime;
        waveRect.sizeDelta = size;

        CheckWaveCollisions();

        // Check End Condition (Wave hits top of file window)
        if (waveRect.sizeDelta.y >= 900)
        {
            EndGame(false);
        }
    }

    private void CheckWaveCollisions()
    {
        // Get the world Y position of the top of the wave
        Vector3[] waveCorners = new Vector3[4];
        waveRect.GetWorldCorners(waveCorners);
        float waveTopY = waveCorners[1].y;

        foreach (var file in files)
        {
            if (file.isSafe || file.isEncrypted || file.isDragging) continue;

            // Get world Y position of the file row
            Vector3[] fileCorners = new Vector3[4];
            file.rect.GetWorldCorners(fileCorners);
            float fileCenterY = (fileCorners[0].y + fileCorners[1].y) / 2f;

            // If wave top overtakes the file's center
            if (waveTopY >= fileCenterY)
            {
                file.Encrypt();
            }
        }
    }

    public void HandleFileDrop(CyberFile file)
    {
        // Check overlap with Safe Drive using Screen coordinates
        if (RectTransformUtility.RectangleContainsScreenPoint(driveRect, file.rect.position, mainCanvas.worldCamera))
        {
            file.SaveFile();
            if (file.isImportant)
            {
                importantSaved++;
                progressText.text = $"FILES SECURED          <color=#00ffcc>{importantSaved} / {importantTotal}</color>";
            }
            
            if (importantSaved >= importantTotal)
            {
                EndGame(false);
            }
        }
        else
        {
            // If dropped outside, return to original parent/list
            file.ResetPosition();
        }
    }

    public void TriggerTrap()
    {
        if (!isPlaying) return;
        EndGame(true);
    }

    private void EndGame(bool scammed)
    {
        isPlaying = false;
        
        // Spawn End Screen Overlay
        GameObject endScreen = new GameObject("EndScreen");
        endScreen.transform.SetParent(mainCanvas.transform, false);
        Image img = endScreen.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.95f);
        
        RectTransform rect = endScreen.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        string title = scammed ? "<color=red>SYSTEM COMPROMISED</color>" : "<color=#00ffcc>WAVE COMPLETE</color>";
        string desc = scammed ? 
            "You paid the attackers. They took the money and deleted your files anyway.\n<size=20>Rule #1: Never pay the ransom.</size>" : 
            $"Important Files Secured: {importantSaved} / {importantTotal}\n<size=20>Great job prioritizing critical data.</size>";

        GameObject textObj = new GameObject("ResultText");
        textObj.transform.SetParent(endScreen.transform, false);
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = $"{title}\n\n{desc}";
        tmp.fontSize = 40;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.sizeDelta = new Vector2(1000, 400);
    }

    public Canvas GetMainCanvas() => mainCanvas;
}

/// <summary>
/// Component attached to each file row to handle dragging and states.
/// </summary>
public class CyberFile : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string fileName;
    public bool isImportant;
    public bool isSafe = false;
    public bool isEncrypted = false;
    public bool isDragging = false;
    
    public RectTransform rect;
    private Image bgImage;
    private TextMeshProUGUI statusText;
    private CyberRansomwareManager manager;
    
    private Transform originalParent;
    private Vector2 originalPosition;

    public void Setup(string name, bool important, Image bg, TextMeshProUGUI status, CyberRansomwareManager mgr)
    {
        fileName = name;
        isImportant = important;
        bgImage = bg;
        statusText = status;
        manager = mgr;
        rect = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isEncrypted || isSafe || !manager.isPlaying) return;
        
        isDragging = true;
        originalParent = transform.parent;
        originalPosition = rect.anchoredPosition;

        // Move to main canvas so it renders on top of everything while dragging
        transform.SetParent(manager.GetMainCanvas().transform);
        
        bgImage.color = new Color(0, 0.8f, 0.8f, 0.2f); // Cyan highlight
        statusText.text = "<color=#00ffcc>MOVING...</color>";
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isEncrypted || isSafe || !manager.isPlaying) return;
        
        // Drag logic scaled for canvas resolution
        rect.anchoredPosition += eventData.delta / manager.GetMainCanvas().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isEncrypted || isSafe || !manager.isPlaying) return;
        
        isDragging = false;
        manager.HandleFileDrop(this);
    }

    public void ResetPosition()
    {
        transform.SetParent(originalParent);
        rect.anchoredPosition = originalPosition;
        bgImage.color = new Color(1, 1, 1, 0); // Clear background
        statusText.text = "<color=#aaaaaa>AT RISK</color>";
    }

    public void SaveFile()
    {
        isSafe = true;
        gameObject.SetActive(false); // Hide the row once safely in the drive
    }

    public void Encrypt()
    {
        isEncrypted = true;
        bgImage.color = new Color(0.9f, 0.1f, 0.1f, 0.3f); // Red background tint
        statusText.text = "<color=red>ENCRYPTED</color>";
    }
}