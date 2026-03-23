using System.Collections;
using System.Collections.Generic;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.Core.Utilities;
using CaptainPinkTurd.UnitSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CaptainPinkTurd.SpawnSystem.Wave
{
    public class WaveSpawner : MonoBehaviour
    {
        [SerializeField] private Wave[] waves;
        [SerializeField] private List<Transform> spawnPoints;
        [SerializeField] private float spawnDelay = .3f;
        
        private Wave currentWave;
        private float timeBetweenSpawns;
        private int waveIndex = -1;
        private int currentEnemiesInWaveCount;

        private void Start()
        {
            WaveIncrement();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator Spawn()
        {
            for (int i = 0; i < currentWave.numberToSpawn; i++)
            {
                currentEnemiesInWaveCount++;
                int spawnPointIndex = Random.Range(0, spawnPoints.Count);
                int enemyIndex = Random.Range(0, currentWave.enemiesInWave.Length);

                UnitBase spawnUnit = ObjectPoolManager.Instance.SpawnObject(currentWave.enemiesInWave[enemyIndex].gameObject,
                    spawnPoints[spawnPointIndex].position, Quaternion.identity, ObjectPoolManager.PoolType.Unit)
                    .GetComponent<UnitBase>();
                
                spawnUnit.OnDisableEvents.RefreshWithCachedListeners();
                spawnUnit.OnDisableEvents.Subscribe(() =>
                {
                    currentEnemiesInWaveCount--;
                    WaveIncrement();
                }, EPriority.Medium, false);

                yield return new WaitForSeconds(spawnDelay);
            }
        }

        private void WaveIncrement()
        {
            if (currentEnemiesInWaveCount > 0) return;
            
            currentEnemiesInWaveCount = 0;
            
            waveIndex = (waveIndex + 1) % waves.Length;
            currentWave = waves[waveIndex];
            
            StartCoroutine(Spawn());
        }
    }
}
