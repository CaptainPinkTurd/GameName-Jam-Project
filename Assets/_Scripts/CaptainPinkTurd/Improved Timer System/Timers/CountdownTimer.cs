using UnityEngine;

namespace CaptainPinkTurd.ImprovedTimers
{
    /// <summary>
    /// Timer that counts down from a specific value to zero.
    /// </summary>
    public class CountdownTimer : Timer 
    {
        private float eventTriggerTime;
        
        public override float Progress => 1f - Mathf.Clamp(CurrentTime / initialTime, 0, 1);
        
        public CountdownTimer(float value, float eventTriggerTime) : base(value)
        {
            this.eventTriggerTime = Mathf.Clamp(eventTriggerTime / initialTime, 0, 1);
        }

        public override void Tick() 
        {
            if (IsRunning && CurrentTime > 0) 
            {
                CurrentTime -= Time.deltaTime;
            }
            if (Progress >= eventTriggerTime && !HasTriggeredEvent)
            {
                TriggerEventMidTimer();
            }
            if (IsRunning && CurrentTime <= 0)
            {
                Stop();
            }
        }

        public override bool IsFinished => CurrentTime <= 0;
    }
}