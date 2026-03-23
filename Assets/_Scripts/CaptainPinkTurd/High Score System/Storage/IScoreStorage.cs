using System.Collections.Generic;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.HighScoreSystem.Data;

namespace CaptainPinkTurd.Core.Interfaces
{
    public interface IScoreStorage
    {
        void SaveScore(ELeaderboardId leaderboardId, ScoreEntry entry);
        List<ScoreEntry> LoadScores(ELeaderboardId leaderboardId);
        void Clear(ELeaderboardId leaderboardId);
    }
}