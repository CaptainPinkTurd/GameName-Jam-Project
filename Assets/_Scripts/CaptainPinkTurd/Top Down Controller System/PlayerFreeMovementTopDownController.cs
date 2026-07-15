using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Variables;
using CaptainPinkTurd.Core.Utils;
using CaptainPinkTurd.Input;
using UnityEngine;

namespace CaptainPinkTurd.TopDownControllerSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerFreeMovementTopDownController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerTopDownMovementStats referenceMoveStats; 
        [SerializeField] private Vector2VariableSO movementInput;
        [SerializeField] private FloatVariableSO currentPlayerSpeed;
        [SerializeField] private Vector3VariableSO playerPosition;
        
        private PlayerTopDownMovementStats runtimeMoveStats;
        public InputSystemActions playerInputs;
        private Rigidbody rb;
        private Vector3 smoothedMovementInput;
        private Vector3 movementSmoothVelocity;
        private Vector3 externalVelocity = Vector3.zero;
        
        private bool isDashing;
        private bool dashOnCooldown;
        private bool movementEnabled;
        
        public bool MovementEnabled => movementEnabled;

        private void Awake()
        {
            playerInputs = new InputSystemActions();
            rb = GetComponent<Rigidbody>();

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
        }

        private void Update()
        {
            // if (Keyboard.current.kKey.wasPressedThisFrame)
            // {
            //     ToggleMovement(!movementEnabled);
            // }
            movementInput.Value = playerInputs.Player.Move.ReadValue<Vector2>();
            
            if (isDashing || !movementEnabled) return;

            DashCheck();
            
            if (movementInput.Value == Vector2.zero) return;
            
            playerPosition.Value = transform.position;
        }

        private void FixedUpdate()
        {
            if (!movementEnabled) return;
            
            if (!isDashing)
            {
                UpdateMovement();
            }
            
            externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, 10f * Time.fixedDeltaTime);
        }

        #region Movement
        
        public void ToggleMovement(bool toggle) 
        {
            movementEnabled = toggle;
            
            if(movementEnabled) return;
            
            rb.linearVelocity = Vector3.zero;
            movementInput.Value = Vector3.zero;
            currentPlayerSpeed.Value = 0f;
        }
        
        private void UpdateMovement()
        {
            if (runtimeMoveStats.smoothMovement)
            {
                //not usable yet
                smoothedMovementInput = Vector3.SmoothDamp(
                    smoothedMovementInput,
                    movementInput.Value,
                    ref movementSmoothVelocity,
                    runtimeMoveStats.smoothTime
                );
            }
            else
            {
                smoothedMovementInput.x = movementInput.Value.x;
                smoothedMovementInput.z = movementInput.Value.y;
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

        public void AddExternalVelocity(Vector3 velocity) => externalVelocity += velocity;
        
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
            Vector3 dashDir = movementInput.Value.sqrMagnitude < runtimeMoveStats.dashInputThreshold *
                              runtimeMoveStats.dashInputThreshold
                ? Vector3.zero
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