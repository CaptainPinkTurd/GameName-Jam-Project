using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.BulletHell;
using CaptainPinkTurd.Core.CustomDataStructure;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Utils;
using UnityEngine;

namespace CaptainPinkTurd.Game.Enemy
{
    public class Turret : BiformisEmitterController
    {
        [Header("Turret Animations")]
        [SerializeField] private SerializeKeyValuePair<EColor, AnimationClip>[] idleAnimationClips;
        [SerializeField] private SerializeKeyValuePair<EColor, AnimationClip>[] spawnAnimationClips;
        
        private AnimationClip currentIdleAnim;
        private AnimationClip currentSpawnAnim;
        
        private bool isSpawning;

        protected override void Awake()
        {
            base.Awake();

            ToggleEmitter(false);
        }

        protected override void OnEnable()
        {
            isSpawning = true;
            
            base.OnEnable();

            StartCoroutine(CoroutineUtils.WaitForCondition(() => SoundManager.Instance.didAwake,
                () =>
                {
                    SoundManager.Instance.CreateSoundBuilder()
                        .WithPosition(transform.position).WithRandomPitch().Play(startUpSfx);
                }));
            
            PlayAnimation(Animator.StringToHash(currentSpawnAnim.name),
                onAnimationEnd: () =>
                {
                    isSpawning = false;
                    Coll.isTrigger = true;
                    ToggleEmitter(true);
                    PlayAnimation(Animator.StringToHash(currentIdleAnim.name));
                });
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            ToggleEmitter(false);
            currentIdleAnim = null;
            currentSpawnAnim = null;
        }

        protected override void OnColorChangeEvent(EColor color)
        {
            base.OnColorChangeEvent(color);

            if (spawnAnimationClips.TryGetValue(color, out var spawnAnim))
            {
                currentSpawnAnim = spawnAnim;
            }
            else
            {
                Debug.LogError($"No spawn animation found for color: {color}");
            }
            if (idleAnimationClips.TryGetValue(color, out var idleAnim))
            {
                currentIdleAnim = idleAnim;

                if (!didStart || isSpawning) return;
                PlayAnimation(Animator.StringToHash(currentIdleAnim.name));
            }
            else
            {
                Debug.LogError($"No idle animation found for color: {color}");
            }
        }
    }
}