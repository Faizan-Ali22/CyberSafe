using UnityEngine;
using UnityEngine.UI;

public class Packet : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 direction;
    private float speed;
    private bool isBot;
    private int stuckFrames = 0;

    private const float serverRadius = 40f;

    public void Init(Vector2 startPos, Vector2 dir, float spd, bool bot, float angle)
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = startPos;
        
        direction = dir;
        speed = spd;
        isBot = bot;

        rectTransform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
    }

    void Update()
    {
        if (GameController.Instance.State != GameController.GameState.PLAYING) return;

        Vector2 currentPos = rectTransform.anchoredPosition;
        Vector2 nextPos = currentPos + (direction * speed);

        // --- THE MIRROR FIX ---
        // UI is Bottom-Left (Y goes up). Radar/Walls are Top-Left (Y goes down).
        // We MUST flip the Y axis so the packet's hitbox perfectly matches the drawn wall!
        Vector2 physicsPos = new Vector2(nextPos.x, 1080f - nextPos.y);

        // 1. Check Firewall Collisions using the synced Physics coordinates
        if (FirewallManager.Instance.IsNearAnyWall(physicsPos, 30f))
        {
            stuckFrames++;
            
            // Jitter visually in UI space
            rectTransform.anchoredPosition += new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f));

            if (isBot && stuckFrames > 20)
            {
                ParticleManager.Instance.Burst(physicsPos, ParticleManager.COL_RED, 8);
                Destroy(gameObject);
            }
            else if (!isBot && stuckFrames > 300)
            {
                GameController.Instance.LegitTimedOut();
                ParticleManager.Instance.Burst(physicsPos, ParticleManager.COL_AMBER, 12);
                PopupManager.Instance.ShowPopup(physicsPos, "TIMEOUT (UX DROP)", true);
                Destroy(gameObject);
            }
        }
        else
        {
            // Move forward visually
            rectTransform.anchoredPosition = nextPos;
            stuckFrames = 0;
        }

        // 2. Check Server Arrival
        // Center is 960, 540 in both coordinate systems, so distance checks work directly
        Vector2 serverCenter = new Vector2(960f, 540f); 
        if (Vector2.Distance(rectTransform.anchoredPosition, serverCenter) < serverRadius)
        {
            // Sync explosion coordinates to the radar layer so they don't spawn mirrored
            Vector2 currentPhysicsPos = new Vector2(rectTransform.anchoredPosition.x, 1080f - rectTransform.anchoredPosition.y);
            
            if (isBot)
            {
                GameController.Instance.BotReachedServer();
                ParticleManager.Instance.Burst(currentPhysicsPos, ParticleManager.COL_RED, 15);
            }
            else
            {
                GameController.Instance.LegitReachedServer();
                ParticleManager.Instance.Burst(currentPhysicsPos, ParticleManager.COL_GREEN, 8);
            }
            Destroy(gameObject);
        }
    }
}