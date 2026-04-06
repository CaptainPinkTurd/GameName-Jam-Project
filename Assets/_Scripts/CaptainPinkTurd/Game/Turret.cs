using BulletHell;
using CaptainPinkTurd.AnimationSystem;
using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.Utils;
using UnityEngine;

namespace CaptainPinkTurd.Game
{
    public class Turret : AnimationControllerBase
    {
        [Header("Turret Configs")]
        [SerializeField] private ProjectileEmitterBase projectileEmitter;
        [SerializeField] private SoundData startUpSfx;
        
        [Header("Turret Animations")]
        [SerializeField] private AnimationClip idleAnimationClip;
        [SerializeField] private AnimationClip spawnAnimationClip;
        
        public override int DefaultAnimationHash { get; set; }

        protected override void Awake()
        {
            base.Awake();
            
            projectileEmitter.gameObject.SetActive(false);
            DefaultAnimationHash = Animator.StringToHash(idleAnimationClip.name);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            StartCoroutine(CoroutineUtils.WaitForCondition(() => SoundManager.Instance.didAwake,
                () =>
                {
                    SoundManager.Instance.CreateSoundBuilder()
                        .WithPosition(transform.position).WithRandomPitch().Play(startUpSfx);
                }));
            
            PlayAnimation(Animator.StringToHash(spawnAnimationClip.name),
                onAnimationEnd: () => projectileEmitter.gameObject.SetActive(true));
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            projectileEmitter.gameObject.SetActive(false);
        }
    }
}