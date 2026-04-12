using CaptainPinkTurd.Core.CustomDataStructure;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Interfaces;
using CaptainPinkTurd.Core.Struct;
using CaptainPinkTurd.TopDownController2D;
using UnityEngine;
using UnityEngine.Rendering;

namespace CaptainPinkTurd.Game
{
    public enum RoundUpType
    {
        Ceil,
        Floor
    }

    [RequireComponent(typeof(Rigidbody2D))]
    public class Wall : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float speed;
        [SerializeField] private EDirection2D moveDirection; 
        [SerializeField] private SerializeKeyValuePair<EDirection2D, RoundUpType>[] roundUpTypeOnStopForDirections;
        [SerializeField] private bool moveOnStart;

        [Header("Collision")]
        [SerializeField] private Collider2D[] collisionColliders;
        [SerializeField] private LayerMask targetPlayerLayer;
        [SerializeField] private LayerMask ignorePlayerLayer;
        [SerializeField] private LayerMask blockingLayer;
        [SerializeField] private LayerMask instantKillLayers;
        
        [Header("Sorting Groups")]
        [SerializeField] private SortingGroup[] wallSortingGroups;

        private Rigidbody2D rb;
        private GameObject player;
        private Vector3 startPoint;
        
        private bool isMoving;
        private bool isBacktrack;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            ExcludeCollisionOnIgnoredLayerMask(false);
            
            EnableSortingGroups(false);

            startPoint = transform.position;
        }
        private void Start()
        {
            isMoving = moveOnStart;
        }

        private void FixedUpdate()
        {
            if (!isMoving) return;

            Vector2 moveDir = GetMoveDirection();
            Vector2 newPos = rb.position + moveDir * (speed * Time.fixedDeltaTime);

            rb.MovePosition(newPos);

            if (!isBacktrack) return;

            var distanceToStart = Vector3.Distance(rb.position, startPoint);

            if (distanceToStart > 0.5f) return;
            
            CheckIfPlayerIsCrushed();
            OnMovingStop();
            
            EnableSortingGroups(false);
            rb.position = startPoint;
        }
        private void ExcludeCollisionOnIgnoredLayerMask(bool exclude)
        {
            foreach(var collisionCollider in collisionColliders)
            {
                collisionCollider.excludeLayers = exclude ?
                    collisionCollider.excludeLayers.AddMask(ignorePlayerLayer) : 
                    collisionCollider.excludeLayers.RemoveMask(ignorePlayerLayer);
            }
        }
        private void EnableSortingGroups(bool enable)
        {
            foreach (var sortingGroup in wallSortingGroups)
            {
                sortingGroup.enabled = enable;
            }
        }

        private Vector2 GetMoveDirection()
        {
            return moveDirection.ToVector2();
        }
        
        private void OnMovingStop()
        {
            var x = transform.localPosition.x;
            var y = transform.localPosition.y;

            if (roundUpTypeOnStopForDirections.TryGetValue(moveDirection, out var roundUpType))
            {
                switch (roundUpType)
                {
                    case RoundUpType.Ceil:
                        x = Mathf.CeilToInt(x);
                        break;
                    case RoundUpType.Floor:
                        x = Mathf.FloorToInt(x);
                        break;
                }
            }
            
            transform.localPosition = new Vector3(x, y, 0);
            
            isMoving = false;
            moveDirection = moveDirection.GetOpposite();
            isBacktrack = !isBacktrack;
            ExcludeCollisionOnIgnoredLayerMask(false);
        }
        private void CheckIfPlayerIsCrushed()
        {
            if (!player || !player.TryGetComponentInHierarchy(out IDamageable damageable)) return;
            
            damageable.TakeDamage(new SDamageData(damageable.MaxHealth, gameObject));
        }
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!blockingLayer.Contains(other.gameObject.layer) || !isMoving) return;
            //Debug.Log($"Collision with {other.gameObject.name}");

            OnMovingStop();

            CheckIfPlayerIsCrushed();
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (!targetPlayerLayer.Contains(other.gameObject.layer)) return;
            player = other.gameObject;
            
            if (!isMoving || !player.TryGetComponentInHierarchy(out PlayerFreeMovementTopDownController2D playerController)) return;

            Vector2 pushDir = GetMoveDirection();
            Vector2 dirToPlayer = ((Vector2)player.transform.position - rb.position).normalized;

            if (Vector2.Dot(pushDir, dirToPlayer) <= 0) return;
            
            playerController.AddExternalVelocity(pushDir);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (!targetPlayerLayer.Contains(other.gameObject.layer) && player != other.gameObject) return;
            
            player = null;
        }

        private void OnTriggerStay2D(Collider2D other) //wall could potentially call this twice because it have 2 colliders
        {
            if (instantKillLayers.Contains(other.gameObject.layer) && 
                other.gameObject.TryGetComponentInHierarchy(out IDamageable damageable))
            {
                damageable.TakeDamage(new SDamageData(damageable.MaxHealth, gameObject));
            }
            
            if (!targetPlayerLayer.Contains(other.gameObject.layer) || moveOnStart || isMoving) return;
            
            ExcludeCollisionOnIgnoredLayerMask(true);
            isMoving = true;
            EnableSortingGroups(true);
        }
    }
}