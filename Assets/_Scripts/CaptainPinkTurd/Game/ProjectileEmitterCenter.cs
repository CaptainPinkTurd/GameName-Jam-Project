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
    public class ProjectileEmitterCenter : MonoBehaviour
    {
        [Header("Projectile Center Configs")]
        [SerializeField] private int maxHealth = 3;
        [SerializeField] private float knockbackForce = 10f;
        [SerializeField] private LayerMask damageDealerLayers;
        
        [Header("Impact Configs")]
        [SerializeField] private float hitStopDuration = 0.2f;
        [SerializeField] private ShockwaveScreen impactShockwavePrefab;
        [SerializeField] private BasicVfxAnimationController explosionVfx;
        [SerializeField] private VoidEvent OnDamagedTaken;
        
        [Header("SFXs")]
        [SerializeField] private SoundData damagedSfx;
        [SerializeField] private SoundData explodedSfx;
        
        private Collider2D coll;
        private int currentHealth;

        private void Awake()
        {
            coll = GetComponent<Collider2D>();
            coll.isTrigger = true;
        }

        private void OnEnable()
        {
            currentHealth = maxHealth;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!damageDealerLayers.Contains(other.gameObject.layer)) return;
            
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
                
                SoundManager.Instance.CreateSoundBuilder()
                    .WithPosition(transform.position).WithRandomPitch().Play(explodedSfx);
                ObjectPoolManager.Instance.SpawnObject(explosionVfx.gameObject, transform.position, Quaternion.identity,
                    ObjectPoolManager.PoolType.VFX);
                gameObject.SetActive(false);
            });
        }
    }
}