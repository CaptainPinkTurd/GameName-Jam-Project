using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.Utilities;
using UnityEngine;

namespace CaptainPinkTurd.AnimationSystem
{
    public class BasicVfxAnimationController : AnimationControllerBase
    {
        [Header("Vfx Animation Controller Configs")]
        [SerializeField] private AnimationClip vfxAnimation;
        [SerializeField] private bool spawnFromPool;
        [SerializeField] private SoundData vfxSfx; 
        
        public override int DefaultAnimationHash { get; set; }
        
        protected override void OnEnable()
        {
            OnEnableEvents.Subscribe(PlayVfxAnimation);
            
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            OnEnableEvents.Unsubscribe(PlayVfxAnimation);
        }

        private void PlayVfxAnimation()
        {
            SoundManager.Instance.CreateSoundBuilder()
                .WithPosition(transform.position).WithRandomPitch().Play(vfxSfx);
            PlayAnimation(Animator.StringToHash(vfxAnimation.name), onAnimationEnd: () =>
            {
                if(spawnFromPool)
                {
                    ObjectPoolManager.Instance.ReturnObjectToPool(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            });
        }
    }
}