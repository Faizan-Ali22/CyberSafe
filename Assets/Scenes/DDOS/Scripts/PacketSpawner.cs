using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacketSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject botPrefab;
    public GameObject legitPrefab;

    [Header("References")]
    public Transform packetParent; 

    const int LANES = 6;

    float _centerX;
    float _centerY;
    float _spawnRadius;

    int _frame = 0;
    List<Packet> _activePackets = new List<Packet>();

    void Start()
    {
        _centerX     = 1920f * 0.5f;
        _centerY     = 1080f * 0.5f;
        _spawnRadius = Mathf.Max(1920f, 1080f) * 0.6f;

        GameController.Instance.OnStateChanged += OnStateChanged;
    }

    void OnStateChanged(GameController.GameState state)
    {
        StopAllCoroutines();

        if (state == GameController.GameState.PLAYING)
        {
            ClearPackets();
            _frame = 0;
            StartCoroutine(SpawnLoop());
        }
        else
        {
            ClearPackets();
        }
    }

    void ClearPackets()
    {
        foreach (var p in _activePackets)
            if (p != null) Destroy(p.gameObject);

        _activePackets.Clear();
    }

    IEnumerator SpawnLoop()
    {
        while (GameController.Instance.State == GameController.GameState.PLAYING)
        {
            yield return null;
            _frame++;

            int   wave    = GameController.Instance.Wave;
            float elapsed = GameController.Instance.totalTime
                          - GameController.Instance.TimeLeft;

            int legitInterval = Mathf.Max(40, 60 - Mathf.FloorToInt(elapsed / 2f));
            if (_frame % legitInterval == 0)
                SpawnPacket(Random.Range(0, LANES), false);

            // Wave 1 — Slow, single bots to teach the mechanic. No bursts yet.
            if (wave == 1 && _frame % 90 == 0) 
            {
                SpawnPacket(Random.Range(0, LANES), true);
            }

            // Wave 2 — Introduce bursts (4 bots) to test their new skills
            if (wave == 2 && _frame % 80 == 0)
            {
                int lane = Random.Range(0, LANES);
                StartCoroutine(SpawnBurst(lane, 4));
                yield return new WaitForSeconds(0.2f);
                SpawnPacket(lane, false);
            }

            // Wave 3 — constant stream + heavy bursts
            if (wave == 3)
            {
                if (_frame % 30 == 0)
                    SpawnPacket(Random.Range(0, LANES), Random.value > 0.3f);

                if (_frame % 90 == 0)
                    StartCoroutine(SpawnBurst(Random.Range(0, LANES), 6));
            }
        }
    }

    IEnumerator SpawnBurst(int lane, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (GameController.Instance.State != GameController.GameState.PLAYING)
                yield break;

            SpawnPacket(lane, true);
            yield return new WaitForSeconds(0.2f);
        }
    }

    void SpawnPacket(int lane, bool isBot)
    {
        float laneAngle = (float)lane / LANES * Mathf.PI * 2f;
        float jitter    = (Random.value * 0.8f + 0.1f) * (Mathf.PI * 2f / LANES);
        float angle     = laneAngle + jitter;

        Vector2 spawnPos = new Vector2(
            _centerX + Mathf.Cos(angle) * _spawnRadius,
            _centerY + Mathf.Sin(angle) * _spawnRadius
        );

        Vector2 dir = new Vector2(_centerX - spawnPos.x, _centerY - spawnPos.y).normalized;

        // Speed - Slower overall pace for better strategic readability
        // Bots vary from 0.8 to 1.1 speed. Legit users crawl at 0.5.
        float speed = isBot ? (0.8f + Random.value * 0.3f) : 0.5f;

        GameObject prefab = isBot ? botPrefab : legitPrefab;
        GameObject go     = Instantiate(prefab, packetParent);

        Packet packet = go.GetComponent<Packet>();
        packet.Init(spawnPos, dir, speed, isBot, angle);

        _activePackets.Add(packet);
    }

    void OnDestroy()
    {
        if (GameController.Instance != null)
            GameController.Instance.OnStateChanged -= OnStateChanged;
    }
}