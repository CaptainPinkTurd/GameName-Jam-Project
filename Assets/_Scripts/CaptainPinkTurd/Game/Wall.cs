using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Interfaces;
using CaptainPinkTurd.Core.Struct;
using CaptainPinkTurd.TopDownController2D;
using UnityEngine;
using UnityEngine.Rendering;

namespace CaptainPinkTurd.Game
{
    public enum EWallBehaviour
    {
        Loop,
        Stop
    }

    [RequireComponent(typeof(Rigidbody2D))]
    public class Wall : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float speed;
        [SerializeField] private EWallBehaviour behaviour;
        [SerializeField] private EDirection2D moveDirection; 
        [SerializeField] private bool moveOnStart;

        [Header("Collision")]
        [SerializeField] private LayerMask targetPlayerLayer;
        [SerializeField] private LayerMask blockingLayer;
        [SerializeField] private LayerMask instantKillLayers;

        private Rigidbody2D rb;
        private GameObject player;
        private Vector3 startPoint;
        private SortingGroup bottomWallSortingGroup;
        
        private bool isMoving;
        private bool hasFinishedMoving;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            bottomWallSortingGroup = GetComponentInChildren<SortingGroup>();
            bottomWallSortingGroup.enabled = false;

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
        }

        private Vector2 GetMoveDirection()
        {
            return moveDirection.ToVector2();
        }

        private void SwapTarget()
        {
            // Toggle between start and end
            //currentTarget = (Vector2)currentTarget == (Vector2)endPoint.position ? startPoint : endPoint.position;
        }

        private void OnMovingStop()
        {
            isMoving = false;
            hasFinishedMoving = true;
            
            var x = transform.localPosition.x;
            var y = transform.localPosition.y;
            var signX = Mathf.Sign(x);
            var signY = Mathf.Sign(y);
            
            switch (moveDirection)
            {
                case EDirection2D.Left:
                case EDirection2D.Right:
                    x = Mathf.FloorToInt(Mathf.Abs(x)) * signX;
                    break;
                case EDirection2D.Up:
                case EDirection2D.Down:
                    y = Mathf.FloorToInt(Mathf.Abs(y)) * signY;
                    break;
            }
            
            transform.localPosition = new Vector3(x, y, 0);
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

            switch (behaviour)
            {
                case EWallBehaviour.Stop:
                    OnMovingStop();
                    break;
                case EWallBehaviour.Loop:
                    SwapTarget();
                    break;
            }

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

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (instantKillLayers.Contains(other.gameObject.layer) && 
                other.gameObject.TryGetComponentInHierarchy(out IDamageable damageable))
            {
                damageable.TakeDamage(new SDamageData(damageable.MaxHealth, gameObject));
            }
            
            if (!targetPlayerLayer.Contains(other.gameObject.layer) || moveOnStart || hasFinishedMoving) return;
            
            isMoving = true;
            bottomWallSortingGroup.enabled = true;
        }
    }
}