using BulletHell;
using CaptainPinkTurd.AnimationSystem;
using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Interfaces;
using CaptainPinkTurd.Core.Utilities;
using UnityEngine;

namespace CaptainPinkTurd.Game
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class ProjectileEmitterCenter : AnimationControllerBase
    {
        [Header("Projectile Center Configs")]
        [SerializeField] private int maxHealth = 3;
        [SerializeField] private float knockbackForce = 10f;
        [SerializeField] private LayerMask damageDealerLayers;
        
        [Header("Projectile Emitter Configs")]
        [SerializeField] protected ProjectileEmitterAdvanced advancedEmitter;
        [SerializeField] private float projectileColorChangeIntervalMin = 2.5f;
        [SerializeField] private float projectileColorChangeIntervalMax = 5f;
        
        [Header("Impact Configs")]
        [SerializeField] private float hitStopDuration = 0.2f;
        [SerializeField] private ShockwaveScreen impactShockwavePrefab;
        [SerializeField] private BasicVfxAnimationController explosionVfx;
        [SerializeField] private VoidEvent OnDamagedTaken;
        
        [Header("SFXs")]
        [SerializeField] private SoundData damagedSfx;
        
        public Collider2D Coll { get; private set; }
        private int currentHealth;

        public override int DefaultAnimationHash { get; set; }

        protected override void Awake()
        {
            base.Awake();
                
            Coll = GetComponent<Collider2D>();
            Coll.isTrigger = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            currentHealth = maxHealth;

            ProjectileEmitterSetup();
        }

        private void ProjectileEmitterSetup()
        {
            advancedEmitter.PulseSpeed = Random.Range(projectileColorChangeIntervalMin, projectileColorChangeIntervalMax);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!damageDealerLayers.Contains(other.gameObject.layer) || !enabled) return;
            
            if(other.gameObject.TryGetComponentInHierarchy(out IDamageable damageable))
            {
                var knockbackDir = (other.transform.position - transform.position).normalized;
                damageable.WithKnockback(knockbackDir * knockbackForce, 0);
            }
            
            SoundManager.Instance.CreateSoundBuilder()
                .WithPosition(transform.position).WithRandomPitch().Play(damagedSfx);
            ObjectPoolManager.Instance.SpawnObject(impactShockwavePrefab.gameObject, transform.position, 
                Quaternion.identity, ObjectPoolManager.PoolType.GameObject);
            currentHealth--;
            
            HitStop.Stop(this, hitStopDuration, () =>
            {
                OnDamagedTaken.Raise();

                if (currentHealth > 0) return;
                
                ObjectPoolManager.Instance.SpawnObject(explosionVfx.gameObject, transform.position, Quaternion.identity,
                    ObjectPoolManager.PoolType.VFX);
                gameObject.SetActive(false);
            });
        }
    }
}