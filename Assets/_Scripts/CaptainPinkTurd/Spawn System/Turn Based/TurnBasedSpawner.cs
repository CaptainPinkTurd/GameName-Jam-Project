// using CaptainPinkTurd.TurnBasedSystem;
// using CaptainPinkTurd.UnitSystem;
// using UnityEngine;
//
// namespace CaptainPinkTurd.SpawnSystem.TurnBased
// {
//     public class TurnBasedSpawner : MonoBehaviour
//     {
//         [SerializeField] private SpawnSettings spawnSettings;
//         [SerializeField] private bool randomSpawn;
//         
//         private UnitFactory unitFactory;
//         private int turnPin;
//         private int turnPassSinceSpawn => TurnManager.Instance.turnNumberCount - turnPin;
//         
//         private void Awake()
//         {
//             unitFactory = new UnitFactory();  
//         }
//     }
// }