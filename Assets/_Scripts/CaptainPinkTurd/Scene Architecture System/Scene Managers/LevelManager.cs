using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using UnityEngine;

namespace CaptainPinkTurd.Scene.Manager
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private int nextLevel;
        [SerializeField] private VoidEvent onRestart;

        public void NextLevel()
        {
            SceneController.Instance
                .NewTransition()
                .Load(SceneDatabase.Slots.SessionContent, $"{SceneDatabase.Scenes.Level} {nextLevel}", true)
                .WithOverlay()
                .Perform();
        }

        public void Restart()
        {
            onRestart.Raise();
            
            SceneController.Instance
                .NewTransition()
                .Load(SceneDatabase.Slots.SessionContent, $"{SceneDatabase.Scenes.Level} {1}", true)
                .WithOverlay()
                .Perform();
        }
        public void EndSession()
        {
            onRestart.Raise(); //putting this here cause the GameManager carry the player Unit hp 
            
            SceneController.Instance
                .NewTransition()
                .Load(SceneDatabase.Slots.Menu, SceneDatabase.Scenes.MainMenu, true)
                .Unload(SceneDatabase.Slots.Session)
                .Unload(SceneDatabase.Slots.SessionContent)
                .WithClearUnusedAssets()
                .WithOverlay()
                .Perform();
        }
    }
}