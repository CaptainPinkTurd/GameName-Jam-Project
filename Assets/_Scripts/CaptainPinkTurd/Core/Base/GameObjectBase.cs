using CaptainPinkTurd.Core.CustomDataStructure;
using CaptainPinkTurd.Core.Enum;
using UnityEngine;
using UnityEngine.Events;

namespace CaptainPinkTurd.Core.Base
{
    public abstract class GameObjectBase : MonoBehaviour
    {
        [Header("Game Object Base Events")]
        [SerializeField] private UnityEvent onAwakeEvents;
        [SerializeField] private SerializeKeyValuePair<EPriority, UnityEvent> onEnableEvents;
        [SerializeField] private SerializeKeyValuePair<EPriority, UnityEvent> onDisableEvents;
        [SerializeField] private UnityEvent onStartEvents;
        
        public GameEvent OnEnableEvents = new GameEvent();
        [Tooltip("Remember to manually clear this before you subscribe")]
        public GameEvent OnDisableEvents = new GameEvent();
        
        protected virtual void Awake()
        {
            onAwakeEvents.Invoke();
            
            OnEnableEvents.Subscribe(onEnableEvents.Value.Invoke, onEnableEvents.Key);
            OnDisableEvents.Subscribe(onDisableEvents.Value.Invoke, onDisableEvents.Key);
        }
        protected virtual void OnEnable()
        {
            OnEnableEvents.Raise();
        }

        protected virtual void OnDisable()
        {
            OnDisableEvents.Raise();
        }

        protected virtual void Start()
        {
            onStartEvents.Invoke();
        }
    }
}