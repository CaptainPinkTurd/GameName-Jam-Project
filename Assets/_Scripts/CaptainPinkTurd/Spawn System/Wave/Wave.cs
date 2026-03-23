using CaptainPinkTurd.UnitSystem;
using UnityEngine;

namespace CaptainPinkTurd.SpawnSystem.Wave
{
    [CreateAssetMenu(fileName = "Wave", menuName = "Scriptable Objects/Spawn System/Wave")]
    public class Wave : ScriptableObject
    {
        [SerializeField] internal UnitBase[] enemiesInWave;
        [SerializeField][Range(1, 6)] internal int numberToSpawn;
    }
}