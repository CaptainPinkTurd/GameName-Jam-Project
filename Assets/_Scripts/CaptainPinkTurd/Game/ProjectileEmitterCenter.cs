using System;
using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.Extensions;
using UnityEngine;

namespace CaptainPinkTurd.Game
{
    [RequireComponent(typeof(Collider2D))]
    public class ProjectileEmitterCenter : MonoBehaviour
    {
        [SerializeField] private LayerMask deactivateLayers;
        [SerializeField] private SoundData deactivateSfx;
        
        private Collider2D coll;

        private void Awake()
        {
            coll = GetComponent<Collider2D>();
            coll.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!deactivateLayers.Contains(other.gameObject.layer)) return;
            
            SoundManager.Instance.CreateSoundBuilder()
                .WithPosition(transform.position).WithRandomPitch().Play(deactivateSfx);
            gameObject.SetActive(false);
        }
    }
}