using UnityEngine;
using System.Collections.Generic;

// Draws everything onto the RenderTexture each frame:
// radar grid, walls, packets, particles, and active touch strokes
// Attach to: GameManager

public class RadarDrawer : MonoBehaviour
{
    public static RadarDrawer Instance;

    [Header("References")]
    public RenderTexture renderTexture; // assign RadarRenderTex
    public Camera        radarCamera;   // assign RadarCamera (or leave empty if unused)

    // Internal tracked walls from FirewallManager
    private List<FirewallManager.Wall> _walls = new List<FirewallManager.Wall>();

    // Cached center of radar
    private Vector2 _center;

    // Frame counter for flash effects
    private int _frame = 0;

    // Colors
    static readonly Color COL_BG          = new Color(0.020f, 0.039f, 0.082f); // #050A15
    static readonly Color COL_GRID        = new Color(0.282f, 0.792f, 0.894f, 0.08f); // #48CAE4 8%
    static readonly Color COL_SERVER_GREEN= new Color(0.063f, 0.725f, 0.506f); // #10B981
    static readonly Color COL_SERVER_RED  = new Color(0.937f, 0.267f, 0.267f); // #EF4444
    static readonly Color COL_INNER_CORE  = new Color(0.118f, 0.161f, 0.231f); // #1E293B
    static readonly Color COL_WALL_CYAN   = new Color(0.220f, 0.741f, 0.973f); // #38BDF8

