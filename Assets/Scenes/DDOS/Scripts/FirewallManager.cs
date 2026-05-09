using UnityEngine;
using System.Collections.Generic;

public class FirewallManager : MonoBehaviour
{
    public static FirewallManager Instance;

    // A deployed firewall wall
    public class Wall
    {
        public Vector2 p1, p2;
        public int     life    = 600; // Lasts 7 seconds
        public int     maxLife = 600;
    }

    private List<Wall> _walls = new List<Wall>();

    private static readonly string[] RULES =
    {
        "iptables -A INPUT -j DROP",
        "Rule: BLOCK_UDP_FLOOD",
        "Rule: RATE_LIMIT_TCP",
        "Rule: ISOLATE_SUBNET",
        "Config: BLACKHOLE_IP"
    };

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        GameController.Instance.OnStateChanged += OnStateChanged;
    }

    void OnStateChanged(GameController.GameState state)
    {
        if (state == GameController.GameState.PLAYING)
            ClearAllWalls();
    }

    void Update()
    {
        if (GameController.Instance.State != GameController.GameState.PLAYING) return;
        TickWalls();
    }

    public void DeployWall(Vector2 p1, Vector2 p2)
    {
        float dist = Vector2.Distance(p1, p2);

        // Ignore swipes shorter than 30 pixels
        if (dist < 30f) return;

        var wall = new Wall { p1 = p1, p2 = p2 };
        _walls.Add(wall);

        Vector2 mid = (p1 + p2) * 0.5f;
        ParticleManager.Instance.Burst(mid, ParticleManager.COL_CYAN, 15);

        string rule = RULES[Random.Range(0, RULES.Length)];
        PopupManager.Instance.ShowPopup(mid, rule);

        RadarDrawer.Instance.RegisterWall(wall);
    }

    void TickWalls()
    {
        for (int i = _walls.Count - 1; i >= 0; i--)
        {
            _walls[i].life--;

            if (_walls[i].life <= 0)
            {
                RadarDrawer.Instance.UnregisterWall(_walls[i]);
                _walls.RemoveAt(i);
            }
        }
    }

    public bool IsNearAnyWall(Vector2 point, float hitRadius = 30f)
    {
        foreach (var wall in _walls)
        {
            if (DistanceToSegment(point, wall.p1, wall.p2) < hitRadius)
                return true;
        }
        return false;
    }

    float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float   t  = Vector2.Dot(p - a, ab) / Vector2.Dot(ab, ab);
        t = Mathf.Clamp01(t);
        return Vector2.Distance(p, a + t * ab);
    }

    public void ClearAllWalls()
    {
        foreach (var wall in _walls)
            RadarDrawer.Instance.UnregisterWall(wall);

        _walls.Clear();
    }

    void OnDestroy()
    {
        if (GameController.Instance != null)
            GameController.Instance.OnStateChanged -= OnStateChanged;
    }
}