using System;
using System.Collections;
using UnityEngine;

namespace CaptainPinkTurd.Core.Utilities
{
    public static class HitStop
    {
        public static bool IsWaiting => waiting;
        
        private static bool waiting;
        private static float oldTimeScale;

        public static void Stop(MonoBehaviour caller, float duration, Action onStopEnd = null)
        {
            if (waiting)
            {
                onStopEnd?.Invoke();
                return;
            }
            oldTimeScale = Time.timeScale;
            Time.timeScale = 0.0f;

            if (!caller.gameObject.activeSelf)
            {
                Debug.LogWarning("Attempted to stop a hit stop on an inactive object.");
                Time.timeScale = oldTimeScale;
                return;
            }
            
            // Use the provided MonoBehaviour to start the coroutine.
            caller.StartCoroutine(Wait(duration, onStopEnd));
        }

        private static IEnumerator Wait(float duration, Action onWaitEnd = null)
        {
            waiting = true;
            yield return new WaitForSecondsRealtime(duration);
            
            Time.timeScale = oldTimeScale;
            onWaitEnd?.Invoke();
            waiting = false;
        }
    }
}