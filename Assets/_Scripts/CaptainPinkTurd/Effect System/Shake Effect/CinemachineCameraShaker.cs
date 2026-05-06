using Unity.Cinemachine;
using UnityEngine;

namespace CaptainPinkTurd.EffectSystem.ShakeEffect
{
    [RequireComponent(typeof(CinemachineCamera))]
    public class CinemachineCameraShaker : DefaultGameObjectShaker
    {
        private CinemachineCamera cinemachineCamera;
        private Transform followTarget;
        
        private void Awake()
        {
            cinemachineCamera = GetComponent<CinemachineCamera>();
        }
        
        public override void Shake()
        {
            followTarget = cinemachineCamera.Follow;
            cinemachineCamera.Follow = null;
            
            base.Shake();
        }

        protected override void OnShakeComplete()
        {
            base.OnShakeComplete();
            
            cinemachineCamera.Follow = followTarget;
        }
    }
}