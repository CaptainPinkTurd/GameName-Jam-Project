using UnityEngine;

namespace CaptainPinkTurd.TopDownController2D
{
    public class MoveEffect : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerFreeMovementTopDownController2D playerController;
        [SerializeField] private ParticleSystem moveParticles;
        [SerializeField] private TrailRenderer runTrail;

        private Rigidbody2D rb;

        private void Awake()
        {
            playerController = GetComponent<PlayerFreeMovementTopDownController2D>();
            rb = playerController.GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            HandleParticles();
            HandleTrail();
        }

        private void HandleParticles()
        {
            // Play particles when moving, stop when idle
            if (rb.linearVelocity.magnitude > 0.1f)
            {
                if (!moveParticles.isPlaying)
                    moveParticles.Play();
            }
            else
            {
                if (moveParticles.isPlaying)
                    moveParticles.Stop();
            }
        }

        private void HandleTrail()
        {
            // Enable trail only when Run (Shift) is pressed
            if (playerController.playerInputs.Player.Run.IsPressed())
            {
                if (!runTrail.emitting)
                    runTrail.emitting = true;
            }
            else
            {
                if (runTrail.emitting)
                    runTrail.emitting = false;
            }
        }
    }
}
