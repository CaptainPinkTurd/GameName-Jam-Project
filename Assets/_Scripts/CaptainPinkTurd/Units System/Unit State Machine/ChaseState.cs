using System.Collections;
using CaptainPinkTurd.Core.DesignPattern;
using CaptainPinkTurd.Core.Extensions;
using UnityEngine;

namespace CaptainPinkTurd.UnitSystem
{
    public class ChaseState : BaseState<EnemyUnitBase>
    {
        public ChaseState(EnemyUnitBase stateEntity) : base(stateEntity)
        {
            
        }

        public override void Update()
        {
            base.Update();

            if (StateEntity.transform.IsRightSideOfTransform(StateEntity.Target.transform.position))
            {
                StateEntity.rb.linearVelocity = Vector2.right * StateEntity.speed;
            }
            else
            {
                StateEntity.rb.linearVelocity = Vector2.left * StateEntity.speed;
            }
        }
    }
}