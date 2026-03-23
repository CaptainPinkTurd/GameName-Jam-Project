using CaptainPinkTurd.Core;
using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.EffectSystem.KnockbackEffect;
using CaptainPinkTurd.Core.Base;
using CaptainPinkTurd.Core.Struct;
using UnityEngine;

namespace CaptainPinkTurd.UnitSystem
{
    public abstract class UnitBase : GameObjectBase
    {
        [Header("Unit Base Config")] 
        [SerializeField][InlineScriptableObject] internal UnitInfo unitInfo;
        [SerializeField] internal UnitHealth unitHealth;

        public Knockback Knockback;
        
        public Rigidbody2D rb { get; private set; }
        public GameEvent OnDamageableKill { get; } = new GameEvent();
        
        protected override void Awake()
        {
            base.Awake();
            
            rb = GetComponent<Rigidbody2D>();
            Knockback = new Knockback(rb);
        }

        protected virtual void Start()
        {
            unitHealth.OnDeath.Subscribe(OnDeath);
        }

        internal abstract void OnDamaged(SDamageData damageData);
        internal abstract void OnDeath(SDamageData damageData);
    }
}