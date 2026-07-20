using CaptainPinkTurd.AnimationSystem;
using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.CustomDataStructure;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Variables;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.Core.Extensions;
using UnityEngine;
using ZLinq;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

namespace CaptainPinkTurd.RPG
{
    public class PlayerAnimationController : AnimationControllerBase
    {
        [Header("Player Animation Clips")] 
        [SerializeField] private SerializeKeyValuePair<EDirection2D, AnimationClip>[] idleAnimationClips; 
        [SerializeField] private SerializeKeyValuePair<EDirection2D, AnimationClip>[] walkAnimationClips;
        [Tooltip("The direction specify to flip the player sprite if there is any")]
        [SerializeField] private EDirection2D[] spriteFlipDirections; 
        
        [Header("Input Events")]
        [SerializeField] private Vector2VariableSO currentMovementInput;
        [SerializeField] private EDirectionMode directionMode;
        [SerializeField][ReadOnly] private EDirection2D playerCurrentDirectionState;
        
        private bool isMoving;
        private bool canChangeDirectionState = true;
        
        public override int DefaultAnimationHash { get; set; }

        protected override void Awake()
        {
            base.Awake();
            
            //instead of Subscribe and Unsubscribe in OnEnable and OnDisable
            //we're doing this here to ensure that the event is constantly being called to update our playerCurrentDirectionState all the time
            currentMovementInput.OnValueChanged += OnMovementInputChangeEvent;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            OnMovementInputChangeEvent(currentMovementInput.Value);
        }

        private void OnDestroy()
        {
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
        public void SetCanChangeDirectionState(bool value) => canChangeDirectionState = value;
        private void OnMovementInputChangeEvent(Vector2 input)
        {
            if(!canChangeDirectionState) return;
            
            input = SnapDiagonal(input, directionMode);
            
            var directions = directionMode.GetDirections();
            foreach (var dir in directions)
            {
                if (dir.ToVector2() != input) continue;
                
                playerCurrentDirectionState = dir;
                break;
            }

            CheckForSpriteFlip();
            if (input == Vector2.zero)
            {
                SetPlayerIdleAnimation();
            }
            else
            {
                SetPlayerWalkAnimation();
            }
        }

        private void CheckForSpriteFlip()
        {
            var spriteFlip = spriteFlipDirections.AsValueEnumerable().Contains(playerCurrentDirectionState);
            
            spriteRenderer.flipX = spriteFlip;
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
                Debug.LogWarning("Idle animation not found for direction: " + playerCurrentDirectionState);
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
                Debug.LogWarning("Walk animation not found for direction: " + playerCurrentDirectionState);
            }
        }
    }
}