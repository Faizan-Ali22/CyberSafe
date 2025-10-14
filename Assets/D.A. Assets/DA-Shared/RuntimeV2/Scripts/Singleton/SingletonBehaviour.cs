using UnityEngine;

namespace DA_Assets.Singleton
{
    public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
#if UNITY_2023_1_OR_NEWER
                    _instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);
#else
                    _instance = FindObjectOfType<T>(true);
#endif
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}