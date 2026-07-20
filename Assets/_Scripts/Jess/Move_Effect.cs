using System;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Variables;
using UnityEngine;

public class MoveEffect : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private BoolVariableSO isPlayerDashing;
    [SerializeField] private ParticleSystem moveParticles;
    [SerializeField] private TrailRenderer runTrail;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        isPlayerDashing.OnValueChanged += HandleTrail;
    }

    private void OnDisable()
    {
        isPlayerDashing.OnValueChanged -= HandleTrail;
    }

    private void Update()
    {
        HandleParticles();
    }

    private void HandleParticles()
    {
        // Play particles when moving, stop when idle
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            if (!moveParticles.isPlaying)
                moveParticles.Play();
        }
        else
        {
            if (moveParticles.isPlaying)
                moveParticles.Stop();
        }
    }

    private void HandleTrail(bool isDashing)
    {
        // Enable trail only when Run (Shift) is pressed
        if (isDashing)
        {
            if (!runTrail.emitting)
                runTrail.emitting = true;
        }
        else
        {
            if (runTrail.emitting)
                runTrail.emitting = false;
        }
    }
}
