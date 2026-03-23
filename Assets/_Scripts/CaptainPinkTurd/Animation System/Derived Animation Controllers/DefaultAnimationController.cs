using CaptainPinkTurd.Core.Utilities;
using UnityEngine;

namespace CaptainPinkTurd.AnimationSystem
{
    public class DefaultAnimationController : AnimationControllerBase
    {
        [Header("Default Animation Controller Properties")]
        [SerializeField] private bool spawnFromPool;
        
        public override int DefaultAnimationHash { get; set; }
        
        public void SetAnimation(AnimationClip animationClip, bool disableSpriteRenderer = true)
        {
            //Debug.Log($"Playing VFX: {vfxClip.name}");
            spriteRenderer.enabled = true;
            PlayAnimation(Animator.StringToHash(animationClip.name),
                0, null, () =>
                {
                    if(disableSpriteRenderer) spriteRenderer.enabled = false;
                    if(spawnFromPool) ObjectPoolManager.Instance.ReturnObjectToPool(gameObject);
                }, false, true);
        }
    }
}
