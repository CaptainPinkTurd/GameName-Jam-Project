using CaptainPinkTurd.Core.DesignPattern.SOAP.Variables;
using CaptainPinkTurd.Core.Movement;
using CaptainPinkTurd.Input;
using UnityEngine;

namespace CaptainPinkTurd.TopDownController2D
{
    public class PlayerGridBasedMovementTopDownController2D : GridBasedMovement
    {
        [Header("Player Grid Based Top Down Controller Configs")]
        [SerializeField] private Vector2VariableSO movementInput;
        
        private InputSystemActions playerInputs;

        protected void Awake()
        {
            playerInputs = new InputSystemActions();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            playerInputs.Enable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            playerInputs.Disable();
        }

        private void Update()
        {
            if(isMoving) return;
            
            //TODO: Remove diagonal movement or normalize it in the future, can use ToCardinalNormalized method in here
            movementInput.Value = playerInputs.Player.Move.ReadValue<Vector2>();
            Move(movementInput.Value);
        }
    }
}