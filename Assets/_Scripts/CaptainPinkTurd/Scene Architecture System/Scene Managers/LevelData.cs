using CaptainPinkTurd.DataPersistence;
using CaptainPinkTurd.DataPersistence.Data;
using UnityEngine;

namespace CaptainPinkTurd.Scene.Manager
{
    [CreateAssetMenu(fileName = "Level Data", menuName = "Game/Level/Level Data")]
    public class LevelData : ScriptableObject, IDataPersistence
    {
        public bool hasDoneTutorial;
        public int totalLevelInGame = 6;

        public string Name => name;

        public void LoadData(GameData data)
        {
            hasDoneTutorial = data.hasDoneTutorial;
        }

        public void SaveData(GameData data)
        {
            //no need to save hasDoneTutorial once it has been done
            if (hasDoneTutorial)
            {
                data.hasDoneTutorial = true;
                return;
            }
            data.hasDoneTutorial = false;
        }
    }
}
