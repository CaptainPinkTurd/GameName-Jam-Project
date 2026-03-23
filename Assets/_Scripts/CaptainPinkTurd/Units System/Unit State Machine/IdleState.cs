using CaptainPinkTurd.Core.DesignPattern;
using UnityEngine;

namespace CaptainPinkTurd.UnitSystem
{
    public class IdleState : BaseState<EnemyUnitBase>
    {
        public float IdleTimer { get; private set; }
        public IdleState(EnemyUnitBase stateEntity) : base(stateEntity)
        {

        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            IdleTimer = Random.Range(1f, 3f);
        }

        public override void Update()
        {
            base.Update();
            
            IdleTimer -= Time.deltaTime;
        }
    }
}