using System;
using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.DataPersistence;
using CaptainPinkTurd.DataPersistence.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace CaptainPinkTurd.Scene.Manager
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private bool isTutorialLevel;
        
        [ShowIf(nameof(isTutorialLevel))]
        [SerializeField] private bool isLastTutorialLevel;
        [ShowIf(EConditionalLogic.And, nameof(isTutorialLevel), true, nameof(isLastTutorialLevel), false)]
        [SerializeField] private int nextTutorialLevel;
        
        [ShowIf(nameof(isTutorialLevel), false)]
        [SerializeField] private LevelData levelData;
        [ShowIf(nameof(isTutorialLevel), false)]
        [SerializeField] private VoidEvent onRestart;
        
        [SerializeField] private AudioClip levelTheme;

        private void Start()
        {
            MusicManager.Instance.Play(levelTheme, loop: true);
        }

        public void NextLevel()
        {
            if (isTutorialLevel && !levelData.hasDoneTutorial)
            {
                if (isLastTutorialLevel)
                {
                    levelData.hasDoneTutorial = true;
                    NextLevel();
                    return;
                }
                SceneController.Instance
                    .NewTransition()
                    .Load(SceneDatabase.Slots.SessionContent, $"{SceneDatabase.Scenes.Level} {nextTutorialLevel} Tutorial", true)
                    .WithOverlay()
                    .Perform();
            }
            else
            {
                int nextLevel = Random.Range(1, levelData.totalLevelInGame + 1);
            
                SceneController.Instance
                    .NewTransition()
                    .Load(SceneDatabase.Slots.SessionContent, $"{SceneDatabase.Scenes.Level} {nextLevel}", true)
                    .WithOverlay()
                    .Perform();
            }
        }

        public void Restart()
        {
            onRestart.Raise();

            if (isTutorialLevel)
            {
                SceneController.Instance
                    .NewTransition()
                    .Load(SceneDatabase.Slots.SessionContent, SceneManager.GetActiveScene().name, true)
                    .WithOverlay()
                    .Perform();
            }
            else
            {
                int restartLevel = Random.Range(1, levelData.totalLevelInGame + 1);
            
                SceneController.Instance
                    .NewTransition()
                    .Load(SceneDatabase.Slots.SessionContent, $"{SceneDatabase.Scenes.Level} {restartLevel}", true)
                    .WithOverlay()
                    .Perform();
            }
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