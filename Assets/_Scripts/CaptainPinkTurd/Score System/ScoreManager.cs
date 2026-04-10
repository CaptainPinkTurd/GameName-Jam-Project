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

        private ScoreService highScoreService;
        private string scoreText;
        
        public int Score { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();
                
            highScoreService = new ScoreService(new LocalScoreStorage());
            scoreText = "000000";
        }

        public void OnGameOverEvent()
        {
            FinalizeScores();
        }

        public void AddScore(IScorable scorable)
        {
            int value = scorable.ScoreConfig.GetFinalScore();
            Score += value;

            if (Score <= 999999)
            {
                scoreText = Score switch
                {
                    < 10 => "00000" + Score,
                    < 100 => "0000" + Score,
                    < 1000 => "000" + Score,
                    < 10000 => "00" + Score,
                    < 100000 => "0" + Score,
                    _ => Score.ToString(CultureInfo.InvariantCulture)
                };
            }
            else
            {
                scoreText = Score.ToString(CultureInfo.InvariantCulture);
            }

            onScoreUpdate.Raise(scoreText);
            scorable.OnScored();
        }
        private void FinalizeScores()
        {
            highScoreService.SubmitScore(scoreLeaderboardConfig, 
                new ScoreEntry(scoreLeaderboardConfig.leaderboardId.ToKey(), "Player", new SScoreValue(Score)));
            //scoreText.text = highScoreService.GetScores(scoreLeaderboardConfig)[0].Score.Value.ToString(CultureInfo.InvariantCulture);
        }

        public void OnLevelSceneLoadedEvent()
        {
            onScoreUpdate.Raise(scoreText);
        }
    }
}