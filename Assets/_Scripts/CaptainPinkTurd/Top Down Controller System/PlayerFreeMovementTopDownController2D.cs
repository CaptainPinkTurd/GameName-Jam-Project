using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Variables;
using CaptainPinkTurd.Core.Utils;
using CaptainPinkTurd.Input;
using UnityEngine;
namespace CaptainPinkTurd.TopDownControllerSystem
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerFreeMovementTopDownController2D : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerTopDownMovementStats referenceMoveStats; 
        [SerializeField] private Vector2VariableSO movementInput;
        [SerializeField] private FloatVariableSO currentPlayerSpeed;
        [SerializeField] private Vector3VariableSO playerPosition;
        [SerializeField] private BoolVariableSO isDashing;
        
        public InputSystemActions playerInputs;
        
        private PlayerTopDownMovementStats runtimeMoveStats;
        private Rigidbody2D rb;
        private Vector2 smoothedMovementInput;
        private Vector2 movementSmoothVelocity;
        private Vector2 externalVelocity = Vector2.zero;
        private Vector2 lastNonZeroMovementInput = Vector2.down;
        
        private bool dashOnCooldown;
        private bool movementEnabled;
        
        public bool MovementEnabled => movementEnabled;

        private void Awake()
        {
            playerInputs = new InputSystemActions();
            rb = GetComponent<Rigidbody2D>();

            GenerateRuntimeMoveStats();
        }

        private void Start()
        {
            ToggleMovement(true);
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
            isDashing.Value = false; // ensure dash never survives a deactivate/reload
        }

        private void Update()
        {
            // if (Keyboard.current.kKey.wasPressedThisFrame)
            // {
            //     ToggleMovement(!movementEnabled);
            // }
            movementInput.Value = playerInputs.Player.Move.ReadValue<Vector2>();
            if(movementInput.Value != Vector2.zero) 
            {
                lastNonZeroMovementInput = movementInput.Value;
            }
            
            if (isDashing.Value || !movementEnabled) return;

            DashCheck();
            
            if (movementInput.Value == Vector2.zero) return;
            
            playerPosition.Value = transform.position;
        }

        private void FixedUpdate()
        {
            if (!movementEnabled) return;
            
            if (!isDashing.Value)
            {
                UpdateMovement();
            }
            
            externalVelocity = Vector2.Lerp(externalVelocity, Vector2.zero, 10f * Time.fixedDeltaTime);
        }

        #region Movement
        
        public void ToggleMovement(bool toggle) 
        {
            movementEnabled = toggle;
            
            if(movementEnabled) return;
            
            rb.linearVelocity = Vector2.zero;
            movementInput.Value = Vector2.zero;
            currentPlayerSpeed.Value = 0f;
        }
        
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

            float speed = 0f;
            if (movementInput.Value != Vector2.zero)
            {
                speed = playerInputs.Player.Run.IsPressed()
                    ? runtimeMoveStats.runSpeed
                    : runtimeMoveStats.walkSpeed;
            }
            
            currentPlayerSpeed.Value = speed / runtimeMoveStats.walkSpeed; 

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
            isDashing.Value = true;

            SoundManager.Instance.CreateSoundBuilder().WithPosition(transform.position).WithRandomPitch()
                .Play(runtimeMoveStats.dashSfx);

            // Direction input handling
            Vector2 dashDir = movementInput.Value.sqrMagnitude < runtimeMoveStats.dashInputThreshold *
                              runtimeMoveStats.dashInputThreshold
                ? lastNonZeroMovementInput.normalized
                : movementInput.Value.normalized;

            // running increases dash speed and reduces dash duration
            float dashSpeedMultiplier = playerInputs.Player.Run.IsPressed()
                ? runtimeMoveStats.runSpeed / runtimeMoveStats.walkSpeed
                : 1f;

            rb.linearVelocity = dashDir * (runtimeMoveStats.dashSpeed * dashSpeedMultiplier);

            StartCoroutine(CoroutineUtils.WaitForSeconds(
                runtimeMoveStats.dashDuration / dashSpeedMultiplier,
                EndDash
            ));
        }

        private void EndDash()
        {
            isDashing.Value = false;
            dashOnCooldown = true;

            StartCoroutine(CoroutineUtils.WaitForSeconds(
                runtimeMoveStats.dashCooldown,
                () => dashOnCooldown = false
            ));
        }
        #endregion
    }
}