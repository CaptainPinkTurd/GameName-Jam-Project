using CaptainPinkTurd.Core.Attributes;
using DG.Tweening;
using UnityEngine;

namespace CaptainPinkTurd.EffectSystem.ShakeEffect
{
    public class DefaultGameObjectShaker : MonoBehaviour
    {
        [SerializeField] private GameObjectShakeProfile shakeProfile;

        [Button("Shake")]
        public virtual void Shake()
        {
            transform.DOShakePosition(
                    shakeProfile.defaultShakeDuration,
                    shakeProfile.shakeStrength,
                    shakeProfile.vibration,
                    shakeProfile.randomness,
                    shakeProfile.snapping,
                    shakeProfile.fadeOut)
                .SetUpdate(UpdateType.Normal, true)
                .OnComplete(OnShakeComplete);
        }

        protected virtual void OnShakeComplete() { }
    }
}