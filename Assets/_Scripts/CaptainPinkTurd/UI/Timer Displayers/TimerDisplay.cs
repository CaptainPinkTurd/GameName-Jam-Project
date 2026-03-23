using CaptainPinkTurd.ImprovedTimers;
using CaptainPinkTurd.Managers;
using CaptainPinkTurd.UI.TextUI;
using TMPro;
using UnityEngine;

namespace CaptainPinkTurd.UI.TimerDisplayers
{
    public abstract class TimerDisplay : MonoBehaviour
    {
        [Header("Timer Display Config")]
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TextFormatRule timerFormatRule;
        
        protected Timer timer;
        
        public float CurrentTime => timer.CurrentTime;

        private void Awake()
        {
            SetupTimer();
        }

        protected virtual void OnEnable()
        {
            timer.Start();
            GameManager.Instance.OnGameOver.Subscribe(StopTimer);
        }

        protected virtual void OnDisable()
        {
            timer.Stop();
            
            if(!gameObject.scene.isLoaded) return;
            GameManager.Instance.OnGameOver.Unsubscribe(StopTimer);
        }

        private void Update()
        {
            TimerUpdate();
        }

        protected void SetTimerTextByFormat(params object[] values)
        {
            timerText.text = timerFormatRule.Format(values);
        }
        protected void StopTimer() => timer.Stop();
        protected abstract void SetupTimer();
        protected abstract void TimerUpdate();

        private void OnDestroy()
        {
            timer.Dispose();
        }
    }
}