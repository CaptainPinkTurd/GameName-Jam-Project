using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Core.Interfaces;
using CaptainPinkTurd.Core.Struct;
using CaptainPinkTurd.TopDownController2D;
using UnityEngine;

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
        [SerializeField] private Transform endPoint;
        [SerializeField] private EWallBehaviour behaviour;
        [SerializeField] private bool moveOnStart;

        [Header("Collision")]
        [SerializeField] private LayerMask targetPlayerLayer;
        [SerializeField] private LayerMask blockingLayer;

        private Rigidbody2D rb;
        private GameObject player;
        private Vector3 startPoint;
        private Vector3 currentTarget;

        private bool isMoving;
        private bool hasFinishedMoving;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            startPoint = transform.position;
            currentTarget = endPoint ? endPoint.position : startPoint;
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

            CheckReachedTarget();
        }

        private Vector2 GetMoveDirection()
        {
            Vector2 toTarget = (currentTarget - (Vector3)rb.position).normalized;
            return toTarget;
        }

        private void CheckReachedTarget()
        {
            float distance = Vector2.Distance(rb.position, currentTarget);
            
            if (distance >= 0.05f) return;
            
            switch (behaviour)
            {
                case EWallBehaviour.Loop:
                    SwapTarget();
                    break;
                case EWallBehaviour.Stop:
                    OnMovingStop();
                    break;
            }
        }

        private void SwapTarget()
        {
            // Toggle between start and end
            currentTarget = (Vector2)currentTarget == (Vector2)endPoint.position ? startPoint : endPoint.position;
        }

        private void OnMovingStop()
        {
            isMoving = false;
            hasFinishedMoving = true;
        }
        private void CheckIfPlayerIsCrushed()
        {
            if (!player || !player.TryGetComponentInHierarchy(out IDamageable damageable)) return;
            
            damageable.TakeDamage(new SDamageData(damageable.MaxHealth, gameObject));
        }
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!blockingLayer.Contains(other.gameObject.layer)) return;

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
            if (!targetPlayerLayer.Contains(other.gameObject.layer) || moveOnStart || hasFinishedMoving) return;
            
            isMoving = true;
        }
    }
}