using CaptainPinkTurd.Core.Attributes;
using UnityEngine;

namespace CaptainPinkTurd.TopDownController2D
{
    [CreateAssetMenu(fileName = "PlayerTopDownMovementStats", menuName = "Scriptable Objects/Player Movement Stats/Topdown")]
    public class PlayerTopDownMovementStats : ScriptableObject
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

        [Tooltip("Cooldown before dashing again.")]
        [ShowIf(nameof(canDash))]
        [Range(0f, 2f)] public float dashCooldown = 0.5f;

        [Tooltip("Minimum input magnitude needed to dash.")]
        [ShowIf(nameof(canDash))]
        [Range(0f, 1f)] public float dashInputThreshold = 0.15f;
        
        private void OnValidate()
        {
            // Prevent running from being slower than walking
            if (runSpeed < walkSpeed) runSpeed = walkSpeed;
        }
    }
}