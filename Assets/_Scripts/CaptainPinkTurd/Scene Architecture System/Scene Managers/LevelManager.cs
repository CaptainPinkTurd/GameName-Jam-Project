using UnityEngine;

namespace CaptainPinkTurd.Scene.Manager
{
    public class LevelManager : MonoBehaviour
    {
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