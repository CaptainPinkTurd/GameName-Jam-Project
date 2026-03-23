using CaptainPinkTurd.Core.DesignPattern;

namespace CaptainPinkTurd.UnitSystem
{
    public class AttackState : BaseState<EnemyUnitBase>
    {
        public AttackState(EnemyUnitBase stateEntity) : base(stateEntity)
        {
            
        }
    }
}