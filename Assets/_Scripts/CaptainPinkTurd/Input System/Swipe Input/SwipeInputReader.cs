using System;
using CaptainPinkTurd.Core.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CaptainPinkTurd.Input.Swipe
{
    [RequireComponent(typeof(SwipeDetection))]
    public class SwipeInputReader : MonoBehaviour
    {
        private InputSystemActions inputActions;
        private Camera mainCam;

        public event Action<Vector2, float> OnStartTouch;
        public event Action<Vector2, float> OnEndTouch;

        private void Awake()
        {
            inputActions = new InputSystemActions();
            mainCam = Camera.main;
        }

        private void OnEnable()
        {
            inputActions.Enable();
        }
        private void OnDisable()
        {
            inputActions.Disable();
        }

        private void Start()
        {
            inputActions.Touch.PrimaryContact.started += StartTouchPrimary;
            inputActions.Touch.PrimaryContact.canceled += EndTouchPrimary;
        }

        private void StartTouchPrimary(InputAction.CallbackContext ctx)
        {
            OnStartTouch?.Invoke(PrimaryPosition, (float)ctx.startTime);
        }
        private void EndTouchPrimary(InputAction.CallbackContext ctx)
        {
            OnEndTouch?.Invoke(PrimaryPosition, (float)ctx.time);
        }

        public Vector2 PrimaryPosition => mainCam.ScreenToWorld2D(inputActions.Touch.PrimaryPosition.ReadValue<Vector2>());
    }
}