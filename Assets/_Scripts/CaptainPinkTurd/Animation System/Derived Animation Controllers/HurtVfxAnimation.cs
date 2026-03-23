using UnityEngine;

namespace CaptainPinkTurd.AnimationSystem
{
    public class HurtVfxAnimation : DefaultAnimationController
    {
        [SerializeField] private AnimationClip hurtVfxAnimation;
        
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
            SetAnimation(hurtVfxAnimation);
        }
    }
}
