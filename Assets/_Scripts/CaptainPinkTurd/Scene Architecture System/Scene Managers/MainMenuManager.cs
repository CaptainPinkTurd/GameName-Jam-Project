using CaptainPinkTurd.Core.Attributes;
using UnityEngine;

namespace CaptainPinkTurd.Scene.Manager
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private bool randomizedInitialLevel;
        
        [ShowIf(nameof(randomizedInitialLevel), false)]
        [SerializeField] private int initialLevel = 1;
        [ShowIf(nameof(randomizedInitialLevel))]
        [SerializeField] private LevelData levelData;
        
        public void StartSession()
        {
            var level = randomizedInitialLevel ? Random.Range(1, levelData.totalLevelInGame + 1) : initialLevel;
            
            SceneController.Instance
                .NewTransition()
                .Load(SceneDatabase.Slots.Session, SceneDatabase.Scenes.Session)
                .Load(SceneDatabase.Slots.SessionContent, $"{SceneDatabase.Scenes.Level} {level}", true)
                .Unload(SceneDatabase.Slots.Menu)
                .WithOverlay()
                .Perform();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}