using CaptainPinkTurd.Core.Attributes;
using UnityEngine;

namespace CaptainPinkTurd.ScoreSystem
{
    [CreateAssetMenu(fileName = "ScoreConfig", menuName = "Scriptable Objects/Score System/Score Config")]
    public class ScoreConfig : ScriptableObject
    {
        [SerializeField] private int baseScore = 10;

        [Header("Modifiers")]
        [SerializeField] private bool useRandomRange;
        [ShowIf(nameof(useRandomRange))]
        [SerializeField] private int minScore = 5;
        [ShowIf(nameof(useRandomRange))]
        [SerializeField] private int maxScore = 20;

        [SerializeField] private bool useMultiplier;
        [ShowIf(nameof(useMultiplier))]
        [SerializeField] private float multiplier = 1f;

        public int GetFinalScore()
        {
            int score = baseScore;

            if (useRandomRange)
            {
                score = Random.Range(minScore, maxScore + 1);
            }

            if (useMultiplier)
            {
                score = Mathf.RoundToInt(score * multiplier);
            }

            return score;
        }
    }
}