using UnityEngine;

namespace CaptainPinkTurd.Scene.Manager
{
    public class CoreManager : MonoBehaviour
    {
        private void Start()
        {
            //Core Setup for the game
            //Load everything like SoundManager, Save System,...
            SceneController.Instance
                .NewTransition()
                .Load(SceneDatabase.Slots.Menu, SceneDatabase.Scenes.MainMenu)
                .Perform();
        }
    }
}