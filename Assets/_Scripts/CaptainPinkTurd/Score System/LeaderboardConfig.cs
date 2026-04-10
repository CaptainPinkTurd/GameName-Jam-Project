using CaptainPinkTurd.Core.Enum;
using UnityEngine;

namespace CaptainPinkTurd.ScoreSystem
{
    [CreateAssetMenu(fileName = "LeaderboardConfig", menuName = "Scriptable Objects/Score System/Leaderboard Config")]
    public class LeaderboardConfig : ScriptableObject
    {
        public ELeaderboardId leaderboardId;

        [Tooltip("Maximum number of scores kept")]
        public int maxEntries = 10;

        [Tooltip("How scores are compared")]
        public EScoreRuleType ruleType;
    }
}