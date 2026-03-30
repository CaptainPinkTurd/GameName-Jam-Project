using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Struct;
using CaptainPinkTurd.Game;
using UnityEngine;

namespace CaptainPinkTurd.UnitSystem
{
    public class PlayerUnit : UnitBase
    {
        [Header("Player Unit Properties")]
        [SerializeField] private VoidEvent onPlayerDamaged;
        
        internal override void OnDamaged(SDamageData damageData)
        {
            onPlayerDamaged.Raise();
        }

        internal override void OnDeath(SDamageData damageData)
        {
            StopAllCoroutines();
            
            var source = damageData.Source;
            if (source.TryGetComponentInHierarchy(out EnemyUnitBase enemyUnit))
            {
                enemyUnit.OnDamageableKill.Raise();
            }
            
            gameObject.SetActive(false);
            
            //order matter for this one cause the game over popup needs to be enabled first to get the high score
            GameManager.Instance.OnGameOver.Raise(); 
        }
    }
}