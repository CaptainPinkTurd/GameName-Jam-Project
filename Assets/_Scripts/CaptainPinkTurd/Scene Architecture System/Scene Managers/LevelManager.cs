using UnityEngine;

namespace CaptainPinkTurd.Scene.Manager
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private int nextLevel;

        public void NextLevel()
        {
            SceneController.Instance
                .NewTransition()
                .Load(SceneDatabase.Slots.SessionContent, $"{SceneDatabase.Scenes.Level} {nextLevel}", true)
                .WithOverlay()
                .Perform();
        }
        public void EndSession()
        {
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