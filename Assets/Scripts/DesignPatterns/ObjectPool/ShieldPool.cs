using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Manages a pool of shield objects for the Malware Maze game.
/// Pre-warms the pool to prevent runtime lag from instantiating high-vertex prefabs.
/// </summary>
public class ShieldPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private int maxPoolSize = 20;
    [SerializeField] private int objectsPerFrame = 2;

    private ObjectPool<PoolableShield> _pool;

    /// <summary>
    /// Gets whether the pool has been pre-warmed and is ready for use.
    /// </summary>
    public bool IsReady => _pool?.IsPreWarmed ?? false;

    /// <summary>
    /// Event raised when pre-warming is complete.
    /// </summary>
    public event Action OnPoolReady;

    /// <summary>
    /// The number of available shields in the pool.
    /// </summary>
    public int AvailableCount => _pool?.AvailableCount ?? 0;

    /// <summary>
    /// Initializes the pool with the specified prefab.
    /// </summary>
    /// <param name="prefab">The shield prefab to pool.</param>
    public void Initialize(GameObject prefab)
    {
        if (prefab != null)
        {
            shieldPrefab = prefab;
        }

        if (shieldPrefab == null)
        {
            Debug.LogError("[ShieldPool] No shield prefab assigned!");
            return;
        }

        _pool = new ObjectPool<PoolableShield>(
            shieldPrefab,
            transform,
            initialPoolSize,
            maxPoolSize
        );
    }

    /// <summary>
    /// Pre-warms the pool asynchronously.
    /// Call this during loading to prevent runtime lag.
    /// </summary>
    /// <returns>Coroutine for async pre-warming.</returns>
    public IEnumerator PreWarmAsync()
    {
        if (_pool == null)
        {
            Debug.LogError("[ShieldPool] Pool not initialized. Call Initialize first.");
            yield break;
        }

        _pool.OnPreWarmComplete += HandlePreWarmComplete;
        yield return _pool.PreWarmAsync(this, objectsPerFrame);
    }

    /// <summary>
    /// Pre-warms the pool synchronously.
    /// </summary>
    public void PreWarmSync()
    {
        if (_pool == null)
        {
            Debug.LogError("[ShieldPool] Pool not initialized. Call Initialize first.");
            return;
        }

        _pool.PreWarmSync();
        OnPoolReady?.Invoke();
    }

    /// <summary>
    /// Gets a shield from the pool.
    /// </summary>
    /// <param name="position">Position to place the shield.</param>
    /// <param name="rotation">Rotation for the shield.</param>
    /// <returns>The pooled shield component.</returns>
    public PoolableShield GetShield(Vector3 position, Quaternion rotation)
    {
        if (_pool == null)
        {
            Debug.LogError("[ShieldPool] Pool not initialized!");
            return null;
        }

        return _pool.Get(position, rotation);
    }

    /// <summary>
    /// Returns a shield to the pool.
    /// </summary>
    /// <param name="shield">The shield to return.</param>
    public void ReturnShield(PoolableShield shield)
    {
        _pool?.Return(shield);
    }

    /// <summary>
    /// Returns all active shields to the pool.
    /// </summary>
    public void ReturnAllShields()
    {
        _pool?.ReturnAll();
    }

    /// <summary>
    /// Clears the pool and destroys all pooled shields.
    /// </summary>
    public void Clear()
    {
        _pool?.Clear();
    }

    private void HandlePreWarmComplete()
    {
        _pool.OnPreWarmComplete -= HandlePreWarmComplete;
        OnPoolReady?.Invoke();
    }

    private void OnDestroy()
    {
        _pool?.Clear();
    }
}
