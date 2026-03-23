namespace CaptainPinkTurd.Core.Struct
{
    [System.Serializable]
    public struct SScoreValue
    {
        public double Value;

        public SScoreValue(int score) => Value = score;
        public SScoreValue(double score) => Value = score;
    }
}