using System;
using CaptainPinkTurd.Core.CustomDataStructure;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.Core.Extensions;
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
        [SerializeField] private GameObject currentModel;
        
        private SpriteRenderer spriteRenderer;

        protected override void Awake()
        {
            base.Awake();
            
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void OnColorChangeEvents(EColor color)
        {
            if(playerStates.TryGetValue(color, out var playerState))
            {
                currentModel?.SetActive(false);
                currentModel = playerState.model;
                currentModel.SetActive(true);

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
    }

    [Serializable]
    public struct PlayerStateInfo
    {
        public GameObject model;
        public int layerValue;
    }
}