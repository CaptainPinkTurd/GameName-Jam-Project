using System;
using CaptainPinkTurd.AnimationSystem;
using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using CaptainPinkTurd.Core.Extensions;
using UnityEngine;

namespace CaptainPinkTurd.Game
{
    public class Door : AnimationControllerBase
    {
        [Header("Door Configs")] 
        [SerializeField] private LayerMask playerLayers;
        [SerializeField] private SoundData openSfx;
        [SerializeField] private VoidEvent onDoorEnter;
        
        [Header("Door Animations")]
        [SerializeField] private AnimationClip doorCloseAnimation;
        [SerializeField] private AnimationClip doorOpeningAnimation;
        [SerializeField] private AnimationClip doorOpenAnimation;
        
        public override int DefaultAnimationHash { get; set; }

        private bool isOpen;

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

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isOpen) return;
            if (!playerLayers.Contains(other.gameObject.layer)) return;
            
            onDoorEnter.Raise();
        }
    }
}