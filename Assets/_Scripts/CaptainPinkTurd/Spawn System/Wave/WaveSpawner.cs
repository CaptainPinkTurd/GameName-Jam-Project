using CaptainPinkTurd.SpawnSystem.Procedural;
using UnityEngine;

namespace CaptainPinkTurd.SpawnSystem.Wave
{
    [RequireComponent(typeof(ProceduralObjectSpawner))]
    public class WaveSpawner : MonoBehaviour
    {
        [SerializeField] private int[] numberToSpawnEachWave;
        
        private ProceduralObjectSpawner proceduralSpawner;
        private float timeBetweenSpawns;
        private int waveIndex = -1;

        private void Awake()
        {
            proceduralSpawner = GetComponent<ProceduralObjectSpawner>();
        }

        private void OnEnable()
        {
            proceduralSpawner.OnObjectDespawn.Subscribe(WaveIncrement);
        }
        private void OnDisable()
        {
            proceduralSpawner.OnObjectDespawn.Unsubscribe(WaveIncrement);
        }
        private void Start()
        {
            WaveIncrement();
        }
        
        private void Spawn()
        {
            StartCoroutine(proceduralSpawner.SpawnObjects(numberToSpawnEachWave[waveIndex]));
        }

        private void WaveIncrement()
        {
            if (proceduralSpawner.CurrentActiveObjects > 0) return;
            
            waveIndex = (waveIndex + 1) % numberToSpawnEachWave.Length;

            Spawn();
        }
    }
}