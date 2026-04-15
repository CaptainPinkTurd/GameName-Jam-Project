using System;
using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CaptainPinkTurd.Scene.Manager
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private bool randomizedInitialLevel;
        
        [ShowIf(nameof(randomizedInitialLevel), false)]
        [SerializeField] private int initialLevel = 1;
        [ShowIf(nameof(randomizedInitialLevel))]
        [SerializeField] private LevelData levelData;
        
        [SerializeField] private AudioClip menuMusic;
        
        private void OnEnable()
        {
            Cursor.visible = true;
        }
        private void OnDisable()
        {
            Cursor.visible = false;
        }
        private void Start()
        {
            levelData.hasDoneTutorial = PlayerPrefs.GetInt("HasDoneTutorial", 0) == 1;
            MusicManager.Instance.Play(menuMusic, loop: true);
        }

        public void StartSession()
        {
            if (!levelData.hasDoneTutorial)
            {
                SceneController.Instance
                    .NewTransition()
                    .Load(SceneDatabase.Slots.Session, SceneDatabase.Scenes.Session)
                    .Load(SceneDatabase.Slots.SessionContent, $"{SceneDatabase.Scenes.Level} 1 Tutorial", true)
                    .Unload(SceneDatabase.Slots.Menu)
                    .WithOverlay()
                    .Perform();
            }
            else
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
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}