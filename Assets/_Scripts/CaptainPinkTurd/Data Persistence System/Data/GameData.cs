using System;

namespace CaptainPinkTurd.DataPersistence.Data
{
    public class GameData
    {
        public bool hasDoneTutorial = false;
        public int highScore = 0;

        public int GetPercentageComplete()
        {
            throw new NotImplementedException();
        }
    }
}