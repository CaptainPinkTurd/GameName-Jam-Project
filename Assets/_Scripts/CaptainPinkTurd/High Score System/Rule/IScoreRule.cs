using CaptainPinkTurd.HighScoreSystem.Data;

namespace CaptainPinkTurd.Core.Interfaces
{
    public interface IScoreRule
    {
        bool IsBetter(ScoreEntry newScore, ScoreEntry existingScore);
    }
}