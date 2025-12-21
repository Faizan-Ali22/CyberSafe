using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic object pool for Unity GameObjects.
/// Supports async pre-warming to prevent runtime lag from instantiation.
/// </summary>
/// <typeparam name="T">The MonoBehaviour component type that implements IPoolable.</typeparam>
public class ObjectPool<T> where T : MonoBehaviour, IPoolable
{
    private readonly GameObject _prefab;
    private readonly Transform _parent;
    private readonly Queue<T> _pool = new Queue<T>();
    private readonly List<T> _activeObjects = new List<T>();
    private readonly int _initialSize;
    private readonly int _maxSize;
    private bool _isPreWarmed = false;

    /// <summary>
    /// Gets the number of available objects in the pool.
    /// </summary>
    public int AvailableCount => _pool.Count;

    /// <summary>
    /// Gets the number of currently active (in-use) objects.
    /// </summary>
    public int ActiveCount => _activeObjects.Count;

    /// <summary>
    /// Gets whether the pool has been pre-warmed.
    /// </summary>
    public bool IsPreWarmed => _isPreWarmed;

    /// <summary>
    /// Event raised when pre-warming is complete.
    /// </summary>
    public event Action OnPreWarmComplete;

    /// <summary>
    /// Creates a new object pool.
    /// </summary>
    /// <param name="prefab">The prefab to instantiate.</param>
    /// <param name="parent">Parent transform for pooled objects.</param>
    /// <param name="initialSize">Initial pool size for pre-warming.</param>
    /// <param name="maxSize">Maximum pool size (0 = unlimited).</param>
    public ObjectPool(GameObject prefab, Transform parent = null, int initialSize = 10, int maxSize = 0)
    {
        _prefab = prefab;
        _parent = parent;
        _initialSize = initialSize;
        _maxSize = maxSize;
    }

    /// <summary>
    /// Pre-warms the pool by instantiating objects ahead of time.
    /// Call this during loading screens to prevent runtime lag.
    /// </summary>
    /// <param name="mono">A MonoBehaviour to run the coroutine on.</param>
    /// <param name="objectsPerFrame">Number of objects to create per frame.</param>
    /// <returns>Coroutine for async pre-warming.</returns>
    public IEnumerator PreWarmAsync(MonoBehaviour mono, int objectsPerFrame = 5)
    {
        int created = 0;
        while (created < _initialSize)
        {
            int toCreate = Mathf.Min(objectsPerFrame, _initialSize - created);
            for (int i = 0; i < toCreate; i++)
            {
                CreatePooledObject();
                created++;
            }
            yield return null;
        }

        _isPreWarmed = true;
        OnPreWarmComplete?.Invoke();
        Debug.Log($"[ObjectPool<{typeof(T).Name}>] Pre-warm complete. {_pool.Count} objects ready.");
    }

    /// <summary>
    /// Pre-warms the pool synchronously.
    /// Use for smaller pools where instant loading is acceptable.
    /// </summary>
    public void PreWarmSync()
    {
        for (int i = 0; i < _initialSize; i++)
        {
            CreatePooledObject();
        }
        _isPreWarmed = true;
        Debug.Log($"[ObjectPool<{typeof(T).Name}>] Sync pre-warm complete. {_pool.Count} objects ready.");
    }

    /// <summary>
    /// Gets an object from the pool, creating one if necessary.
    /// </summary>
    /// <param name="position">Position to place the object.</param>
    /// <param name="rotation">Rotation for the object.</param>
    /// <returns>The pooled object component.</returns>
    public T Get(Vector3 position, Quaternion rotation)
    {
        T obj;

        if (_pool.Count > 0)
        {
            obj = _pool.Dequeue();
        }
        else
        {
            if (_maxSize > 0 && (_pool.Count + _activeObjects.Count) >= _maxSize)
            {
                Debug.LogWarning($"[ObjectPool<{typeof(T).Name}>] Max pool size reached. Returning null.");
                return null;
            }
            obj = CreateNewObject();
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.gameObject.SetActive(true);
        obj.OnSpawnFromPool();
        _activeObjects.Add(obj);

        return obj;
    }

    /// <summary>
    /// Returns an object to the pool for reuse.
    /// </summary>
    /// <param name="obj">The object to return.</param>
    public void Return(T obj)
    {
        if (obj == null) return;

        obj.OnReturnToPool();
        obj.gameObject.SetActive(false);
        _activeObjects.Remove(obj);
        _pool.Enqueue(obj);
    }

    /// <summary>
    /// Returns all active objects to the pool.
    /// </summary>
    public void ReturnAll()
    {
        var activeList = new List<T>(_activeObjects);
        foreach (var obj in activeList)
        {
            Return(obj);
        }
    }

    /// <summary>
    /// Clears the pool and destroys all pooled objects.
    /// </summary>
    public void Clear()
    {
        ReturnAll();
        while (_pool.Count > 0)
        {
            var obj = _pool.Dequeue();
            if (obj != null)
            {
                UnityEngine.Object.Destroy(obj.gameObject);
            }
        }
        _activeObjects.Clear();
        _isPreWarmed = false;
    }

    private void CreatePooledObject()
    {
        T obj = CreateNewObject();
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
    }

    private T CreateNewObject()
    {
        GameObject go = UnityEngine.Object.Instantiate(_prefab, _parent);
        T component = go.GetComponent<T>();
        if (component == null)
        {
            Debug.LogError($"[ObjectPool<{typeof(T).Name}>] Prefab missing {typeof(T).Name} component!");
        }
        return component;
    }
}
