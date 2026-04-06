using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core;
using CaptainPinkTurd.Core.Attributes;
using UnityEngine;
using CaptainPinkTurd.Core.DesignPattern.Singleton;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using CaptainPinkTurd.Core.Enum;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CaptainPinkTurd.Game
{
    public class GameManager : Singleton<GameManager>
    {
        [Header("GameManager Properties")]
        [SerializeField] private float gameFastForwardSpeed = 2;
        [SerializeField] private UnityEvent onGameOver;
        
        [Header("Game Specific Variables")]
        [SerializeField] private EColorEvent onDimensionChange;
        [SerializeField] private SoundData dimensionSwitchSfx;
        [SerializeField, ReadOnly] private EColor currentDimension;
            
        public GameEvent OnGameOver = new GameEvent();

        private float oldTimeScale;

        protected override void Awake()
        {
            base.Awake();
            
            switchDimensionAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/j");
        }

        private void Start()
        {
            currentDimension = EColor.Red;
            onDimensionChange.Raise(currentDimension);
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
            currentDimension = currentDimension == EColor.Red ? EColor.Blue : EColor.Red;
            SoundManager.Instance.CreateSoundBuilder().WithRandomPitch().Play(dimensionSwitchSfx);
            
            onDimensionChange.Raise(currentDimension);
        }

        #endregion
    }
}