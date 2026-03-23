using UnityEngine;

namespace CaptainPinkTurd.Core.DesignPattern
{
    public abstract class BaseState<T> : IState where T : MonoBehaviour
    {
        protected readonly T StateEntity;

        protected BaseState(T stateEntity)
        {
            StateEntity = stateEntity;
        }

        public virtual void OnEnter()
        {
            //Debug.Log($"Entering {GetType().Name}");
        }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void OnExit() { }
    }
}