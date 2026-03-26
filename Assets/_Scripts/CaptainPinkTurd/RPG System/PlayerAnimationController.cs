using CaptainPinkTurd.AnimationSystem;
using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.CustomDataStructure;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Variables;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.Core.Extensions;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace CaptainPinkTurd.RPG
{
    public class PlayerAnimationController : AnimationControllerBase
    {
        [Header("Player Animation Clips")] 
        [SerializeField] private SerializeKeyValuePair<EDirection2D, AnimationClip>[] idleAnimationClips; 
        [SerializeField] private SerializeKeyValuePair<EDirection2D, AnimationClip>[] walkAnimationClips; 
        
        [Header("Input Events")]
        [SerializeField] private Vector2VariableSO currentMovementInput;
        [SerializeField] private EDirectionMode directionMode;
        [SerializeField][ReadOnly] private EDirection2D playerCurrentDirectionState;
        
        private bool isMoving;
        
        public override int DefaultAnimationHash { get; set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            currentMovementInput.OnValueChanged += OnMovementInputChangeEvent;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            
            currentMovementInput.OnValueChanged -= OnMovementInputChangeEvent;
        }

        private Vector2 SnapDiagonal(Vector2 input, EDirectionMode mode)
        {
            //is player is already moving then there's no need to snap diagonal to get the right animation
            if (input.x == 0 || input.y == 0 || isMoving) return input;

            input = input.With(Mathf.Sign(input.x), Mathf.Sign(input.y));

            if (mode == EDirectionMode.FourDirectional)
            {
                if (Random.value < 0.5f)
                {
                    input.x = 0;
                }
                else
                {
                    input.y = 0;
                }
            }

            return input;
        }
        private void OnMovementInputChangeEvent(Vector2 input)
        {
            input = SnapDiagonal(input, directionMode);
            
            var directions = directionMode.GetDirections();
            foreach (var dir in directions)
            {
                if (dir.ToVector2() != input) continue;
                
                playerCurrentDirectionState = dir;
                break;
            }

            if (input == Vector2.zero)
            {
                SetPlayerIdleAnimation();
            }
            else
            {
                SetPlayerWalkAnimation();
            }
        }
        private void SetPlayerIdleAnimation()
        {
            if (idleAnimationClips.TryGetValue(playerCurrentDirectionState, out var idleAnim))
            {
                isMoving = false;
                PlayAnimation(Animator.StringToHash(idleAnim.name));
            }
            else
            {
                Debug.LogError("Idle animation not found for direction: " + playerCurrentDirectionState);
            }
        }
        private void SetPlayerWalkAnimation()
        {
            if (walkAnimationClips.TryGetValue(playerCurrentDirectionState, out var walkAnim))
            {
                isMoving = true;
                PlayAnimation(Animator.StringToHash(walkAnim.name));
            }
            else
            {
                Debug.LogError("Walk animation not found for direction: " + playerCurrentDirectionState);
            }
        }
    }
}