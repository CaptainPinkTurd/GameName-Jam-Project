using CaptainPinkTurd.Core;
using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.EffectSystem.KnockbackEffect;
using CaptainPinkTurd.Core.Base;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using CaptainPinkTurd.Core.Struct;
using UnityEngine;

namespace CaptainPinkTurd.UnitSystem
{
    public abstract class UnitBase : GameObjectBase
    {
        [Header("Unit Base Config")] 
        [SerializeField][InlineScriptableObject] internal UnitInfo unitInfo;
        [SerializeField] internal UnitHealth unitHealth;
        [SerializeField] private BoolEvent onKnockback; 

        public Knockback Knockback;
        
        public Rigidbody2D rb { get; private set; }
        public GameEvent OnDamageableKill { get; } = new GameEvent();
        
        protected override void Awake()
        {
            base.Awake();
            
            rb = GetComponent<Rigidbody2D>();
            Knockback = new Knockback(rb);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Knockback.OnKnockback.Subscribe(OnKnockbackEvents);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Knockback.OnKnockback.Unsubscribe(OnKnockbackEvents);
        }

        protected virtual void Start()
        {
            unitHealth.OnDeath.Subscribe(OnDeath);
        }
        private void OnKnockbackEvents(bool isKnockback)
        {
            onKnockback.Raise(isKnockback);
        }
        internal abstract void OnDamaged(SDamageData damageData);
        internal abstract void OnDeath(SDamageData damageData);
    }
}