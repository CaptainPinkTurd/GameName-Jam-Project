using System;
using UnityEngine;

namespace CaptainPinkTurd.ImprovedTimers
{
    public abstract class Timer : IDisposable 
    {
        public float CurrentTime { get; protected set; }
        public bool IsRunning { get; private set; }
        public abstract bool IsFinished { get; }
        public bool HasTriggeredEvent { get; private set; }
        public abstract float Progress { get; }

        protected float initialTime;

        public Action OnTimerStart = delegate { };
        public Action OnMidTimer = delegate { };
        public Action OnTimerStop = delegate { };

        protected Timer(float value) 
        {
            initialTime = value;
        }

        public void Start() 
        {
            CurrentTime = initialTime;
            if (!IsRunning)
            {
                IsRunning = true;
                TimerManager.RegisterTimer(this);
                OnTimerStart?.Invoke();
            }
        }

        public void TriggerEventMidTimer()
        {
            if (IsRunning && !HasTriggeredEvent) 
            {
                HasTriggeredEvent = true;
                OnMidTimer?.Invoke();
            }
        } 
        public void Stop()
        {
            if (!IsRunning) return;
            
            IsRunning = false;
            TimerManager.DeregisterTimer(this);
            OnTimerStop?.Invoke();
        }

        public abstract void Tick();

        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;

        public virtual void Reset() => CurrentTime = initialTime;

        public virtual void Reset(float newTime)
        {
            initialTime = newTime;
            Reset();
        }

        bool disposed;

        ~Timer() 
        {
            Dispose(false);
        }

        // Call Dispose to ensure deregistration of the timer from the TimerManager
        // when the consumer is done with the timer or being destroyed
        public void Dispose() 
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) 
        {
            if (disposed) return;

            if (disposing)
            {
                TimerManager.DeregisterTimer(this);
            }

            disposed = true;
        }
    }
}