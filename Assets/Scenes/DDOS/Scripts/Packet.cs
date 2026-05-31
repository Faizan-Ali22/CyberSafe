using UnityEngine;
using UnityEngine.UI;

public class Packet : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 direction;
    private float speed;
    public bool isBot;
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

    // Returns position in RenderTexture pixel space (top-left origin)
    // This matches the coordinate space that DrawInputManager uses
    public Vector2 GetPhysicsPos()
    {
        Vector2 anchored = rectTransform.anchoredPosition;
        // anchoredPosition is UI space (bottom-left origin), flip Y to match RT space
        return new Vector2(anchored.x, 1080f - anchored.y);
    }

    public bool IsBot => isBot;

    public void DestroyAsBlocked()
    {
        Vector2 pos = GetPhysicsPos();
        ParticleManager.Instance.Burst(pos, ParticleManager.COL_RED, 12);
        Destroy(gameObject);
    }

    void Update()
    {
        if (GameController.Instance.State != GameController.GameState.PLAYING) return;

        Vector2 currentPos = rectTransform.anchoredPosition;
        Vector2 nextPos = currentPos + (direction * speed);

        // UI is Bottom-Left (Y goes up). Radar/Walls are Top-Left (Y goes down).
        Vector2 physicsPos = new Vector2(nextPos.x, 1080f - nextPos.y);

        // 1. Check Firewall Collisions
        if (FirewallManager.Instance.IsNearAnyWall(physicsPos, 30f))
        {
            if (isBot)
            {
                ParticleManager.Instance.Burst(physicsPos, ParticleManager.COL_RED, 8);
                Destroy(gameObject);
                return;
            }

            stuckFrames++;
            rectTransform.anchoredPosition += new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f));

            if (!isBot && stuckFrames > 300)
            {
                GameController.Instance.LegitTimedOut();
                ParticleManager.Instance.Burst(physicsPos, ParticleManager.COL_AMBER, 12);
                PopupManager.Instance.ShowPopup(physicsPos, "TIMEOUT (UX DROP)", true);
                Destroy(gameObject);
            }
        }
        else
        {
            rectTransform.anchoredPosition = nextPos;
            stuckFrames = 0;
        }

        // 2. Check Server Arrival
        Vector2 serverCenter = new Vector2(960f, 540f);
        if (Vector2.Distance(rectTransform.anchoredPosition, serverCenter) < serverRadius)
        {
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