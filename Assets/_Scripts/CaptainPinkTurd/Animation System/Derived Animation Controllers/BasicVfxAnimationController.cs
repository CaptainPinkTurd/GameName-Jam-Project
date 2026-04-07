using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.Utilities;
using UnityEngine;

namespace CaptainPinkTurd.AnimationSystem
{
    public class BasicVfxAnimationController : AnimationControllerBase
    {
        [Header("Vfx Animation Controller Configs")]
        [SerializeField] private AnimationClip vfxAnimation;
        [SerializeField] private bool disableSpriteRenderer;
        [SerializeField] private bool spawnFromPool;
        [SerializeField] private SoundData vfxSfx; 
        
        public override int DefaultAnimationHash { get; set; }
        
        protected override void OnEnable()
        {
            OnEnableEvents.Subscribe(SetHurtVfxAnimation);
            
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            OnEnableEvents.Unsubscribe(SetHurtVfxAnimation);
        }

        public void SetHurtVfxAnimation()
        {
            spriteRenderer.enabled = true;
            
            SoundManager.Instance.CreateSoundBuilder().WithPosition(transform.position).WithRandomPitch().Play(vfxSfx);
            PlayAnimation(Animator.StringToHash(vfxAnimation.name), onAnimationEnd: () =>
            {
                if(disableSpriteRenderer) spriteRenderer.enabled = false;
                if(spawnFromPool) ObjectPoolManager.Instance.ReturnObjectToPool(gameObject);
            });
        }
    }
}