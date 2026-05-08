using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Packet {
public int id;
public float x, y;
public float vx, vy;
public float angle;
public bool isBot;
public float speed;
public int stuckFrames;
public int lane;
}
public class PacketSpawner : MonoBehaviour
{
    public static PacketSpawner Instance;
public List<Packet> Packets { get; private set; } = new List<Packet>();
 
    
[Header("References")]
public FirewallManager firewallManager;
public DdosParticleManager particleManager;
public RadarDrawer radarDrawer;
 
private readonly int lanesCount = 6;
private float width, height;
private int frame = 0;
private int nextId = 0;
 
void Awake() => Instance = this;
 
public void Init(float w, float h) {
width = w; height = h;
Packets.Clear();
frame = 0;
}
 
public void Tick() {
if (GameController.Instance.GameState != GameController.State.PLAYING) return;
frame++;
 
int wave = GameController.Instance.Wave;
float elapsed = GameController.Instance.totalTime - GameController.Instance.TimeRemaining;
// Legit users — steady stream, speeds up over time
int legitInterval = Mathf.Max(40, 60 - (int)(elapsed / 2f));
if (frame % legitInterval == 0)
SpawnPacket(Random.Range(0, lanesCount), false);
 
// Attack waves
if (wave == 1 && frame % 120 == 0)
SpawnBurst(Random.Range(0, lanesCount), 6);
else if (wave == 2 && frame % 80 == 0) {
int lane = Random.Range(0, lanesCount);
SpawnBurst(lane, 4);
StartCoroutine(DelayedSpawn(lane, false, 0.2f));
} else if (wave == 3) {
if (frame % 30 == 0)
SpawnPacket(Random.Range(0, lanesCount), Random.value > 0.3f);
if (frame % 90 == 0)
SpawnBurst(Random.Range(0, lanesCount), 6);
}
 
UpdatePackets();
}
 
void SpawnPacket(int lane, bool isBot) {
float cx = width / 2f;
float cy = height / 2f;
float spawnRadius = Mathf.Max(width, height) * 0.6f;
float laneStep = (Mathf.PI * 2f) / lanesCount;
float angle = lane * laneStep + Random.Range(laneStep * 0.1f, laneStep * 0.9f);
float startX = cx + Mathf.Cos(angle) * spawnRadius;
float startY = cy + Mathf.Sin(angle) * spawnRadius;
float vx = -Mathf.Cos(angle);
float vy = -Mathf.Sin(angle);
float speed = isBot ? 1.2f + Random.value * 0.5f : 0.7f;
 
Packets.Add(new Packet {
id = nextId++, lane = lane,
x = startX, y = startY,
vx = vx, vy = vy,
angle = angle + Mathf.PI,
isBot = isBot, speed = speed,
stuckFrames = 0
});
}
void SpawnBurst(int lane, int count) {
for (int i = 0; i < count; i++) {
int idx = i; // capture
StartCoroutine(DelayedSpawn(lane, true, i * 0.2f));
}
}
 
IEnumerator DelayedSpawn(int lane, bool isBot, float delay) {
yield return new WaitForSeconds(delay);
if (GameController.Instance.GameState == GameController.State.PLAYING)
SpawnPacket(lane, isBot);
}
 
void UpdatePackets() {
float cx = width / 2f;
float cy = height / 2f;
float serverR = 40f;
 
for (int i = Packets.Count - 1; i >= 0; i--) {
Packet p = Packets[i];
float nx = p.x + p.vx * p.speed;
float ny = p.y + p.vy * p.speed;
 
bool hitWall = firewallManager.IsBlockedByWall(nx, ny);
 
if (hitWall) {
p.stuckFrames++;
p.x += Random.Range(-1.5f, 1.5f);
p.y += Random.Range(-1.5f, 1.5f);
 
if (p.isBot && p.stuckFrames > 20) {
particleManager.Spawn(p.x, p.y, new Color(0.94f,0.27f,0.27f), 8);
Packets.RemoveAt(i); continue;
} else if (!p.isBot && p.stuckFrames > 120) {
GameController.Instance.AddFrustration(10f);
particleManager.Spawn(p.x, p.y, new Color(0.96f,0.62f,0.04f), 12);
radarDrawer.AddPopup(p.x, p.y - 10f, "TIMEOUT (UX DROP)",
new Color(0.96f,0.62f,0.04f));
Packets.RemoveAt(i); continue;
}
} else {
p.x = nx; p.y = ny; p.stuckFrames = 0;
}
 
// Server arrival
if (Mathf.Sqrt((p.x-cx)*(p.x-cx)+(p.y-cy)*(p.y-cy)) < serverR) {
if (p.isBot) {
GameController.Instance.AddLoad(12f);
particleManager.Spawn(p.x, p.y, new Color(0.94f,0.27f,0.27f), 15);
} else {
GameController.Instance.ReduceLoad(2f);
particleManager.Spawn(p.x, p.y, new Color(0.063f,0.725f,0.506f), 8);
}
Packets.RemoveAt(i);
}
}
}
}
