using CaptainPinkTurd.Core.DesignPattern;
using UnityEngine;
using UnityEngine.AI;

namespace CaptainPinkTurd.UnitSystem
{
    public class PatrolState : BaseState<EnemyUnitBase>
    {
        internal readonly Vector3[] waypoints = new Vector3[4];
        
        private NavMeshTriangulation triangulation;
        private Coroutine patrolCoroutine;
        private int waypointIndex = 0;
        
        public PatrolState(EnemyUnitBase stateEntity) : base(stateEntity)
        {

        }
    }
}