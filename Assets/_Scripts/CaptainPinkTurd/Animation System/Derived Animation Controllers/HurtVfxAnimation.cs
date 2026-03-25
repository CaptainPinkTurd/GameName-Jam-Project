using CaptainPinkTurd.Core.Utilities;
using UnityEngine;

namespace CaptainPinkTurd.AnimationSystem
{
    public class HurtVfxAnimation : AnimationControllerBase
    {
        [SerializeField] private AnimationClip hurtVfxAnimation;
        [SerializeField] private bool disableSpriteRenderer;
        [SerializeField] private bool spawnFromPool;
        
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
            PlayAnimation(Animator.StringToHash(hurtVfxAnimation.name), onAnimationEnd: () =>
            {
                if(disableSpriteRenderer) spriteRenderer.enabled = false;
                if(spawnFromPool) ObjectPoolManager.Instance.ReturnObjectToPool(gameObject);
            });
        }
    }
}