using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.BulletHell;
using CaptainPinkTurd.Core.Utils;
using UnityEngine;

namespace CaptainPinkTurd.Game.Enemy
{
    public class Turret : BiformisEmitterController
    {
        [Header("Turret Animations")]
        [SerializeField] private AnimationClip idleAnimationClip;
        [SerializeField] private AnimationClip spawnAnimationClip;

        protected override void Awake()
        {
            base.Awake();

            ToggleEmitter(false);
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
                    ToggleEmitter(true);
                });
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            ToggleEmitter(false);
        }
    }
}