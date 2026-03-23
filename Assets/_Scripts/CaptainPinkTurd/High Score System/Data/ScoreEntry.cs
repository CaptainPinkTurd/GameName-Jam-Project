using CaptainPinkTurd.Core.Struct;

namespace CaptainPinkTurd.HighScoreSystem.Data
{
    [System.Serializable]
    public class ScoreEntry
    {
        public string PlayerId;
        public string PlayerName;
        public SScoreValue Score;
        public long Timestamp;

        public ScoreEntry(string playerId, string playerName, SScoreValue score)
        {
            PlayerId = playerId;
            PlayerName = playerName;
            Score = score;
            Timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}
