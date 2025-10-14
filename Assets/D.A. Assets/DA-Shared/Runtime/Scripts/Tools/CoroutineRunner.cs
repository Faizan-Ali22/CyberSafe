using System.Collections;
using UnityEngine;

namespace DA_Assets.Tools
{
    internal static class CoroutineRunner
    {
        private static GameObject _coroutineRunnerGameObject;
        private static CoroutineRunnerBehaviour _coroutineRunnerInstance;

        public static Coroutine StartCoroutine(IEnumerator coroutine)
        {
            CreateGameObjectIfNotExists();
            return _coroutineRunnerInstance.StartCoroutine(coroutine);
        }

        private static void CreateGameObjectIfNotExists()
        {
            if (_coroutineRunnerGameObject == null)
            {
                _coroutineRunnerGameObject = new GameObject(name: nameof(CoroutineRunner));
                _coroutineRunnerInstance = _coroutineRunnerGameObject.AddComponent<CoroutineRunnerBehaviour>();

                if (Application.isPlaying)
                {
                    UnityEngine.Object.DontDestroyOnLoad(_coroutineRunnerGameObject);
                }
            }
        }

        private class CoroutineRunnerBehaviour : MonoBehaviour { }
    }
}
