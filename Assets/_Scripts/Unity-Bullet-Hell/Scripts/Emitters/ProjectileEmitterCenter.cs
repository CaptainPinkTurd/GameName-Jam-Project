using BulletHell;
using CaptainPinkTurd.AnimationSystem;
using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core;
using CaptainPinkTurd.Core.CustomDataStructure;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Interfaces;
using CaptainPinkTurd.Core.Struct;
using CaptainPinkTurd.Core.Utilities;
using CaptainPinkTurd.ScoreSystem;
using CaptainPinkTurd.UI.Popup;
using UnityEngine;
using UnityEngine.Events;

namespace CaptainPinkTurd.BulletHell
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class ProjectileEmitterCenter : AnimationControllerBase, IDamageable, IScorable
    {
        [Header("Projectile Center Configs")]
        [SerializeField] private int maxHealth = 3;
        [SerializeField] private float knockbackForce = 10f;
        [SerializeField] private PopupText scorePopup;
        [SerializeField] private float scoreTextTargetHeight = 1.5f;
        [SerializeField] private ScoreConfig scoreConfig;
        [SerializeField] private bool spawnFromPool = true;
        
        [Header("Projectile Emitter Configs")]
        [SerializeField] protected ProjectileEmitterAdvanced advancedEmitter;
        [SerializeField] private float projectileColorChangeIntervalMin = 2.5f;
        [SerializeField] private float projectileColorChangeIntervalMax = 5f;
        [SerializeField] private GameObject alertModel;
        [SerializeField] private SerializeKeyValuePair<EColor, GameObject>[] colorAlertModels;
        
        [Header("Impact Configs")]
        [SerializeField] private float hitStopDuration = 0.2f;
        [SerializeField] private ShockwaveScreen impactShockwavePrefab;
        [SerializeField] private BasicVfxAnimationController explosionVfx;
        [SerializeField] private UnityEvent OnDamagedTaken;
        
        [Header("SFXs")]
        [SerializeField] protected SoundData startUpSfx;
        [SerializeField] private SoundData damagedSfx;
        [SerializeField] private SoundData colorChangeAlertSfx;

        private GameObject damageSource;
        public Collider2D Coll { get; private set; }
        
        public int CurrentHealth { get; private set; }
        public int MaxHealth => maxHealth;
        public GameEvent<SDamageData> OnTakeDamage { get; } = new GameEvent<SDamageData>();
        public GameEvent<SDamageData> OnDeath { get; } = new GameEvent<SDamageData>();

        public ScoreConfig ScoreConfig => scoreConfig;

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
            
            alertModel.SetActive(false);
            advancedEmitter.OnProjectilePulseChangeAlert.Subscribe(OnProjectileColorChangeAlert);
            advancedEmitter.OnProjectileColorPulseChange.Subscribe(OnColorChangeEvent);
            
            CurrentHealth = maxHealth;

            ProjectileEmitterSetup();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            advancedEmitter.OnProjectilePulseChangeAlert.Unsubscribe(OnProjectileColorChangeAlert);
            advancedEmitter.OnProjectileColorPulseChange.Unsubscribe(OnColorChangeEvent);
        }

        private void OnColorChangeEvent(EColor color)
        {
            alertModel.SetActive(false);
            foreach (var colorModel in colorAlertModels)
            {
                colorModel.Value.SetActive(colorModel.Key == color);
            }
        }

        private void OnProjectileColorChangeAlert()
        {
            alertModel.SetActive(true);
            SoundManager.Instance.CreateSoundBuilder()
                .WithPosition(transform.position).WithRandomPitch().Play(colorChangeAlertSfx,
                    () => alertModel.SetActive(false));
        }

        protected void ToggleEmitter(bool on)
        {
            advancedEmitter.enabled = on;
            advancedEmitter.AutoFire = on;
        }
        private void ProjectileEmitterSetup()
        {
            advancedEmitter.PulseSpeed = Random.Range(projectileColorChangeIntervalMin, projectileColorChangeIntervalMax);
            advancedEmitter.OutlinePulseSpeed = advancedEmitter.PulseSpeed;

            if (!advancedEmitter.UseFollowTarget) return;
            
            var target = FindAnyObjectByType<Target>();
            if (!target)
            {
                Debug.LogError("No target found in scene");
                return;
            }
            advancedEmitter.Target = target.transform;
        }
        
        public void TakeDamage(SDamageData damageData)
        {
            if (damageSource) return;
            
            damageSource = damageData.Source;
            if(damageData.Source.TryGetComponentInHierarchy(out IDamageable damageable))
            {
                var knockbackDir = (damageData.Source.transform.position - transform.position).normalized;
                damageable.WithKnockback(knockbackDir * knockbackForce, 0);
            }
            
            SoundManager.Instance.CreateSoundBuilder()
                .WithPosition(transform.position).WithRandomPitch().Play(damagedSfx);
            ObjectPoolManager.Instance.SpawnObject(impactShockwavePrefab.gameObject, transform.position, 
                Quaternion.identity, ObjectPoolManager.PoolType.VFX);
            //multiplier is equal to enemy current health before they died
            if(ScoreConfig.useMultiplier) ScoreConfig.runtimeMultiplier = CurrentHealth;
            CurrentHealth -= damageData.Amount;
            
            HitStop.Stop(hitStopDuration, () =>
            {
                damageSource = null;
                OnDamagedTaken.Invoke();

                if (CurrentHealth > 0) return;
                
                ScoreManager.Instance.AddScore(this);
                ObjectPoolManager.Instance.SpawnObject(explosionVfx.gameObject, transform.position, Quaternion.identity,
                    ObjectPoolManager.PoolType.VFX);
                if (spawnFromPool)
                {
                    ObjectPoolManager.Instance.ReturnObjectToPool(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            });
        }
        public Transform GetTransform() => transform;
        public void OnScored()
        {
            scorePopup.InitializeText(ScoreConfig.GetFinalScore().ToString(), transform.position, scoreTextTargetHeight);
        }
    }
}