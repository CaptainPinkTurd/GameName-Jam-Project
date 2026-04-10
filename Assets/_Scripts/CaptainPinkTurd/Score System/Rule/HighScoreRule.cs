using CaptainPinkTurd.Core.Interfaces;
using CaptainPinkTurd.ScoreSystem.Data;

namespace CaptainPinkTurd.ScoreSystem.Rule
{
    public class HighScoreRule : IScoreRule
    {
        public bool IsBetter(ScoreEntry newScore, ScoreEntry existingScore)
        {
            return newScore.Score.Value > existingScore.Score.Value;
        }
    }
}