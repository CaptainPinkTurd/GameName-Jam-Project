using System;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Struct;
using CaptainPinkTurd.HighScoreSystem.Data;
using CaptainPinkTurd.HighScoreSystem.Storage;
using CaptainPinkTurd.Managers;
using CaptainPinkTurd.UI;
using CaptainPinkTurd.UI.TextUI;
using CaptainPinkTurd.UI.TimerDisplayers;
using TMPro;
using UnityEngine;

namespace CaptainPinkTurd.HighScoreSystem
{
    public class HighScoreManager : MonoBehaviour //very scuff script to test out the high score system for now
    {
        [Header("Kill Score Config")]
        [SerializeField] private LeaderboardConfig scoreLeaderboardConfig;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TextFormatRule scoreFormat;
        
        [Header("Timer Score Config")]
        [SerializeField] private LeaderboardConfig timerLeaderboardConfig;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TextFormatRule timerFormat;
        [SerializeField] private StopwatchTimerDisplay timerDisplay;

        private HighScoreService highScoreService;
        
        private void Awake()
        {
            highScoreService = new HighScoreService(new LocalScoreStorage());
            
            GameManager.Instance.OnGameOver.Subscribe(OnGameOverEvent);
        }

        private void OnDestroy()
        {
            if (!gameObject.scene.isLoaded) return;
            
            GameManager.Instance.OnGameOver.Unsubscribe(OnGameOverEvent);
        }

        private void OnGameOverEvent()
        {
            FinalizeScores();
        }

        private void FinalizeScores()
        {
            var score = UiManager.Instance.GetKillCount();
            highScoreService.SubmitScore(scoreLeaderboardConfig, 
                new ScoreEntry(scoreLeaderboardConfig.leaderboardId.ToKey(), "Player", new SScoreValue(score)));
            scoreText.text = scoreFormat.Format(highScoreService.GetScores(scoreLeaderboardConfig)[0].Score.Value);

            var timeSurvived = timerDisplay.CurrentTime;
            highScoreService.SubmitScore(timerLeaderboardConfig, 
                new ScoreEntry(timerLeaderboardConfig.leaderboardId.ToKey(), "Player", new SScoreValue(timeSurvived)));
            
            var longestTime = highScoreService.GetScores(timerLeaderboardConfig)[0].Score.Value;
            int minutes = Mathf.FloorToInt(((float)longestTime % 3600f) / 60f);
            int seconds = Mathf.FloorToInt((float)longestTime % 60f);
            
            timerText.text = timerFormat.Format(minutes, seconds); 
        }
    }
}