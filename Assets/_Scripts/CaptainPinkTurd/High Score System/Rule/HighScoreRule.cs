using CaptainPinkTurd.Core.Interfaces;
using CaptainPinkTurd.HighScoreSystem.Data;

namespace CaptainPinkTurd.HighScoreSystem.Rule
{
    public class HighScoreRule : IScoreRule
    {
        public bool IsBetter(ScoreEntry newScore, ScoreEntry existingScore)
        {
            return newScore.Score.Value > existingScore.Score.Value;
        }
    }
}