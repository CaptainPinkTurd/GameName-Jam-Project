using CaptainPinkTurd.DataPersistence.Data;
using UnityEngine;

namespace CaptainPinkTurd.DataPersistence
{
    public interface IDataPersistence
    {
        string Name { get; } 
        void LoadData(GameData data);
        void SaveData(GameData data);
    }
}