using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core;
using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Interfaces;
using CaptainPinkTurd.Core.Struct;
using CaptainPinkTurd.Core.Utils;
using CaptainPinkTurd.EffectSystem.ImpactEffect;
using CaptainPinkTurd.UI.Popup;
using CaptainPinkTurd.UI.SliderUI;
using DG.Tweening;
using UnityEngine;

namespace CaptainPinkTurd.UnitSystem
{
    public class UnitHealth : MonoBehaviour, IDamageable
    {
        [Header("Unit Health Configs")]
        [SerializeField] private UnitBase unit;
        [SerializeField] private int maxHealth;
        [SerializeField] private float invincibilityFrameDuration;
        [SerializeField] private bool invincibilityFrameAffectKnockback = true;
        [SerializeField] private ImpactType onDamageImpactType;
        
        [Header("Health Bar Configs")]
        [SerializeField] private bool hasHealthBar;
        
        [ShowIf(nameof(hasHealthBar))]
        [SerializeField] private SliderController healthBar;
        [ShowIf(nameof(hasHealthBar), false)]
        [SerializeField, ReadOnly] private int currentHealth;
        
        [Header("Damage Text Popup Configs")]
        [SerializeField] private bool useDamageTextPopup;
        [ShowIf(nameof(useDamageTextPopup))]
        [SerializeField] private PopupText damageTextPopupPrefab;
        [ShowIf(nameof(useDamageTextPopup))]
        [SerializeField] private float damageTextTargetHeight;
        
        [Header("SFX Configs")]
        [SerializeField] private SoundData hurtSfx;
        [SerializeField] private SoundData deathSfx;

        private bool deathEventsHasBeenRaised; //prevent duplicate
        
        public bool IsInvincibilityFrameOn { get; private set; }
        public int CurrentHealth { get => currentHealth; private set => currentHealth = value; }
        public int MaxHealth { get => maxHealth; private set => maxHealth = value; }
        public GameEvent<SDamageData> OnTakeDamage { get; private set; } = new GameEvent<SDamageData>();
        public GameEvent<SDamageData> OnDeath { get; private set; } = new GameEvent<SDamageData>();

        /////////////////////////HIT EFFECT///////////////////////////////
        [Header("Hit Effect")]
        [SerializeField] private float _duration = 0.25f;
        private int _hitEffectAmount = Shader.PropertyToID("_HitEffectAmount");
        private SpriteRenderer[] _spriteRenderers;
        private Material[] _materials;
        private float _lerpAmount;
        
        private void Awake()
        {
            // Grab all SpriteRenderers from children (red / blue character)
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

            _materials = new Material[_spriteRenderers.Length];
            for (int i = 0; i < _materials.Length; i++)
            {
                _materials[i] = _spriteRenderers[i].material;
            }
        }
        /////////////////////////HIT EFFECT///////////////////////////////
        private void OnEnable()
        {
            OnTakeDamage.Subscribe(unit.OnDamaged);
            OnDeath.Subscribe(unit.OnDeath);
            
            deathEventsHasBeenRaised = false;
            IsInvincibilityFrameOn = false;
            CurrentHealth = MaxHealth;
            
            StartCoroutine(CoroutineUtils.WaitForCondition(() => !hasHealthBar || healthBar?.Slider, 
                () => HealthBarUpdate(MaxHealth)));
        }
        private void OnDisable()
        {
            OnTakeDamage.Unsubscribe(unit.OnDamaged);
            OnDeath.Unsubscribe(unit.OnDeath);
        }

        public Transform GetTransform() => transform;

        public void TakeDamage(SDamageData damageData)
        {
            //if damage is healing then ignore invincibility frame
            if(IsInvincibilityFrameOn && damageData.Amount > 0) return; 

            TriggerInvincibilityFrame();

            //Debug.Log($"{gameObject.name} took {damageData.Amount} damage from {damageData.Source.name}");
            var tempHealth = CurrentHealth - damageData.Amount;
            CurrentHealth = Mathf.Clamp(tempHealth, 0, MaxHealth);

            // Trigger hit effect when damage is taken
            if (damageData.Amount > 0 )
            {
                PlayHitEffect();
            }

            if (useDamageTextPopup)
            {
                damageTextPopupPrefab.InitializeText(damageData.Amount.ToString(),
                    gameObject.transform.position, damageTextTargetHeight);
            }
            
            HealthBarUpdate(CurrentHealth);
            HandleDamageEvents(damageData);
        }

        private void HandleDamageEvents(SDamageData damageData)
        {
            if (damageData.Amount != 0)
            {
                SoundManager.Instance.CreateSoundBuilder()
                    .WithPosition(gameObject.transform.position)
                    .WithRandomPitch().Play(hurtSfx);
                
                OnTakeDamage.Raise(damageData); 
            }

            if (CurrentHealth > 0 || damageData.Amount == 0 || deathEventsHasBeenRaised) return;
            
            deathEventsHasBeenRaised = true;
            
            SoundManager.Instance.CreateSoundBuilder()
                .WithPosition(gameObject.transform.position)
                .WithRandomPitch().Play(deathSfx);
                
            OnDeath.Raise(damageData);
        }
        private void HealthBarUpdate(int healthValue)
        {
            if (!hasHealthBar)  return;

            if (healthBar.HasText)
            {
                healthBar.SliderUpdate(healthValue, 0, MaxHealth, false,
                    ((float)CurrentHealth)
                    .Remap(0, MaxHealth, healthBar.Slider.minValue, healthBar.Slider.maxValue),
                    healthBar.Slider.maxValue);
            }
            else
            {
                healthBar.SliderUpdate(healthValue, 0, MaxHealth);
            }
        }

        public IDamageable WithKnockback(Vector3 knockbackForce, float constantForce)
        {
            if (!IsInvincibilityFrameOn || !invincibilityFrameAffectKnockback)
            {
                StartCoroutine(unit.Knockback.KnockbackAction(unit.unitInfo.defaultKnockbackConfig,
                    knockbackForce, constantForce, Vector2.zero));
            }
            return this;
        }

        public IDamageable WithSurfaceImpact(Vector3 hitPoint, Vector3 hitNormal)
        {
            SurfaceManager.Instance.HandleImpact(gameObject, 
                hitPoint, hitNormal, onDamageImpactType, 0);
            return this;
        }
        
        private void TriggerInvincibilityFrame()
        {
            IsInvincibilityFrameOn = true;

            if (!gameObject.activeInHierarchy) return;
            
            StartCoroutine(CoroutineUtils.WaitForSeconds(invincibilityFrameDuration, 
                () => IsInvincibilityFrameOn = false));
        }

        /////////////////////////HIT EFFECT///////////////////////////////
        public void PlayHitEffect()
        {
            _lerpAmount = 0f;
            DOTween.To(GetLerpValue, SetLerpValue, 1f, _duration)
                .SetEase(Ease.OutExpo)
                .OnUpdate(OnLerpUpdate)
                .OnComplete(OnLerpComplete);
        }

        private float GetLerpValue()
        {
            return _lerpAmount;
        }

        private void SetLerpValue(float newValue)
        {
            _lerpAmount = newValue;
        }

        private void OnLerpUpdate()
        {
            // Apply effect to all child materials
            for (int i = 0; i < _materials.Length; i++)
            {
                _materials[i].SetFloat(_hitEffectAmount, GetLerpValue());
            }
        }

        private void OnLerpComplete()
        {
            // Fade back down
            DOTween.To(GetLerpValue, SetLerpValue, 0f, _duration)
                .OnUpdate(OnLerpUpdate);
        }
    }
}