using System.Collections;
using System.Collections.Generic;
using CaptainPinkTurd.Core.Enum;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CaptainPinkTurd.UI
{
    public class DropdownNavigation : MonoBehaviour //keep this class mono so we could use coroutine for scroll in here
    {
        [SerializeField] private InputActionReference navigationAction;
        [SerializeField] private float navigationHoldDelayTime = 0.2f;
        
        private readonly List<ButtonDropdownBase> buttons = new List<ButtonDropdownBase>();
        
        private InputAction spaceBarAction;
        private ButtonDropdownBase currentButton;
        private int currentIndex = 0;
        private bool isNavigating = false;

        private void Awake()
        {
            spaceBarAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/space");
        }

        public void ReverseButtonsList()
        {
            buttons.Reverse();
            currentIndex = buttons.Count - currentIndex - 1;
        }
        
        internal void AddNewButton(ButtonDropdownBase button)
        {
            buttons.Add(button);
            button.onButtonHover.Subscribe(OnExternalNavigationChange);
        }

        private void RemoveOldButtons()
        {
            foreach(var button in buttons)
            {
                button.ButtonHoverEvent(false);
                button.onButtonHover.Unsubscribe(OnExternalNavigationChange);
            }
            buttons.Clear();
            currentIndex = 0;
        }
        public void EnableInput()
        {
            RemoveOldButtons();
            
            navigationAction.action.Enable();
            navigationAction.action.started += OnNavigationStarted;
            navigationAction.action.canceled += OnNavigationCanceled;
            
            spaceBarAction.Enable();
            spaceBarAction.performed += SpaceBarActionOnPerformed;
        }

        public void DisableInput()
        {
            currentIndex = 0;
            
            navigationAction.action.started -= OnNavigationStarted;
            navigationAction.action.canceled -= OnNavigationCanceled;
            navigationAction.action.Disable();
            
            spaceBarAction.performed -= SpaceBarActionOnPerformed;
            spaceBarAction.Disable();
        }

        private void OnDisable()
        {
            DisableInput();
        }

        internal void SetCurrentNavigation(EInputDirection inputDirection)
        {
            if(currentButton) currentButton.ButtonHoverEvent(false);

            switch (inputDirection)
            {
                case EInputDirection.Up:
                    do
                    {
                        currentIndex++;
                        if (currentIndex >= buttons.Count) currentIndex = 0;
                    } while (buttons[currentIndex].isInactive);
                    break;
                case EInputDirection.Down:
                    do
                    {
                        currentIndex--;
                        if (currentIndex < 0) currentIndex = buttons.Count - 1;
                    } while (buttons[currentIndex].isInactive);
                    break;
            }
            
            currentButton = buttons[currentIndex];
            currentButton.ButtonHoverEvent(true);
        }

        public void InitialNavigationSetup()
        {
            while (buttons[currentIndex].isInactive)
            {
                currentIndex++;
                if (currentIndex >= buttons.Count) currentIndex = 0;
            }
            
            currentButton = buttons[currentIndex];
            currentButton.ButtonHoverEvent(true, false);
        }
        private void OnNavigationStarted(InputAction.CallbackContext ctx)
        {
            if (isNavigating) return;
            
            isNavigating = true;
            Vector2 input = ctx.ReadValue<Vector2>();
            switch (input.y)
            {
                case > 0:
                    StartCoroutine(OnNavigationHold(EInputDirection.Up));
                    break;
                case < 0:
                    StartCoroutine(OnNavigationHold(EInputDirection.Down));
                    break;
            }
        }
        private void OnNavigationCanceled(InputAction.CallbackContext ctx)
        {
            isNavigating = false;
        }

        private IEnumerator OnNavigationHold(EInputDirection inputDirection)
        {
            float timer = 0f;

            while (isNavigating)
            {
                if (timer <= 0f)
                {
                    SetCurrentNavigation(inputDirection);
                    timer = navigationHoldDelayTime;
                }

                timer -= Time.deltaTime;
                yield return null; 
                //check every frame to ensure the while loop will break instantly if the condition turns to false in a split second
            }
        }
        private void SpaceBarActionOnPerformed(InputAction.CallbackContext _)
        {
            buttons[currentIndex].OnButtonClickEvent();
        }

        private void OnExternalNavigationChange(ButtonDropdownBase newButton)
        {
            if (!currentButton || currentButton == newButton) return;
            
            currentButton.ButtonHoverEvent(false);
            currentButton = newButton; 
            currentIndex = buttons.FindIndex(button => button == newButton);
        }
    }
}