using System;
using UnityEngine;

namespace CaptainPinkTurd.AudioSystem
{
    public class SoundBuilder
    {
        private readonly SoundManager soundManager;
        private Vector3 position = Vector3.zero;
        
        private bool randomPitch;
        private float minPitch;
        private float maxPitch;

        public SoundBuilder(SoundManager soundManager) 
        {
            this.soundManager = soundManager;
        }

        public SoundBuilder WithPosition(Vector3 position) 
        {
            this.position = position;
            return this;
        }

        public SoundBuilder WithRandomPitch(float min = -0.05f, float max = 0.05f) 
        {
            minPitch = min;
            maxPitch = max;
            this.randomPitch = true;
            return this;
        }

        public void Play(SoundData soundData, Action onSoundEnd = null) 
        {
            if (soundData == null || !soundData.clip) 
            {
                Debug.LogWarning("SoundData is null");
                return;
            }
            
            if (soundManager && !soundManager.CanPlaySound(soundData)) return;

            SoundEmitter soundEmitter = soundManager.Get();
            
            if (!soundEmitter)
            {
                Debug.LogError("No sound emitters available");
                return;
            }
            
            soundEmitter.Initialize(soundData);
            soundEmitter.transform.position = position;
            soundEmitter.transform.parent = soundManager.transform;

            if (randomPitch) 
            {
                soundEmitter.WithRandomPitch(minPitch, maxPitch);
            }

            if (soundData.frequentSound) 
            {
                soundEmitter.Node = soundManager.FrequentSoundEmitters.AddLast(soundEmitter);
            }
            
            soundEmitter.onSoundEnd = onSoundEnd;
            soundEmitter.Play();
        }
    }
}
