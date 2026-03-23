using CaptainPinkTurd.Core.Enum;

namespace CaptainPinkTurd.Core.Extensions
{
    public static class LeaderboardIdExtensions
    {
        public static string ToKey(this ELeaderboardId id)
        {
            return $"Leaderboard_{id}";
        }
    }
}