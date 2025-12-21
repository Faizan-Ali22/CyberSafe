using UnityEngine;

/// <summary>
/// Poolable shield pickup that implements IPoolable for efficient reuse.
/// Replaces ShieldPickup for use with the object pool system.
/// </summary>
public class PoolableShield : MonoBehaviour, IPoolable
{
    /// <summary>
    /// Reference to the MazeGameManager.
    /// </summary>
    [HideInInspector]
    public MazeGameManager manager;

    private bool consumed = false;

    /// <summary>
    /// Called when the shield is retrieved from the pool.
    /// Resets the shield state for reuse.
    /// </summary>
    public void OnSpawnFromPool()
    {
        consumed = false;
    }

    /// <summary>
    /// Called when the shield is returned to the pool.
    /// Cleans up the shield state.
    /// </summary>
    public void OnReturnToPool()
    {
        consumed = true;
        manager = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (consumed) return;
        if (!other.CompareTag("Player")) return;

        consumed = true;
        manager?.OnPoolableShieldCollected(this);
    }
}
