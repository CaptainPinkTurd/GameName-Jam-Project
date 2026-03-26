using CaptainPinkTurd.Core;
using UnityEngine;
using CaptainPinkTurd.Core.DesignPattern.Singleton;
using UnityEngine.Events;
using UnityEngine.InputSystem;
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

        protected override void Awake()
        {
            base.Awake();
            
            switchDimensionAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/j");
        }

        private void OnEnable()
        {
            switchDimensionAction.Enable();
            switchDimensionAction.performed += DimensionSwitch;
            
            OnGameOver.Subscribe(OnGameOverEvents);
        }
        private void OnDisable()
        {
            switchDimensionAction.performed -= DimensionSwitch;
            switchDimensionAction.Disable();
            
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

        private InputAction switchDimensionAction;
        private bool isLightDimension;
        private void DimensionSwitch(InputAction.CallbackContext obj)
        {
            isLightDimension = !isLightDimension;
            Camera.main.backgroundColor = isLightDimension ? Color.white : Color.black;
        }

        #endregion
    }
}