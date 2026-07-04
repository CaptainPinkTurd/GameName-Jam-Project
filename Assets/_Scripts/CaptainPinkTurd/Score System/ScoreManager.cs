using System.Globalization;
using CaptainPinkTurd.Core.DesignPattern.Singleton;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.DataPersistence;
using CaptainPinkTurd.DataPersistence.Data;
using CaptainPinkTurd.ScoreSystem.Rule;
using UnityEngine;

namespace CaptainPinkTurd.ScoreSystem
{
    public class ScoreManager : Singleton<ScoreManager>, IDataPersistence //Logic change depending on the game
    {
        [Header("Score Config")]
        [SerializeField] private EScoreRuleType scoreRule;
        [SerializeField] private StringEvent onScoreUpdate;
        [SerializeField] private StringEvent onHighScoreUpdate;

        private int bestScore;
        private string scoreText;
        
        public int Score { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();
                
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
            var rule = ScoreRuleFactory.Create(scoreRule);
            if (rule.IsBetter(Score, bestScore))
            {
                bestScore = Score;
            }

            onHighScoreUpdate.Raise(GetScoreDisplayText(bestScore));
            
            if(!DataPersistenceManager.HasInstance || !DataPersistenceManager.Instance.HasGameData)
            {
                Debug.LogError("ScoreManager: DataPersistenceManager.HasInstance == false or has no game data available to save ");
                return;
            }
            DataPersistenceManager.Instance.SaveGame();
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
            
            scoreText = "000000";
            Score = 0;
        }

        public string Name => name;

        public void LoadData(GameData data)
        {
            bestScore = data.highScore;
        }

        public void SaveData(GameData data)
        {
            data.highScore = bestScore;
        }
    }
}