using CaptainPinkTurd.Core.Struct;

namespace CaptainPinkTurd.UnitSystem
{
    public class DummyUnit : EnemyUnitBase
    {
        protected override void StateMachineSetup()
        {
            
        }

        internal override void OnDeath(SDamageData damageData)
        {
            base.OnDeath(damageData);
            
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }
}