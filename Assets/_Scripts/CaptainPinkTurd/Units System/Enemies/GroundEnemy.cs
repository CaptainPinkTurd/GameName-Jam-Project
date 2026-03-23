using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.CustomPhysics.Collision;
using CaptainPinkTurd.Core.DesignPattern;
using CaptainPinkTurd.Core.Predicate;
using CaptainPinkTurd.Core.Struct;
using UnityEngine;

namespace CaptainPinkTurd.UnitSystem
{
    public class GroundEnemy : EnemyUnitBase
    {
        [Header("Ground Enemy Configs")] 
        [SerializeField] private Collider2D physicsColl;
        [SerializeField][InlineScriptableObject] private GroundCollision2DCheckScriptableObject groundCollisionCheck;
        [SerializeField][ReadOnly] internal bool isColliding = true;
        [SerializeField][ReadOnly] internal bool isGrounded = true;
        
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            isGrounded = groundCollisionCheck.IsGrounded(physicsColl);
        }

        protected override void StateMachineSetup()
        {
            StateMachine = new StateMachine();
            
            //declare states
            var idleState = new IdleState(this);
            var chaseState = new ChaseState(this);
            var patrolState = new PatrolState(this);
            var attackState = new AttackState(this);
            
            //define transitions 
            StateMachine.At(chaseState, idleState, new FuncPredicate(() => !Target));
            StateMachine.At(idleState, patrolState, new FuncPredicate(() => !Target && idleState.IdleTimer <= 0));
            StateMachine.At(idleState, chaseState, new FuncPredicate(() => Target));
            StateMachine.At(patrolState, chaseState, new FuncPredicate(() => Target));
            //StateMachine.At(chaseState, attackState, new FuncPredicate(() => attackRadius.IsInRange));
            //StateMachine.At(attackState, chaseState, new FuncPredicate(() => !attackRadius.IsInRange && Target));
            
            StateMachine.SetState(idleState);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            isColliding = true;
        }
        private void OnCollisionExit2D(Collision2D other)
        {
            isColliding = false;
        }
    }
}