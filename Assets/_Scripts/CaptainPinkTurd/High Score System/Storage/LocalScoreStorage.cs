using System.Collections.Generic;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Interfaces;
using CaptainPinkTurd.HighScoreSystem.Data;
using UnityEngine;

namespace CaptainPinkTurd.HighScoreSystem.Storage
{
    public class LocalScoreStorage : IScoreStorage
    {
        public void SaveScore(ELeaderboardId leaderboardId, ScoreEntry entry)
        {
            var scores = LoadScores(leaderboardId);
            scores.Add(entry);

            string json = JsonUtility.ToJson(new ScoreListWrapper(scores));
            PlayerPrefs.SetString(leaderboardId.ToKey(), json);
        }

        public List<ScoreEntry> LoadScores(ELeaderboardId leaderboardId)
        {
            if (!PlayerPrefs.HasKey(leaderboardId.ToKey()))
                return new List<ScoreEntry>();

            string json = PlayerPrefs.GetString(leaderboardId.ToKey());
            return JsonUtility.FromJson<ScoreListWrapper>(json).Scores;
        }

        public void Clear(ELeaderboardId leaderboardId)
        {
            PlayerPrefs.DeleteKey(leaderboardId.ToKey());
        }
    }
    [System.Serializable]
    public class ScoreListWrapper
    {
        public List<ScoreEntry> Scores;
        public ScoreListWrapper(List<ScoreEntry> scores) => Scores = scores;
    }
}