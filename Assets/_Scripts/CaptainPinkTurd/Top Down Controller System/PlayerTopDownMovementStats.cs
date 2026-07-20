using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core;
using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.SO;
using UnityEngine;

namespace CaptainPinkTurd.TopDownControllerSystem
{
    [CreateAssetMenu(fileName = "PlayerTopDownMovementStats", menuName = "Scriptable Objects/Player Movement Stats/Topdown")]
    public class PlayerTopDownMovementStats : RuntimeScriptableObject
    {
        [Header("Speed Configuration")]
        [Range(0.1f, 50f)] public float walkSpeed = 8f;
        [Range(0.1f, 50f)] public float runSpeed = 16f;

        [Header("Movement Smoothing")]
        public bool smoothMovement = false;
        [ShowIf(nameof(smoothMovement))]
        [Range(0.01f, 1f)] public float smoothTime = 0.08f;

        [Header("Dash")]
        public bool canDash = true;
        
        [Tooltip("Speed applied during dash.")]
        [ShowIf(nameof(canDash))]
        [Range(1f, 100f)] public float dashSpeed = 25f;

        [Tooltip("How long the dash lasts (seconds).")]
        [ShowIf(nameof(canDash))]
        [Range(0f, 1f)] public float dashDuration = 0.25f;
        
        [Tooltip("Speed scale across the dash's normalized lifetime (x: 0→1 time, y: speed multiplier). " +
                 "Only shapes the easing - total dash distance is preserved regardless of curve shape.")]
        [ShowIf(nameof(canDash))]
        public AnimationCurve dashSpeedCurve = new AnimationCurve(
            new Keyframe(0f, 1f), new Keyframe(1f, 0f));   // burst then ease-out to a clean stop

        [Tooltip("Cooldown before dashing again.")]
        [ShowIf(nameof(canDash))]
        [Range(0f, 2f)] public float dashCooldown = 0.5f;

        [Tooltip("Minimum input magnitude needed to dash.")]
        [ShowIf(nameof(canDash))]
        [Range(0f, 1f)] public float dashInputThreshold = 0.15f;

        [ShowIf(nameof(canDash))]
        public SoundData dashSfx;

        internal GameEvent OnValueChange;
        
        private void OnValidate()
        {
            // Prevent running from being slower than walking
            if (runSpeed < walkSpeed) runSpeed = walkSpeed;

            OnValueChange?.Raise();
        }

        protected override void OnReset()
        {
            OnValueChange = new GameEvent();
        }
    }
}