using UnityEngine;

namespace CaptainPinkTurd.Core.Interfaces
{
    public interface ISpawner
    {
        int CurrentActiveObjects { get; }
        void SetSpawnableState(bool value);
        void SpawnObjects(int amount);
        void DespawnObject(GameObject obj);
        void DespawnObjects();
    }
}
