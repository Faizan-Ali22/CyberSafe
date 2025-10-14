using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DA_Assets.DAI
{
#if UNITY_EDITOR
    [InitializeOnLoad]
    public class DARunner
    {
        /// <summary>
        /// Thread-safe queue for actions to be executed on the main thread.
        /// </summary>
        private static ConcurrentQueue<Action> _executionQueue = new ConcurrentQueue<Action>();

        /// <summary>
        /// Time since the script was reloaded.
        /// </summary>
        public static double timeSinceScriptReload => _stopwatch.Elapsed.TotalSeconds;

        /// <summary>
        /// Delegate for update functions.
        /// </summary>
        public static Action update;

        /// <summary>
        /// Delta time between updates.
        /// </summary>
        public static float deltaTime => (float)_deltaTime;
        private static double _deltaTime;

        private static double _previousTime;
        private static Stopwatch _stopwatch;

        private static int _frequencyMs = 15; // Update frequency in milliseconds.

        private static Thread _thread;
        private static CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Static constructor to initialize the DARunner.
        /// </summary>
        static DARunner()
        {
            update += () =>
            {
                while (_executionQueue.TryDequeue(out var action))
                {
                    try
                    {
                        action?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error executing action on main thread: {ex.Message}");
                    }
                }
            };

            _stopwatch = Stopwatch.StartNew();
            SynchronizationContext context = SynchronizationContext.Current;

            Update(context);

            // Subscribe to assembly reload events to properly terminate the thread.
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        }

        /// <summary>
        /// Called before assembly reload to terminate the background thread.
        /// </summary>
        private static void OnBeforeAssemblyReload()
        {
            _cancellationTokenSource?.Cancel();
            _thread?.Join();
            _thread = null;
            _cancellationTokenSource = null;
        }

        /// <summary>
        /// Starts the background thread that posts updates to the main thread's synchronization context.
        /// </summary>
        /// <param name="context">The synchronization context of the main thread.</param>
        private static void Update(SynchronizationContext context)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;

            _thread = new Thread(() =>
            {
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        double currentTime = _stopwatch.Elapsed.TotalSeconds;
                        _deltaTime = currentTime - _previousTime;
                        _previousTime = currentTime;
                        context.Post(_ => update(), null);
                        Thread.Sleep(_frequencyMs);
                    }
                }
                // Add Code Start
                catch (ThreadAbortException ex)
                {
                    if (Application.isBatchMode) return;
                    context.Post(_ => { Debug.LogError($"Error in background thread: {ex.Message}"); }, null);
                }
                // Add Code End
                catch (Exception ex)
                {
                    // Change Code
                    context.Post(_ => { Debug.LogError($"Error in background thread: {ex.Message}"); }, null);
                }
            });

            _thread.Start();
        }

        /// <summary>
        /// Enqueues an action to be executed on the main thread.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public static void ExecuteOnMainThread(Action action)
        {
            _executionQueue.Enqueue(action);
        }
    }
#endif
}
