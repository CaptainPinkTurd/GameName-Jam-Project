using CaptainPinkTurd.AnimationSystem;
using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Variables;
using CaptainPinkTurd.Core.Extensions;
using UnityEngine;

namespace CaptainPinkTurd.Game
{
    public class Door : AnimationControllerBase
    {
        [Header("Door Configs")] 
        [SerializeField] private LayerMask playerLayers;
        [SerializeField] private VoidEvent onDoorEnter;
        [SerializeField] private SoundData openSfx;
        [SerializeField] private SoundData enterSfx;
        
        [Header("Door Animations")]
        [SerializeField] private AnimationClip doorCloseAnimation;
        [SerializeField] private AnimationClip doorOpeningAnimation;
        [SerializeField] private AnimationClip doorOpenAnimation;

        [Header("Player Variable Events")] 
        [SerializeField] private Vector2VariableSO playerInput;
        
        public override int DefaultAnimationHash { get; set; }

        private bool isOpen;
        private bool playerIsInDoorVicinity;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            playerInput.OnValueChanged += DoorEnterCheck;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            
            playerInput.OnValueChanged -= DoorEnterCheck;
        }
        
        protected override void Start()
        {
            base.Start();
            
            PlayAnimation(Animator.StringToHash(doorCloseAnimation.name));
            isOpen = false;
        }
        
        public void OpenDoor()
        {
            SoundManager.Instance.CreateSoundBuilder()
                .WithPosition(transform.position).WithRandomPitch().Play(openSfx);
            
            PlayAnimation(Animator.StringToHash(doorOpeningAnimation.name), onAnimationEnd: () =>
            {
                isOpen = true;
                PlayAnimation(Animator.StringToHash(doorOpenAnimation.name), isClamp: true);
            });
        }
        private void DoorEnterCheck(Vector2 value)
        {
            if (!playerIsInDoorVicinity || value.y <= 0) return;
            
            SoundManager.Instance.CreateSoundBuilder().WithPosition(transform.position).WithRandomPitch().Play(enterSfx);
            onDoorEnter.Raise();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isOpen) return;
            if (!playerLayers.Contains(other.gameObject.layer)) return;
            
            playerIsInDoorVicinity = true;
            
            DoorEnterCheck(playerInput.Value);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!isOpen) return;
            if (!playerLayers.Contains(other.gameObject.layer)) return;
            
            playerIsInDoorVicinity = false;
        }
    }
}