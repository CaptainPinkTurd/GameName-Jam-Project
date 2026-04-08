using CaptainPinkTurd.Core.Struct;

namespace CaptainPinkTurd.Game.Enemy
{
    public class DummyUnit : EnemyUnitBase
    {
        protected override void StateMachineSetup()
        {
            
        }

        public override void OnDeath(SDamageData damageData)
        {
            base.OnDeath(damageData);
            
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }
}