using CaptainPinkTurd.Core.Attributes;
using UnityEngine;

namespace CaptainPinkTurd.EffectSystem.ShakeEffect
{
    [CreateAssetMenu(fileName = "GameObjectShakeProfile", menuName ="Scriptable Objects/Shake Profiles/GameObject Shake Profile")]
    public class GameObjectShakeProfile : ScriptableObject
    {
        public bool useWithHitStop;
        [ShowIf("useWithHitStop")] public float hitStopDuration;
        public float shakeDuration;
        [Tooltip("The shake strength on each axis")] 
        public Vector3 shakeStrength;
        [Tooltip("Indicates how much will the shake vibrate")]
        public int vibration = 10;
        [Tooltip("Indicates how much the shake will be random (0 to 180 - values higher than 90 kind of suck, so beware). Setting it to 0 will shake along a single direction.")]
        public float randomness = 90f;
        [Tooltip("If TRUE the tween will smoothly snap all values to integers")]
        public bool snapping = false;
        [Tooltip("If TRUE the shake will automatically fadeOut smoothly within the tween's duration, otherwise it will not")]
        public bool fadeOut = true;
    }
}
