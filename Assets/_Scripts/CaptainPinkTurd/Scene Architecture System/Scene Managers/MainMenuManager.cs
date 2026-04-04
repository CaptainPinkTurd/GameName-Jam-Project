using UnityEngine;

namespace CaptainPinkTurd.Scene.Manager
{
    public class MainMenuManager : MonoBehaviour
    {
        public void StartSession()
        {
            SceneController.Instance
                .NewTransition()
                .Load(SceneDatabase.Slots.Session, SceneDatabase.Scenes.Session)
                .Load(SceneDatabase.Slots.SessionContent, SceneDatabase.Scenes.Level, true)
                .Unload(SceneDatabase.Slots.Menu)
                .WithOverlay()
                .Perform();
        }
    }
}