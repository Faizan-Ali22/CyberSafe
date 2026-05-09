using UnityEngine;
using System.Collections.Generic;

// Handles ALL touch and mouse input for drawing firewall lines
// Attach to: GameManager

public class DrawInputManager : MonoBehaviour
{
    // Singleton so RadarDrawer can easily fetch active strokes
    public static DrawInputManager Instance;

    [Header("References")]
    public RectTransform gameCanvasRect; // Assign GameCanvas (RawImage) here

    // Class to track active finger strokes
    private class TouchStroke
    {
        public Vector2 startPos;
        public Vector2 currentPos;
    }

    private Dictionary<int, TouchStroke> _activeStrokes = new Dictionary<int, TouchStroke>();

    // Mouse fallback tracking
    private bool    _mouseDown    = false;
    private Vector2 _mouseStart   = Vector2.zero;
    private Vector2 _mouseCurrent = Vector2.zero; 

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // Don't allow drawing if we aren't actively playing
        if (GameController.Instance == null || GameController.Instance.State != GameController.GameState.PLAYING) 
            return;

        HandleTouchInput();
        HandleMouseInput();
    }

    // ── Touch Input (Android multitouch) ─────────────────────────
    void HandleTouchInput()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            Vector2 canvasPos = ScreenToCanvas(t.position);

            switch (t.phase)
            {
                case TouchPhase.Began:
                    _activeStrokes[t.fingerId] = new TouchStroke { startPos = canvasPos, currentPos = canvasPos };
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (_activeStrokes.ContainsKey(t.fingerId))
                        _activeStrokes[t.fingerId].currentPos = canvasPos;
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (_activeStrokes.ContainsKey(t.fingerId))
                    {
                        FinishStroke(_activeStrokes[t.fingerId].startPos, _activeStrokes[t.fingerId].currentPos);
                        _activeStrokes.Remove(t.fingerId);
                    }
                    break;
            }
        }
    }

    // ── Mouse Input (Editor testing) ─────────────────────────────
    void HandleMouseInput()
    {
        // Only use mouse if no real touches exist to prevent duplicate inputs
        if (Input.touchCount > 0) return;

        Vector2 canvasPos = ScreenToCanvas(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            _mouseDown    = true;
            _mouseStart   = canvasPos;
            _mouseCurrent = canvasPos;
        }
        else if (Input.GetMouseButton(0) && _mouseDown)
        {
            // Update current position while dragging so the white line follows the cursor
            _mouseCurrent = canvasPos; 
        }
        else if (Input.GetMouseButtonUp(0) && _mouseDown)
        {
            _mouseDown = false;
            FinishStroke(_mouseStart, canvasPos);
        }
    }

    // ── Finish a stroke → send to FirewallManager ─────────────────
    void FinishStroke(Vector2 start, Vector2 end)
    {
        float dist = Vector2.Distance(start, end);

        // Minimum draw distance 30px (ignores accidental taps)
        if (dist < 30f) return;

        FirewallManager.Instance.DeployWall(start, end);
    }

    // ── Feed active strokes to the RadarDrawer ────────────────────
    public List<(Vector2 start, Vector2 current)> GetActiveStrokes()
    {
        var result = new List<(Vector2, Vector2)>();
        
        foreach (var kvp in _activeStrokes)
        {
            result.Add((kvp.Value.startPos, kvp.Value.currentPos));
        }

        if (_mouseDown)
        {
            result.Add((_mouseStart, _mouseCurrent));
        }

        return result;
    }

    // ── Bulletproof Coordinate Math ───────────────────────────────
    Vector2 ScreenToCanvas(Vector2 screenPos)
    {
        if (gameCanvasRect == null) return Vector2.zero;

        // 1. Convert Screen space to Local UI space. 
        // FORCE null for the camera parameter. If a camera is passed in while 
        // using Screen Space - Overlay, the math completely breaks.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gameCanvasRect,
            screenPos,
            null, 
            out Vector2 localPos
        );

        // 2. Ignore Pivots entirely. Calculate the exact percentage (0.0 to 1.0) 
        // of where the click happened across the Rect's width and height.
        Rect rect = gameCanvasRect.rect;
        float normalizedX = (localPos.x - rect.x) / rect.width;
        float normalizedY = (localPos.y - rect.y) / rect.height;

        // 3. Map that percentage directly to our 1920x1080 RenderTexture resolution.
        // Y is inverted because RenderTexture (0,0) is Top-Left, but UI (0,0) is Bottom-Left.
        float pixelX = normalizedX * 1920f;
        float pixelY = (1f - normalizedY) * 1080f;

        return new Vector2(pixelX, pixelY);
    }
}