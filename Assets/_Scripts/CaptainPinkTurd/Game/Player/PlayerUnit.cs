using System;
using CaptainPinkTurd.AnimationSystem;
using CaptainPinkTurd.Core.CustomDataStructure;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Interfaces;
using CaptainPinkTurd.Core.Struct;
using CaptainPinkTurd.Game.Enemy;
using CaptainPinkTurd.UnitSystem;
using UnityEngine;

namespace CaptainPinkTurd.Game.Player
{
    public class PlayerUnit : UnitBase
    {
        [Header("Player Unit Properties")]
        [SerializeField] private VoidEvent onPlayerDamaged;
        [SerializeField] private SerializeKeyValuePair<EColor, PlayerStateInfo>[] playerStates;
        [SerializeField] private LayerMask enemyLayers;
        [SerializeField] private BasicVfxAnimationController colorSwitchVfx;
        
        public void OnColorChangeEvents(EColor color)
        {
            if(playerStates.TryGetValue(color, out var playerState))
            {
                foreach (var state in playerStates)
                {
                    state.Value.model.SetActive(false);
                }
                colorSwitchVfx.gameObject.SetActive(true);
                playerState.model.SetActive(true);

                gameObject.layer = playerState.layerValue;
                foreach (Transform child in transform)
                {
                    child.gameObject.layer = playerState.layerValue;
                }
            }
            else
            {
                Debug.LogError($"Player State for dimension {color} not found");
            }
        }

        public override void OnDamaged(SDamageData damageData)
        {
            onPlayerDamaged.Raise();
        }

        public override void OnDeath(SDamageData damageData)
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

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!enemyLayers.Contains(other.gameObject.layer)) return;
            if (!other.gameObject.TryGetComponentInHierarchy(out IDamageable enemyDamageable)) return;
            
            enemyDamageable.TakeDamage(new SDamageData(1, gameObject));
        }
    }

    [Serializable]
    public struct PlayerStateInfo
    {
        public GameObject model;
        public int layerValue;
    }
}