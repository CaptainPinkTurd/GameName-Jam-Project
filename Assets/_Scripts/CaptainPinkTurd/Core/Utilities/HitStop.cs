using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CaptainPinkTurd.Core.Utilities
{
    public static class HitStop
    {
        public static bool IsWaiting => running;

        private static bool running;
        private static float remainingTime;
        private static float oldTimeScale;

        // Queue all callbacks
        private static Action pendingCallbacks;

        private class HitStopRunner : MonoBehaviour { }

        private static HitStopRunner runner;

        private static void EnsureRunner()
        {
            if (runner) return;

            var go = new GameObject("[HitStopRunner]");
            runner = go.AddComponent<HitStopRunner>();
            Object.DontDestroyOnLoad(go);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ResetStaticVariables()
        {
            running = false;
            remainingTime = 0f;
            pendingCallbacks = null;
        }

        public static void Stop(float duration, Action onStopEnd = null)
        {
            EnsureRunner();

            // Always queue callback (even if already running)
            if (onStopEnd != null)
            {
                pendingCallbacks -= onStopEnd; //prevent duplicates
                pendingCallbacks += onStopEnd; 
            }
            
            // Stack duration
            remainingTime = Mathf.Max(remainingTime, duration);

            // If already running, return after extending remainingTime
            if (running) return;
            
            running = true;
            oldTimeScale = Time.timeScale;
            Time.timeScale = 0.0f;

            runner.StartCoroutine(WaitLoop());
        }

        private static IEnumerator WaitLoop()
        {
            while (remainingTime > 0f)
            {
                remainingTime -= Time.unscaledDeltaTime;
                yield return null;
            }

            Time.timeScale = oldTimeScale;

            // Execute ALL queued callbacks safely
            try
            {
                pendingCallbacks?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"HitStop callback error: {e}");
            }

            // Reset state
            pendingCallbacks = null;
            remainingTime = 0f;
            running = false;
        }
    }
}