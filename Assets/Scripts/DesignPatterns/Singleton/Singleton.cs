using UnityEngine;

/// <summary>
/// Generic Singleton base class for MonoBehaviours that should have only one instance per scene.
/// This singleton is destroyed when the scene is unloaded.
/// </summary>
/// <typeparam name="T">The type of the singleton class.</typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new object();
    private static bool _applicationIsQuitting = false;

    /// <summary>
    /// Gets the singleton instance. Creates one if it doesn't exist.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance of {typeof(T)} already destroyed on application quit. Returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        Debug.LogWarning($"[Singleton] No instance of {typeof(T)} found in scene. Make sure one exists in the scene.");
                    }
                }

                return _instance;
            }
        }
    }

    /// <summary>
    /// Returns true if the singleton instance exists.
    /// </summary>
    public static bool HasInstance => _instance != null;

    /// <summary>
    /// Override this method to add additional initialization logic.
    /// Always call base.Awake() first when overriding.
    /// </summary>
    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"[Singleton] Duplicate instance of {typeof(T)} found. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        _instance = this as T;
        OnSingletonAwake();
    }

    /// <summary>
    /// Called after the singleton instance is established.
    /// Override this instead of Awake for initialization logic.
    /// </summary>
    protected virtual void OnSingletonAwake() { }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    protected virtual void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }
}
