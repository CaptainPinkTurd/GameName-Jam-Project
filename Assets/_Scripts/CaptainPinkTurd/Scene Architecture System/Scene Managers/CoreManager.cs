using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.Utils;
using UnityEngine;

namespace CaptainPinkTurd.Scene.Manager
{
    public class CoreManager : MonoBehaviour
    {
        private void Start()
        {
            //Core Setup for the game
            //Load everything like SoundManager, Save System,...
            StartCoroutine(CoroutineUtils.WaitForCondition(() => SoundManager.Instance.didAwake,
                () =>
                {
                    SceneController.Instance
                        .NewTransition()
                        .Load(SceneDatabase.Slots.Menu, SceneDatabase.Scenes.MainMenu)
                        .Perform();
                }));
        }
    }
}