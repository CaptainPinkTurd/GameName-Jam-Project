using CaptainPinkTurd.Core.DesignPattern.Singleton;
using Unity.Cinemachine;
using UnityEngine;

namespace CaptainPinkTurd.EffectSystem.ShakeEffect
{
    public class CameraShakeManager : Singleton<CameraShakeManager>
    {
        [SerializeField] private CinemachineImpulseSource impulseSource;
        [SerializeField] private CinemachineImpulseListener impulseListener;
        [SerializeField] private float globalShakeForce = 1f;
        
        private CinemachineImpulseDefinition ImpulseDefinition => impulseSource.ImpulseDefinition;
        
        public void ScreenShake()
        {
            impulseSource.GenerateImpulseWithForce(globalShakeForce);
        }
        public void ScreenShakeFromProfile(ScreenShakeProfile profile)
        {
            SetupScreenShakeSettings(profile);
            impulseSource.GenerateImpulseWithForce(profile.impulseForce);
        }

        private void SetupScreenShakeSettings(ScreenShakeProfile profile)
        {
            ImpulseDefinition.ImpulseDuration = profile.impulseTime;
            impulseSource.DefaultVelocity = profile.defaultVelocity;
            if (profile.ImpulseShapeIsCustom)
            {
                ImpulseDefinition.CustomImpulseShape = profile.impulseCustomShape;
            }
            else
            {
                ImpulseDefinition.ImpulseShape = profile.impulseShape;
            }
            
            impulseListener.ReactionSettings.AmplitudeGain = profile.listenerAmplitude;
            impulseListener.ReactionSettings.FrequencyGain = profile.listenerFrequency;
            impulseListener.ReactionSettings.Duration = profile.listenerDuration;
        }
    }
}
