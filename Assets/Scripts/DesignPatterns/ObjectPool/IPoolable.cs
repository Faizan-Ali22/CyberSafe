using UnityEngine;

/// <summary>
/// Interface for objects that can be pooled.
/// Implement this interface to handle spawn and return events.
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// Called when the object is retrieved from the pool.
    /// Use this to reset/initialize the object state.
    /// </summary>
    void OnSpawnFromPool();

    /// <summary>
    /// Called when the object is returned to the pool.
    /// Use this to clean up and prepare for reuse.
    /// </summary>
    void OnReturnToPool();
}
