using CaptainPinkTurd.Core.Extensions;
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

        [Header("Collision")]
        [SerializeField] private LayerMask targetPlayerLayer;
        [SerializeField] private LayerMask blockingLayer;

        private Rigidbody2D rb;

        private Vector3 startPoint;
        private Vector3 currentTarget;

        private bool isMoving = true;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            startPoint = transform.position;
            currentTarget = endPoint ? endPoint.position : startPoint;
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
                    isMoving = false;
                    break;
            }
        }

        private void SwapTarget()
        {
            // Toggle between start and end
            currentTarget = (Vector2)currentTarget == (Vector2)endPoint.position ? startPoint : endPoint.position;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!blockingLayer.Contains(other.gameObject.layer)) return;

            switch (behaviour)
            {
                case EWallBehaviour.Stop:
                    isMoving = false;
                    break;
                case EWallBehaviour.Loop:
                    SwapTarget();
                    break;
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (!targetPlayerLayer.Contains(other.gameObject.layer)) return;
            if (!other.gameObject.TryGetComponentInHierarchy(out PlayerFreeMovementTopDownController2D playerController)) return;

            Vector2 pushDir = GetMoveDirection();
            playerController.AddExternalVelocity(pushDir * speed);
        }
    }
}