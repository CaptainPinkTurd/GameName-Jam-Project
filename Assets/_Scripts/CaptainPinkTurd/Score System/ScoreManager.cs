using System.Globalization;
using CaptainPinkTurd.Core.DesignPattern.Singleton;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Struct;
using CaptainPinkTurd.ScoreSystem.Data;
using CaptainPinkTurd.ScoreSystem.Storage;
using UnityEngine;

namespace CaptainPinkTurd.ScoreSystem
{
    public class ScoreManager : Singleton<ScoreManager> //Logic change depending on the game
    {
        [Header("Score Config")]
        [SerializeField] private LeaderboardConfig scoreLeaderboardConfig;
        [SerializeField] private StringEvent onScoreUpdate;
        [SerializeField] private StringEvent onHighScoreUpdate;

        private ScoreService highScoreService;
        private string scoreText;
        
        public int Score { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();
                
            highScoreService = new ScoreService(new LocalScoreStorage());
            scoreText = "000000";
        }

        public void AddScore(IScorable scorable)
        {
            int value = scorable.ScoreConfig.GetFinalScore();
            Score += value;

            scoreText = GetScoreDisplayText(Score);

            onScoreUpdate.Raise(scoreText);
            scorable.OnScored();
        }
        private void FinalizeScores()
        {
            highScoreService.SubmitScore(scoreLeaderboardConfig, 
                new ScoreEntry(scoreLeaderboardConfig.leaderboardId.ToKey(), "Player", new SScoreValue(Score)));
            
            var currentHighScore = highScoreService.GetScores(scoreLeaderboardConfig)[0].Score.Value;
            var highScoreText = GetScoreDisplayText((int)currentHighScore);
            
            onHighScoreUpdate.Raise(highScoreText);
        }

        private string GetScoreDisplayText(int score)
        {
            string scoreText;
            
            if (score <= 999999)
            {
                scoreText = score switch
                {
                    < 10 => "00000" + score,
                    < 100 => "0000" + score,
                    < 1000 => "000" + score,
                    < 10000 => "00" + score,
                    < 100000 => "0" + score,
                    _ => score.ToString(CultureInfo.InvariantCulture)
                };
            }
            else
            {
                scoreText = score.ToString(CultureInfo.InvariantCulture);
            }
            
            return scoreText;
        }
        public void OnLevelSceneLoadedEvent()
        {
            onScoreUpdate.Raise(scoreText);
        }
        public void OnGameOverEvent()
        {
            FinalizeScores();
        }
    }
}