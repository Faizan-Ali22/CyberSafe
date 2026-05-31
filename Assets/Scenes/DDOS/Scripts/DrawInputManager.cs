using UnityEngine;
using System.Collections.Generic;

// Handles ALL touch and mouse input for drawing firewall lines
// Attach to: GameManager

public class DrawInputManager : MonoBehaviour
{
    public static DrawInputManager Instance;

    [Header("References")]
    public RectTransform gameCanvasRect;

    private class TouchStroke
    {
        public Vector2 startPos;
        public Vector2 currentPos;
        public Vector2 prevPos;
    }

    private Dictionary<int, TouchStroke> _activeStrokes = new Dictionary<int, TouchStroke>();

    private bool    _mouseDown    = false;
    private Vector2 _mouseStart   = Vector2.zero;
    private Vector2 _mouseCurrent = Vector2.zero;
    private Vector2 _mousePrev    = Vector2.zero;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (GameController.Instance == null || GameController.Instance.State != GameController.GameState.PLAYING)
            return;

        HandleTouchInput();
        HandleMouseInput();
    }

    // ── Touch Input ───────────────────────────────────────────────
    void HandleTouchInput()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            Vector2 canvasPos = ScreenToCanvas(t.position);

            switch (t.phase)
            {
                case TouchPhase.Began:
                    _activeStrokes[t.fingerId] = new TouchStroke
                    {
                        startPos   = canvasPos,
                        currentPos = canvasPos,
                        prevPos    = canvasPos
                    };
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (_activeStrokes.ContainsKey(t.fingerId))
                    {
                        Vector2 prev = _activeStrokes[t.fingerId].currentPos;
                        _activeStrokes[t.fingerId].prevPos    = prev;
                        _activeStrokes[t.fingerId].currentPos = canvasPos;
                        CheckSwipeKills(prev, canvasPos);
                    }
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
        if (Input.touchCount > 0) return;

        Vector2 canvasPos = ScreenToCanvas(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            _mouseDown    = true;
            _mouseStart   = canvasPos;
            _mouseCurrent = canvasPos;
            _mousePrev    = canvasPos;
        }
        else if (Input.GetMouseButton(0) && _mouseDown)
        {
            _mousePrev    = _mouseCurrent;
            _mouseCurrent = canvasPos;
            CheckSwipeKills(_mousePrev, _mouseCurrent);
        }
        else if (Input.GetMouseButtonUp(0) && _mouseDown)
        {
            _mouseDown = false;
            FinishStroke(_mouseStart, canvasPos);
        }
    }

    // ── Swipe Kill Detection ──────────────────────────────────────
    // Both prevRT and currentRT are already in RenderTexture pixel space (top-left origin)
    // GetPhysicsPos() on Packet also returns RT pixel space
    // So NO coordinate conversion needed — they are already the same space
    void CheckSwipeKills(Vector2 prevRT, Vector2 currentRT)
    {
        if (PacketSpawner.Instance == null) return;

        var packets = PacketSpawner.Instance.GetActivePackets();
        if (packets == null) return;

        for (int i = packets.Count - 1; i >= 0; i--)
        {
            Packet packet = packets[i];
            if (packet == null || !packet.IsBot) continue;

            Vector2 packetPos = packet.GetPhysicsPos();

            if (DistanceToSegment(packetPos, prevRT, currentRT) < 40f)
            {
                packet.DestroyAsBlocked();
                packets.RemoveAt(i);
            }
        }
    }

    float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float t = Vector2.Dot(p - a, ab) / Mathf.Max(Vector2.Dot(ab, ab), 0.0001f);
        t = Mathf.Clamp01(t);
        return Vector2.Distance(p, a + t * ab);
    }

    // ── Finish a stroke → send to FirewallManager ─────────────────
    void FinishStroke(Vector2 start, Vector2 end)
    {
        float dist = Vector2.Distance(start, end);
        if (dist < 30f) return;
        FirewallManager.Instance.DeployWall(start, end);
    }

    // ── Feed active strokes to RadarDrawer ────────────────────────
    public List<(Vector2 start, Vector2 current)> GetActiveStrokes()
    {
        var result = new List<(Vector2, Vector2)>();

        foreach (var kvp in _activeStrokes)
            result.Add((kvp.Value.startPos, kvp.Value.currentPos));

        if (_mouseDown)
            result.Add((_mouseStart, _mouseCurrent));

        return result;
    }

    // ── Coordinate Conversion ─────────────────────────────────────
    // Converts screen pixel position → RenderTexture pixel space (top-left origin, 1920x1080)
    Vector2 ScreenToCanvas(Vector2 screenPos)
    {
        if (gameCanvasRect == null) return Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gameCanvasRect,
            screenPos,
            null,
            out Vector2 localPos
        );

        Rect rect = gameCanvasRect.rect;
        float normalizedX = (localPos.x - rect.x) / rect.width;
        float normalizedY = (localPos.y - rect.y) / rect.height;

        float pixelX = normalizedX * 1920f;
        float pixelY = (1f - normalizedY) * 1080f;

        return new Vector2(pixelX, pixelY);
    }
}