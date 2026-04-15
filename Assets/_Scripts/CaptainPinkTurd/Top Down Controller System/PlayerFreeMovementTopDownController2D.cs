using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Variables;
using CaptainPinkTurd.Core.Utils;
using CaptainPinkTurd.Input;
using UnityEngine;

namespace CaptainPinkTurd.TopDownController2D
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerFreeMovementTopDownController2D : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerTopDownMovementStats referenceMoveStats; 
        [SerializeField] private Vector2VariableSO movementInput;
        [SerializeField] private Vector3VariableSO playerPosition;
        
        private PlayerTopDownMovementStats runtimeMoveStats;
        public InputSystemActions playerInputs;
        private Rigidbody2D rb;
        private Vector2 smoothedMovementInput;
        private Vector2 movementSmoothVelocity;
        private Vector2 externalVelocity = Vector2.zero;
        
        private bool isDashing;
        private bool dashOnCooldown;

        private void Awake()
        {
            playerInputs = new InputSystemActions();
            rb = GetComponent<Rigidbody2D>();

            GenerateRuntimeMoveStats();
        }

        private void GenerateRuntimeMoveStats()
        {
            // Create a runtime clone of movement stats
            runtimeMoveStats = ScriptableObject.CreateInstance<PlayerTopDownMovementStats>();
            TypeUtils.CopyValues(referenceMoveStats, runtimeMoveStats);
        }

        private void OnEnable()
        {
            playerInputs.Enable();
            referenceMoveStats.OnValueChange.Subscribe(GenerateRuntimeMoveStats);
        }

        private void OnDisable()
        {
            playerInputs.Disable();
            referenceMoveStats.OnValueChange.Unsubscribe(GenerateRuntimeMoveStats);
        }

        private void Update()
        {
            if (isDashing) return;

            movementInput.Value = playerInputs.Player.Move.ReadValue<Vector2>();

            DashCheck();
            
            if (movementInput.Value == Vector2.zero) return;
            
            playerPosition.Value = transform.position;
        }

        private void FixedUpdate()
        {
            if (!isDashing)
            {
                UpdateMovement();
            }
            
            externalVelocity = Vector2.Lerp(externalVelocity, Vector2.zero, 10f * Time.fixedDeltaTime);
        }

        #region Movement
        
        //Movement is meant to be disabled when something else is active in the scene (like perhaps Dialogue box, knockback or a cutscene)
        //hence why we're setting it with not toggle
        public void ToggleMovement(bool toggle) => enabled = !toggle;
        
        private void UpdateMovement()
        {
            if (runtimeMoveStats.smoothMovement)
            {
                smoothedMovementInput = Vector2.SmoothDamp(
                    smoothedMovementInput,
                    movementInput.Value,
                    ref movementSmoothVelocity,
                    runtimeMoveStats.smoothTime
                );
            }
            else
            {
                smoothedMovementInput = movementInput.Value;
            }

            float speed = playerInputs.Player.Run.IsPressed()
                ? runtimeMoveStats.runSpeed
                : runtimeMoveStats.walkSpeed;

            rb.linearVelocity = smoothedMovementInput * speed + externalVelocity;
        }

        public void AddExternalVelocity(Vector2 velocity) => externalVelocity += velocity;
        
        #endregion

        #region Dash
        private void DashCheck()
        {
            if (!playerInputs.Player.Dash.WasPressedThisFrame() || dashOnCooldown || !runtimeMoveStats.canDash)
                return;

            StartDash();
        }

        private void StartDash()
        {
            isDashing = true;

            SoundManager.Instance.CreateSoundBuilder().WithPosition(transform.position).WithRandomPitch()
                .Play(runtimeMoveStats.dashSfx);

            // Direction input handling
            Vector2 dashDir = movementInput.Value.sqrMagnitude < runtimeMoveStats.dashInputThreshold *
                              runtimeMoveStats.dashInputThreshold
                ? Vector2.zero
                : movementInput.Value.normalized;

            // running increases dash speed and reduces dash duration
            float dashSpeedMultiplier = playerInputs.Player.Run.IsPressed()
                ? runtimeMoveStats.runSpeed / runtimeMoveStats.walkSpeed
                : 1f;

            rb.linearVelocity = dashDir * (runtimeMoveStats.dashSpeed * dashSpeedMultiplier);

            StartCoroutine(CoroutineUtils.WaitForSeconds( runtimeMoveStats.dashDuration / dashSpeedMultiplier,
                EndDash
            ));
        }

        private void EndDash()
        {
            isDashing = false;
            dashOnCooldown = true;

            StartCoroutine(CoroutineUtils.WaitForSeconds(
                runtimeMoveStats.dashCooldown,
                () => dashOnCooldown = false
            ));
        }
        #endregion
    }
}