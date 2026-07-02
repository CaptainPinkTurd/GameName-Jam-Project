using CaptainPinkTurd.Core.Extensions;
using CaptainPinkTurd.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CaptainPinkTurd.InkDialogue
{
    public class DialogueTriggerInteractable : DialogueTriggerBase
    {
        [Header("Dialogue Trigger Configs")]
        [SerializeField] private GameObject visualCue;
        [SerializeField] private LayerMask playerLayer;
        
        private InputSystemActions playerInput;
        private bool playerInRange;

        private void Awake()
        {
            playerInRange = false;
            visualCue.SetActive(false);
            
            playerInput = new InputSystemActions();
        }

        private void OnEnable()
        {
            playerInput.Enable();
            playerInput.Player.Confirm.performed += OnInteraction;
        }

        private void OnDisable()
        {
            playerInput.Player.Confirm.performed -= OnInteraction;
            playerInput.Disable();
        }

        private void OnInteraction(InputAction.CallbackContext ctx)
        {
            if (!playerInRange) return;
            
            StartDialogue();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(!playerLayer.Contains(other.gameObject.layer)) return;
            
            playerInRange = true;
            visualCue.SetActive(true);
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if(!playerLayer.Contains(other.gameObject.layer)) return;
            
            playerInRange = false;
            visualCue.SetActive(false);
        }
    }
}
