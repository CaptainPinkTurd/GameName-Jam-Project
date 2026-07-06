using CaptainPinkTurd.DataPersistence.Data;

namespace CaptainPinkTurd.DataPersistence
{
    public interface IDataPersistence
    {
        string Name { get; } 
        void LoadData(GameData data);
        void SaveData(GameData data);
    }
}