    // The material required by Unity's GL system
    private Material glMaterial;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        _center = new Vector2(
            renderTexture.width  * 0.5f,
            renderTexture.height * 0.5f
        );
    }

    void Update()
    {
        _frame++;
        DrawFrame();
    }

    // ── GL Material Setup ─────────────────────────────────────────
    void CreateGLMaterial()
    {
        if (!glMaterial)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            glMaterial = new Material(shader);
            glMaterial.hideFlags = HideFlags.HideAndDontSave;
            
            // Turn on Alpha Blending for transparent colors
            glMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            glMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            
            // Turn off culling and depth so our UI renders flat and perfectly on top
            glMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            glMaterial.SetInt("_ZWrite", 0);
            glMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
        }
    }

    // ── Main draw call every frame ────────────────────────────────
    void DrawFrame()
    {
        // Set render target
        RenderTexture.active = renderTexture;

        // ENSURE MATERIAL EXISTS AND IS ACTIVE
        CreateGLMaterial();
        glMaterial.SetPass(0);

        GL.PushMatrix();
        GL.LoadPixelMatrix(0, renderTexture.width, renderTexture.height, 0);

        // Clear background
        GL.Clear(true, true, COL_BG);

        DrawRadarGrid();
        DrawActiveTouchLines(); 
        DrawWalls();
        DrawServerCenter();

        GL.PopMatrix();
        RenderTexture.active = null;
    }

    // ── Preview Lines for Drawing ─────────────────────────────────
    void DrawActiveTouchLines()
{
    if (DrawInputManager.Instance == null) return;
    var strokes = DrawInputManager.Instance.GetActiveStrokes();
    
    foreach (var stroke in strokes)
    {
        // Glow layer (wide, transparent)
        Color glow = new Color(0.220f, 0.741f, 0.973f, 0.3f); // cyan glow
        DrawLine(stroke.start, stroke.current, glow, 18f);
        
        // Core slash line (sharp, bright)
        Color core = new Color(0.220f, 0.741f, 0.973f, 1f);
        DrawLine(stroke.start, stroke.current, core, 4f);
    }
}

    // ── Radar grid (concentric circles + lane lines) ──────────────
    void DrawRadarGrid()
    {
        float maxR = Mathf.Max(renderTexture.width, renderTexture.height);

        // Concentric circles every 60px
        for (float r = 80f; r < maxR; r += 60f)
        {
            DrawCircle(_center, r, COL_GRID, 64);
        }

        // 6 lane lines from center outward
        int lanes = 6;
        for (int i = 0; i < lanes; i++)
        {
            float angle = i * (Mathf.PI * 2f / lanes);
            Vector2 end = _center + new Vector2(
                Mathf.Cos(angle) * maxR,
                Mathf.Sin(angle) * maxR
            );
            DrawLine(_center, end, COL_GRID, 1f);
        }
    }

    // ── Server center circle ──────────────────────────────────────
    void DrawServerCenter()
    {
        float load = GameController.Instance != null ? GameController.Instance.Load : 0f;
        Color serverColor = load > 80f ? COL_SERVER_RED : COL_SERVER_GREEN;

        // Outer glow ring (larger, semi-transparent)
        Color glowColor = new Color(serverColor.r, serverColor.g, serverColor.b,
                                    load > 80f ? 0.4f : 0.15f);
        DrawFilledCircle(_center, 50f, glowColor, 48);

        // Dark fill
        DrawFilledCircle(_center, 40f, COL_BG, 48);

        // Server border ring
        DrawCircle(_center, 40f, serverColor, 48);

        // Inner core
        DrawFilledCircle(_center, 15f, COL_INNER_CORE, 32);
    }

    // ── Walls ─────────────────────────────────────────────────────
    void DrawWalls()
    {
        foreach (var wall in _walls)
        {
            float pct = (float)wall.life / wall.maxLife;

            // Flash when expiring (< 20% life)
            if (pct < 0.2f && _frame % 10 < 5) continue;

            Color wallColor = new Color(
                COL_WALL_CYAN.r,
                COL_WALL_CYAN.g,
                COL_WALL_CYAN.b,
                pct // fade as life drains
            );

            // Crumble effect at < 50% life
            if (pct < 0.5f)
            {
                Vector2 mid = (wall.p1 + wall.p2) * 0.5f + new Vector2(
                    (Random.value - 0.5f) * 10f,
                    (Random.value - 0.5f) * 10f
                );
                DrawLine(wall.p1, mid,    wallColor, 6f);
                DrawLine(mid,    wall.p2, wallColor, 6f);
            }
            else
            {
                DrawLine(wall.p1, wall.p2, wallColor, 6f);
            }

            // Glow behind wall (wider, more transparent)
            Color glowColor = new Color(
                COL_WALL_CYAN.r,
                COL_WALL_CYAN.g,
                COL_WALL_CYAN.b,
                pct * 0.3f
            );
            DrawLine(wall.p1, wall.p2, glowColor, 14f);
        }
    }

    // ── Wall registration (called by FirewallManager) ─────────────
    public void RegisterWall(FirewallManager.Wall wall)
    {
        if (!_walls.Contains(wall))
            _walls.Add(wall);
    }

    public void UnregisterWall(FirewallManager.Wall wall)
    {
        _walls.Remove(wall);
    }

    // ── GL Drawing Helpers ────────────────────────────────────────

    // Draw a line using GL
    void DrawLine(Vector2 a, Vector2 b, Color col, float width)
    {
        // Calculate perpendicular for width
        Vector2 dir  = (b - a).normalized;
        Vector2 perp = new Vector2(-dir.y, dir.x) * (width * 0.5f);

        Vector2 a1 = a + perp;
        Vector2 a2 = a - perp;
        Vector2 b1 = b + perp;
        Vector2 b2 = b - perp;

        DrawQuad(a1, a2, b2, b1, col);
    }

    // Draw a hollow circle (outline only)
    void DrawCircle(Vector2 center, float radius, Color col, int segments)
    {
        float step = Mathf.PI * 2f / segments;
        for (int i = 0; i < segments; i++)
        {
            float a1 = i * step;
            float a2 = (i + 1) * step;
            Vector2 p1 = center + new Vector2(Mathf.Cos(a1), Mathf.Sin(a1)) * radius;
            Vector2 p2 = center + new Vector2(Mathf.Cos(a2), Mathf.Sin(a2)) * radius;
            DrawLine(p1, p2, col, 1f);
        }
    }

    // Draw a filled circle
    void DrawFilledCircle(Vector2 center, float radius, Color col, int segments)
    {
        float step = Mathf.PI * 2f / segments;

        GL.Begin(GL.TRIANGLES);
        GL.Color(col);

        for (int i = 0; i < segments; i++)
        {
            float a1 = i * step;
            float a2 = (i + 1) * step;

            GL.Vertex3(center.x, center.y, 0);
            GL.Vertex3(
                center.x + Mathf.Cos(a1) * radius,
                center.y + Mathf.Sin(a1) * radius, 0);
            GL.Vertex3(
                center.x + Mathf.Cos(a2) * radius,
                center.y + Mathf.Sin(a2) * radius, 0);
        }

        GL.End();
    }

    // Draw a filled quad (used by DrawLine)
    void DrawQuad(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Color col)
    {
        GL.Begin(GL.TRIANGLES);
        GL.Color(col);

        GL.Vertex3(a.x, a.y, 0);
        GL.Vertex3(b.x, b.y, 0);
        GL.Vertex3(c.x, c.y, 0);

        GL.Vertex3(a.x, a.y, 0);
        GL.Vertex3(c.x, c.y, 0);
        GL.Vertex3(d.x, d.y, 0);

        GL.End();
    }

    void OnDestroy()
    {
        RenderTexture.active = null;
    }
}