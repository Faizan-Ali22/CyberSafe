using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PopupData {
public float x, y;
public string text;
public int life;
public Color color;
}
public class RadarDrawer : MonoBehaviour
{
    [Header("References")]
public RenderTexture renderTexture;
public PacketSpawner packetSpawner;
public FirewallManager firewallManager;
 
private List<PopupData> popups = new List<PopupData>();
private Dictionary<int, Vector4> activeTouches = new Dictionary<int, Vector4>();
private int frameCount = 0;
 
// Called by DrawInputManager each Update
public void SetActiveTouches(Dictionary<int, Vector4> t) => activeTouches = t;
 
public void AddPopup(float x, float y, string text, Color col) {
popups.Add(new PopupData { x=x, y=y, text=text, life=90, color=col });
}
 
void Update() {
if (GameController.Instance.GameState != GameController.State.PLAYING) return;
frameCount++;
 
// Tick popup life
for (int i = popups.Count-1; i>=0; i--) {
popups[i].life--;
popups[i].y -= 0.5f;
if (popups[i].life <= 0) popups.RemoveAt(i);
}
 
DrawFrame();
}
 
void DrawFrame() {
// All drawing goes to the RenderTexture using GL
RenderTexture prev = RenderTexture.active;
RenderTexture.active = renderTexture;
GL.PushMatrix();
GL.LoadPixelMatrix(0, renderTexture.width, renderTexture.height, 0);
 
// Clear background: #050A15
GL.Clear(true, true, new Color(0.02f, 0.039f, 0.082f));
 
float w = renderTexture.width;
float h = renderTexture.height;
float cx = w / 2f;
float cy = h / 2f;
float load = GameController.Instance.Load;
 
DrawRadarGrid(cx, cy, w, h);
DrawActiveTouchLines();
DrawWalls();
DrawPackets();
DrawServer(cx, cy, load);
DrawPopups();
 
GL.PopMatrix();
RenderTexture.active = prev;
}
 
// All sub-draw methods use GL.Begin(GL.LINES) / GL.Begin(GL.TRIANGLES)
// See full implementation notes below each method stub
 
void DrawRadarGrid(float cx, float cy, float w, float h) {
// Color: rgba(72, 202, 228, 0.08) ← #48CAE4 at 8%
Color gridColor = new Color(0.282f, 0.792f, 0.894f, 0.08f);
// Draw concentric circles every 60px from 80px to max(w,h)
// Use 64-segment circle approximation for each ring
// Draw 6 radial lines from center at 60-degree intervals
GLDrawer.DrawCircles(cx, cy, gridColor, 80f, 60f, Mathf.Max(w,h));
GLDrawer.DrawRadialLines(cx, cy, gridColor, 6, Mathf.Max(w,h));
}
 
void DrawActiveTouchLines() {
// White dashed lines while finger is held
// Color: white (1,1,1,1)
// Line width: 4px — use 2 parallel offset lines for thickness
foreach (var kv in activeTouches) {
Vector4 t = kv.Value;
GLDrawer.DrawLine(t.x, t.y, t.z, t.w, Color.white, 4f, true, frameCount);
}
}
 
void DrawWalls() {
foreach (var w in firewallManager.Walls) {
float lifePct = w.life / (float)w.maxLife;
if (lifePct < 0.2f && frameCount % 10 < 5) continue; // flash
Color col = new Color(0.22f, 0.745f, 0.976f, lifePct); // #38BDF8
bool glitched = lifePct < 0.5f;
if (glitched) {
float midX = (w.x1+w.x2)/2f + Random.Range(-5f,5f);
float midY = (w.y1+w.y2)/2f + Random.Range(-5f,5f);
GLDrawer.DrawLine(w.x1,w.y1,midX,midY,col,6f,false,0);
GLDrawer.DrawLine(midX,midY,w.x2,w.y2,col,6f,false,0);
} else {
GLDrawer.DrawLine(w.x1,w.y1,w.x2,w.y2,col,6f,false,0);
}
}
}
 
void DrawPackets() {
foreach (var p in packetSpawner.Packets) {
if (p.isBot) {
// Red rotating diamond: #EF4444
Color botColor = new Color(0.937f,0.267f,0.267f);
GLDrawer.DrawDiamond(p.x, p.y, 6f, botColor, frameCount * 0.1f);
} else {
// Green arrow/chevron: #10B981
Color legitColor = new Color(0.063f,0.725f,0.506f);
GLDrawer.DrawArrow(p.x, p.y, 6f, legitColor, p.angle);
}
// Stuck arc progress ring
if (p.stuckFrames > 0) {
int maxStuck = p.isBot ? 20 : 120;
float pct = (float)p.stuckFrames / maxStuck;
GLDrawer.DrawArc(p.x, p.y, 10f, Color.white, 0f, pct * 360f);
}
}
}
 
void DrawServer(float cx, float cy, float load) {
// Outer ring: green normally, red when load > 80
Color serverColor = load > 80f
? new Color(0.937f,0.267f,0.267f) // #EF4444
: new Color(0.063f,0.725f,0.506f); // #10B981
GLDrawer.DrawCircle(cx, cy, 40f, new Color(0.008f,0.024f,0.09f)); // fill #020617
GLDrawer.DrawCircleOutline(cx, cy, 40f, serverColor, 4f);
GLDrawer.DrawCircle(cx, cy, 15f, new Color(0.118f,0.165f,0.235f)); // inner #1E293B
}
 
void DrawPopups() {
// Popup boxes drawn as colored rects with text
// In GL we approximate: draw a dark rect, then colored border, then text via Texture2D
// For simplicity use a separate Canvas-space TMP pool — see HUDManager
foreach (var popup in popups) {
float alpha = Mathf.Min(1f, popup.life / 20f);
Color bg = new Color(0.059f,0.09f,0.165f, 0.9f * alpha); // #0F172A
Color brd = new Color(popup.color.r, popup.color.g, popup.color.b, alpha);
GLDrawer.DrawRect(popup.x - 80f, popup.y - 10f, 160f, 20f, bg);
GLDrawer.DrawRectOutline(popup.x - 80f, popup.y - 10f, 160f, 20f, brd);
}
}
}

public static class GLDrawer
{
    public static void DrawCircles(float cx, float cy, Color col, float startR, float stepR, float maxR) {}
    public static void DrawRadialLines(float cx, float cy, Color col, int count, float radius) {}
    public static void DrawLine(float x1, float y1, float x2, float y2, Color col, float width, bool dashed, int frame) {}
    public static void DrawDiamond(float x, float y, float size, Color col, float rotation) {}
    public static void DrawArrow(float x, float y, float size, Color col, float angle) {}
    public static void DrawArc(float cx, float cy, float radius, Color col, float startAngle, float endAngle) {}
    public static void DrawCircle(float cx, float cy, float radius, Color col) {}
    public static void DrawCircleOutline(float cx, float cy, float radius, Color col, float width) {}
    public static void DrawRect(float x, float y, float w, float h, Color col) {}
    public static void DrawRectOutline(float x, float y, float w, float h, Color col) {}
}
