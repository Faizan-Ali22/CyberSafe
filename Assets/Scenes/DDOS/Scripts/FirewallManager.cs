using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FirewallWall {
public float x1, y1, x2, y2;
public int life, maxLife;
}
public class FirewallManager : MonoBehaviour
{
    public static FirewallManager Instance;
public List<FirewallWall> Walls { get; private set; } = new List<FirewallWall>();
 
[Header("References")]
public RadarDrawer radarDrawer;
public DdosParticleManager particleManager;
 
private static readonly string[] RULE_TEXTS = {
"iptables -A INPUT -j DROP",
"Rule: BLOCK_UDP_FLOOD",
"Rule: RATE_LIMIT_TCP",
"Rule: ISOLATE_SUBNET",
"Config: BLACKHOLE_IP"
};

void Awake() => Instance = this;
 
public void Reset() => Walls.Clear();
 
public void DeployWall(float x1, float y1, float x2, float y2) {
Walls.Add(new FirewallWall { x1=x1,y1=y1,x2=x2,y2=y2, life=300, maxLife=300 });
string rule = RULE_TEXTS[Random.Range(0, RULE_TEXTS.Length)];
radarDrawer.AddPopup((x1+x2)/2f, Mathf.Min(y1,y2)-20f, rule,
new Color(0.22f, 0.745f, 0.976f)); // #38BDF8 cyan
particleManager.Spawn((x1+x2)/2f,(y1+y2)/2f,
new Color(0.22f,0.745f,0.976f), 15);
}
 
public void Tick() {
for (int i = Walls.Count - 1; i >= 0; i--) {
Walls[i].life--;
if (Walls[i].life <= 0) Walls.RemoveAt(i);
}
}
 
// Distance from point to line segment squared
public bool IsBlockedByWall(float px, float py) {
foreach (var w in Walls) {
float l2 = (w.x2-w.x1)*(w.x2-w.x1) + (w.y2-w.y1)*(w.y2-w.y1);
if (l2 == 0f) {
float d2 = (px-w.x1)*(px-w.x1)+(py-w.y1)*(py-w.y1);
if (d2 < 100f) return true; continue;
}
float t = Mathf.Clamp01(((px-w.x1)*(w.x2-w.x1)+(py-w.y1)*(w.y2-w.y1))/l2);
float closestX = w.x1 + t*(w.x2-w.x1);
float closestY = w.y1 + t*(w.y2-w.y1);
float dist2 = (px-closestX)*(px-closestX)+(py-closestY)*(py-closestY);
if (dist2 < 100f) return true; // within 10px of wall
}
return false;
}
}