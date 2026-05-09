using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Handles all particle burst effects on the radar
// Attach to: GameManager

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;

    // All burst colors used in the game
    public static readonly Color COL_CYAN  = new Color(0.220f, 0.741f, 0.973f); // #38BDF8
    public static readonly Color COL_RED   = new Color(0.937f, 0.267f, 0.267f); // #EF4444
    public static readonly Color COL_GREEN = new Color(0.063f, 0.725f, 0.506f); // #10B981
    public static readonly Color COL_AMBER = new Color(0.961f, 0.620f, 0.043f); // #F59E0B

    // Internal particle data (drawn by RadarDrawer)
    public class Particle
    {
        public Vector2 pos;
        public Vector2 vel;
        public Color   col;
        public float   life = 1f; // 1.0 → 0.0
    }

    // Public list so RadarDrawer can read and draw them
    public List<Particle> ActiveParticles = new List<Particle>();

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
            ClearAll();
    }

    void Update()
    {
        if (ActiveParticles.Count == 0) return;

        // Tick all particles
        for (int i = ActiveParticles.Count - 1; i >= 0; i--)
        {
            var p = ActiveParticles[i];

            p.life -= 0.05f;              // decreases over ~20 frames
            p.pos  += p.vel;              // move outward
            p.vel  *= 0.92f;              // slow down over time

            if (p.life <= 0f)
                ActiveParticles.RemoveAt(i);
        }
    }

    // ── Spawn a burst of particles ────────────────────────────────
    // pos   = RenderTexture pixel coordinates
    // col   = burst color
    // count = number of particles
    public void Burst(Vector2 pos, Color col, int count)
    {
        for (int i = 0; i < count; i++)
        {
            ActiveParticles.Add(new Particle
            {
                pos  = pos,
                vel  = new Vector2(
                    (Random.value - 0.5f) * 6f,
                    (Random.value - 0.5f) * 6f
                ),
                col  = col,
                life = 1f
            });
        }
    }

    // ── Clear all on game reset ───────────────────────────────────
    public void ClearAll()
    {
        ActiveParticles.Clear();
    }

    void OnDestroy()
    {
        if (GameController.Instance != null)
            GameController.Instance.OnStateChanged -= OnStateChanged;
    }
}