using CaptainPinkTurd.Core;
using UnityEngine;
using CaptainPinkTurd.Core.DesignPattern.Singleton;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CaptainPinkTurd.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private float gameFastForwardSpeed = 2;
        [SerializeField] private UnityEvent onGameOver;
            
        public GameEvent OnGameOver = new GameEvent();
        
        private float oldTimeScale;

        private void OnEnable()
        {
            OnGameOver.Subscribe(OnGameOverEvents);
        }

        private void OnDisable()
        {
            OnGameOver.Unsubscribe(OnGameOverEvents);
        }

        private void OnGameOverEvents()
        {
            onGameOver.Invoke();
        }
        public void SceneReset()
        {
            Debug.Log($"Reset scene: {SceneManager.GetActiveScene().name}");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            SetTimeScale(1); //in case Timescale got set to something else when scene reset
        }

        public void SetTimeScale(float timeScale)
        {
            oldTimeScale = Time.timeScale;
            Time.timeScale = timeScale;
        }
        public void ResetTimeScale()
        {
            Time.timeScale = oldTimeScale;
        }
        public void QuitGame()
        {
            #if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                EditorApplication.ExitPlaymode();
                return;
            }
            #endif
            
            Application.Quit();
        }

        #region GAME SPECIFIC REGION

        public GameEvent OnGunShoot = new GameEvent(); //in case more events in the future
        public void OnGunShootEvents()
        {
            Time.timeScale = 1;
            OnGunShoot.Raise();
        }

        #endregion
    }
}