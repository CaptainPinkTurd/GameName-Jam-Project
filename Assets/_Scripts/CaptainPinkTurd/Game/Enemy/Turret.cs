using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.Utils;
using UnityEngine;

namespace CaptainPinkTurd.Game.Enemy
{
    public class Turret : ProjectileEmitterCenter
    {
        [Header("Turret Animations")]
        [SerializeField] private AnimationClip idleAnimationClip;
        [SerializeField] private AnimationClip spawnAnimationClip;

        protected override void Awake()
        {
            base.Awake();
            
            advancedEmitter.gameObject.SetActive(false);
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
                onAnimationEnd: () =>
                {
                    Coll.isTrigger = true;
                    advancedEmitter.gameObject.SetActive(true);
                });
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            advancedEmitter.gameObject.SetActive(false);
        }
    }
}