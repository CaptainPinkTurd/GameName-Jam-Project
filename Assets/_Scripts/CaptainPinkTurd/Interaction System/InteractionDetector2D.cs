using CaptainPinkTurd.Core.Interfaces;
using CaptainPinkTurd.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CaptainPinkTurd.Interaction
{
    [RequireComponent(typeof(Collider2D))]
    public class InteractionDetector2D : MonoBehaviour
    {
        private IInteractable closestInteractable;
        private InputSystemActions playerInputs;
        private Collider2D coll;

        private void Awake()
        {
            playerInputs = new InputSystemActions();
            
            coll = GetComponent<Collider2D>();
            coll.isTrigger = true;
        }
        private void OnEnable()
        {
            playerInputs.Enable();
            playerInputs.Player.Interact.performed += OnInteract;
        }
        private void OnDisable()
        {
            playerInputs.Player.Interact.performed -= OnInteract;
            playerInputs.Disable();
        }
        private void OnInteract(InputAction.CallbackContext context)
        {
            closestInteractable?.Interact();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.TryGetComponent(out IInteractable interactable) && interactable.CanInteract)
            {
                closestInteractable = interactable;
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out IInteractable interactable) && interactable == closestInteractable)
            {
                closestInteractable = null;
            }
        }
    }
}