using CaptainPinkTurd.Core.DesignPattern.SOAP.Variables;
using CaptainPinkTurd.Core.Utils;
using CaptainPinkTurd.Input;
using UnityEngine;

namespace CaptainPinkTurd.TopDownController2D
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerTopDownController2D : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerTopDownMovementStats moveStats; 
        [SerializeField] private Vector2VariableSO movementInput;
        
        private InputSystemActions playerInputs;
        private Rigidbody2D rb;
        private Vector2 smoothedMovementInput;
        private Vector2 movementSmoothVelocity;

        private bool isDashing;
        private bool dashOnCooldown;

        private void Awake()
        {
            playerInputs = new InputSystemActions();
            rb = GetComponent<Rigidbody2D>();

            // Create a runtime clone of movement stats
            var runtimeMoveStats = ScriptableObject.CreateInstance<PlayerTopDownMovementStats>();
            TypeUtils.CopyValues(moveStats, runtimeMoveStats);

            moveStats = runtimeMoveStats; //avoid using the original asset 
        }

        private void OnEnable()
        {
            playerInputs.Enable();
        }

        private void OnDisable()
        {
            playerInputs.Disable();
        }

        private void Update()
        {
            if (isDashing) return;

            movementInput.Value = playerInputs.Player.Move.ReadValue<Vector2>();

            DashCheck();
        }

        private void FixedUpdate()
        {
            if (isDashing) return;

            UpdateMovement();
        }

        //Movement is meant to be disabled when something else is active in the scene (like perhaps Dialogue box or a cutscene)
        //hence why we're setting it with not toggle
        public void ToggleMovement(bool toggle) => enabled = !toggle;
        
        #region Movement
        private void UpdateMovement()
        {
            if (moveStats.smoothMovement)
            {
                smoothedMovementInput = Vector2.SmoothDamp(
                    smoothedMovementInput,
                    movementInput.Value,
                    ref movementSmoothVelocity,
                    moveStats.smoothTime
                );
            }
            else
            {
                smoothedMovementInput = movementInput.Value;
            }

            float speed = playerInputs.Player.Run.IsPressed()
                ? moveStats.runSpeed
                : moveStats.walkSpeed;

            rb.linearVelocity = smoothedMovementInput * speed;
        }
        #endregion

        #region Dash
        private void DashCheck()
        {
            if (!playerInputs.Player.Dash.WasPressedThisFrame() || dashOnCooldown || !moveStats.canDash)
                return;

            StartDash();
        }

        private void StartDash()
        {
            isDashing = true;

            // Direction input handling
            Vector2 dashDir = movementInput.Value.sqrMagnitude < moveStats.dashInputThreshold *
                              moveStats.dashInputThreshold
                ? Vector2.zero
                : movementInput.Value.normalized;

            // running increases dash speed and reduces dash duration
            float dashSpeedMultiplier = playerInputs.Player.Run.IsPressed()
                ? moveStats.runSpeed / moveStats.walkSpeed
                : 1f;

            rb.linearVelocity = dashDir * (moveStats.dashSpeed * dashSpeedMultiplier);

            StartCoroutine(CoroutineUtils.WaitForSeconds(
                moveStats.dashDuration / dashSpeedMultiplier,
                EndDash
            ));
        }

        private void EndDash()
        {
            isDashing = false;
            dashOnCooldown = true;

            StartCoroutine(CoroutineUtils.WaitForSeconds(
                moveStats.dashCooldown,
                () => dashOnCooldown = false
            ));
        }
        #endregion
    }
}