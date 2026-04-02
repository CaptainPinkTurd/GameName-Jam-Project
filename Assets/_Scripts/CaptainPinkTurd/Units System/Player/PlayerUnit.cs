using System;
using CaptainPinkTurd.Core.CustomDataStructure;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using CaptainPinkTurd.Core.Enum;
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
        [SerializeField] private SerializeKeyValuePair<EDimension, PlayerStateInfo>[] playerStates;

        private SpriteRenderer spriteRenderer;

        protected override void Awake()
        {
            base.Awake();
            
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void OnDimensionChangeEvents(EDimension dimension)
        {
            if(playerStates.TryGetValue(dimension, out var playerState))
            {
                spriteRenderer.sprite = playerState.sprite;

                gameObject.layer = playerState.layerValue;
                foreach (Transform child in transform)
                {
                    child.gameObject.layer = playerState.layerValue;
                }
            }
            else
            {
                Debug.LogError($"Player State for dimension {dimension} not found");
            }
        }

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

    [Serializable]
    public struct PlayerStateInfo
    {
        public Sprite sprite;
        public int layerValue;
    }
}