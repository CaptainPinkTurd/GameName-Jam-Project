using UnityEngine;

namespace CaptainPinkTurd.Scene.Manager
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private int initialLevel = 1;
        public void StartSession()
        {
            SceneController.Instance
                .NewTransition()
                .Load(SceneDatabase.Slots.Session, SceneDatabase.Scenes.Session)
                .Load(SceneDatabase.Slots.SessionContent, $"{SceneDatabase.Scenes.Level} {initialLevel}", true)
                .Unload(SceneDatabase.Slots.Menu)
                .WithOverlay()
                .Perform();
        }
    }
}