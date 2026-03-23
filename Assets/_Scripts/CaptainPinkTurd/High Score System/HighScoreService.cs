using System.Collections.Generic;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.Core.Interfaces;
using CaptainPinkTurd.HighScoreSystem.Data;
using CaptainPinkTurd.HighScoreSystem.Rule;

namespace CaptainPinkTurd.HighScoreSystem
{
    public class HighScoreService
    {
        private readonly IScoreStorage storage;

        public HighScoreService(IScoreStorage storage)
        {
            this.storage = storage;
        }

        public void SubmitScore(LeaderboardConfig config, ScoreEntry entry)
        {
            var scores = storage.LoadScores(config.leaderboardId);

            scores.Add(entry);
            ApplyRuleAndLimit
            (
                ref scores,
                ScoreRuleFactory.Create(config.ruleType),
                config.maxEntries
            );

            SaveAll(config.leaderboardId, scores);
        }

        public List<ScoreEntry> GetScores(LeaderboardConfig config)
        {
            return storage.LoadScores(config.leaderboardId);
        }
        
        #region LEADERBOARD SORTER LOGIC
        
        private void ApplyRuleAndLimit(ref List<ScoreEntry> scores, IScoreRule rule, int maxEntries)
        {
            scores.Sort((a, b) =>
            {
                if (rule.IsBetter(a, b)) return -1;
                if (rule.IsBetter(b, a)) return 1;
                return 0;
            });

            if (scores.Count > maxEntries)
                scores.RemoveRange(maxEntries, scores.Count - maxEntries);
        }

        private void SaveAll(ELeaderboardId id, List<ScoreEntry> scores)
        {
            storage.Clear(id);
            foreach (var score in scores)
            {
                storage.SaveScore(id, score);
            }
        }
        
        #endregion
    }
}