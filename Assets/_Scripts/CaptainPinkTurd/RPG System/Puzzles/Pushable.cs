using CaptainPinkTurd.Core.DesignPattern.SOAP.Variables;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Movement;
using CaptainPinkTurd.Core.Utils;
using UnityEngine;

namespace CaptainPinkTurd.RPG.Puzzles
{
    public class Pushable : GridBasedMovement
    {
        [Header("Pushable Properties")]
        [SerializeField] private float pushTimeToMove = .25f;
        [SerializeField][Range(0f, 1f)] private float minAlignmentLimit = .85f;
        [SerializeField] private Vector2VariableSO playerCurrentMovementInput;
        [SerializeField] private LayerMask playerLayers;
        
        private GameObject player;
        private bool isInContactWithPlayer = false;
        private bool playerIsPushing;

        protected override void OnEnable()
        {
            base.OnEnable();
            playerCurrentMovementInput.OnValueChanged += OnPlayerMovementInputChange;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            playerCurrentMovementInput.OnValueChanged -= OnPlayerMovementInputChange;
        }

        private void PushEvaluate()
        {
            if (!isInContactWithPlayer || !playerIsPushing || !player) return;
            
            var playerPushPosition = player.transform.position;
            var pushDirection = playerCurrentMovementInput.Value;
            StartCoroutine(CoroutineUtils.WaitForSeconds(pushTimeToMove, () =>
            {
                Move(pushDirection);
            }, PushCancelCondition));

            bool PushCancelCondition()
            {
                Vector2 offset = (transform.position - playerPushPosition).normalized;
                float pushAngleAlignment = Vector2.Dot(offset, pushDirection.normalized);
                bool isNearPushableCenter = pushAngleAlignment >= minAlignmentLimit;

                #region Push Cancel Debug Region

                // if (!isNearPushableCenter)
                // {
                //     Debug.Log($"Player is either too close on the edge or is walking away from the center of {name}," +
                //               $"push angle alignment limit was {pushAngleAlignment}");
                // }

                #endregion
                
                return !isInContactWithPlayer || !playerIsPushing || !isNearPushableCenter;
            }
        }
        private void OnPlayerMovementInputChange(Vector2 input)
        {
            if (input == Vector2.zero || 
                (input.x != 0 && input.y != 0))
            {
                playerIsPushing = false;
            }
            else
            {
                playerIsPushing = true;
                PushEvaluate();
            }
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!playerLayers.Contains(other.gameObject.layer)) return;
            
            player = other.gameObject;
            isInContactWithPlayer = true;
            PushEvaluate();
        }
        private void OnCollisionExit2D(Collision2D other)
        {
            if (!playerLayers.Contains(other.gameObject.layer)) return;
            
            isInContactWithPlayer = false;
        }
    }
}