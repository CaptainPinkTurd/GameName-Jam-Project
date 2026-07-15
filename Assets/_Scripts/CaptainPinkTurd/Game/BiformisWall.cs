using System;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Variables;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Utils;
using CaptainPinkTurd.TopDownControllerSystem;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace CaptainPinkTurd.Game
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BiformisWall : MonoBehaviour
    {
        [Header("Biformis Collision")]
        [SerializeField] protected Collider2D[] collisionColliders;
        [FormerlySerializedAs("targetPlayerLayer")] [SerializeField] protected LayerMask tangibleLayer;
        [FormerlySerializedAs("ignorePlayerLayer")] [SerializeField] protected LayerMask intangibleLayer;
        [SerializeField] protected LayerMask blockingLayer;
        [SerializeField] protected Vector2VariableSO playerCurrentInput;
        
        [Header("Wall Repel Configs")]
        [SerializeField] protected Collider2D insideDetectionCollider;
        [SerializeField] private float repelForce = 4f;
        [SerializeField] private ForceMode2D repelForceMode = ForceMode2D.Force;
        [SerializeField] private float decelerateDamping = 10f;
        [SerializeField] private float stopMagnitudeThreshold = 1f;
        
        [Header("Sorting Groups")]
        [SerializeField] private SortingGroup[] wallSortingGroups;
        
        protected Rigidbody2D rb;
        protected GameObject player;

        private Vector2 playerCurrentDirection;
        private Coroutine decelerateCoroutine;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            playerCurrentInput.OnValueChanged += OnPlayerMovementInputChangeEvent;
        }

        private void OnDisable()
        {
            playerCurrentInput.OnValueChanged -= OnPlayerMovementInputChangeEvent;
        }

        private void OnPlayerMovementInputChangeEvent(Vector2 input)
        {
            if(input == Vector2.zero) return;
            playerCurrentDirection = input;
        }
        
        protected void ExcludeCollisionOnIntangibleLayerMask(bool exclude)
        {
            foreach(var collisionCollider in collisionColliders)
            {
                collisionCollider.excludeLayers = exclude ?
                    collisionCollider.excludeLayers.AddMask(intangibleLayer) : 
                    collisionCollider.excludeLayers.RemoveMask(intangibleLayer);
            }
        }
        protected void ExcludeCollisionOnTangibleLayerMask(bool exclude)
        {
            foreach(var collisionCollider in collisionColliders)
            {
                collisionCollider.excludeLayers = exclude ?
                    collisionCollider.excludeLayers.AddMask(tangibleLayer) : 
                    collisionCollider.excludeLayers.RemoveMask(tangibleLayer);
            }
        }
        protected void EnableSortingGroups(bool enable)
        {
            foreach (var sortingGroup in wallSortingGroups)
            {
                sortingGroup.enabled = enable;
            }
        }

        protected virtual void OnCollisionStay2D(Collision2D other) //need to figure out if the player is inside the collision or not to implement the repel mechanic
        {
            if (!tangibleLayer.Contains(other.gameObject.layer)) return;
            player = other.gameObject;
        }
        
        private void OnCollisionExit2D(Collision2D other)
        {
            if (!tangibleLayer.Contains(other.gameObject.layer) && player != other.gameObject) return;
            
            player = null;
        }
        
        protected virtual void OnTriggerStay2D(Collider2D other)
        {
            if (!insideDetectionCollider) return;
            
            var playerTopDownMovement = other.gameObject.GetComponentInParent<PlayerFreeMovementTopDownController2D>();
            var playerRb = other.gameObject.GetComponentInParent<Rigidbody2D>();
            
            if (tangibleLayer.Contains(other.gameObject.layer))
            {
                if (!insideDetectionCollider.bounds.Contains(other.bounds.max) ||
                    !insideDetectionCollider.bounds.Contains(other.bounds.min)) return;
                
                ExcludeCollisionOnTangibleLayerMask(true);
                playerRb.AddForce(playerCurrentDirection * repelForce, repelForceMode);
                if(playerTopDownMovement.MovementEnabled) playerTopDownMovement.ToggleMovement(false);
            }
            else if (intangibleLayer.Contains(other.gameObject.layer))
            {
                if (insideDetectionCollider.bounds.Contains(other.bounds.max) &&
                    insideDetectionCollider.bounds.Contains(other.bounds.min)) return;
                
                playerTopDownMovement.ToggleMovement(true);
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            if (!insideDetectionCollider) return;

            var playerTopDownMovement = other.gameObject.GetComponentInParent<PlayerFreeMovementTopDownController2D>();
            var playerRb = other.gameObject.GetComponentInParent<Rigidbody2D>();
            
            if ((tangibleLayer.Contains(other.gameObject.layer) || intangibleLayer.Contains(other.gameObject.layer)) 
                && !playerTopDownMovement.MovementEnabled)
            {
                ExcludeCollisionOnTangibleLayerMask(false);
                playerRb.linearDamping = decelerateDamping;
                    
                if (decelerateCoroutine != null) return;
                    
                decelerateCoroutine = StartCoroutine(CoroutineUtils.WaitForCondition(() => playerRb.linearVelocity.magnitude <= stopMagnitudeThreshold,
                    () =>
                    {
                        Debug.Log("Player stopped moving");
                        playerRb.linearDamping = 0;
                        playerTopDownMovement.ToggleMovement(true);
                        decelerateCoroutine = null;
                    }));
            }
        }
    }
}