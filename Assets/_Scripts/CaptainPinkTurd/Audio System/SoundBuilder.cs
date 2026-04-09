using UnityEngine;

namespace CaptainPinkTurd.AudioSystem
{
    public class SoundBuilder
    {
        private readonly SoundManager soundManager;
        private Vector3 position = Vector3.zero;
        private bool randomPitch;

        public SoundBuilder(SoundManager soundManager) 
        {
            this.soundManager = soundManager;
        }

        public SoundBuilder WithPosition(Vector3 position) 
        {
            this.position = position;
            return this;
        }

        public SoundBuilder WithRandomPitch() 
        {
            this.randomPitch = true;
            return this;
        }

        public void Play(SoundData soundData) 
        {
            if (soundData == null || !soundData.clip) 
            {
                Debug.LogError("SoundData is null");
                return;
            }
            
            if (!soundManager.CanPlaySound(soundData)) return;

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
                soundEmitter.WithRandomPitch();
            }

            if (soundData.frequentSound) 
            {
                soundEmitter.Node = soundManager.FrequentSoundEmitters.AddLast(soundEmitter);
            }
            
            soundEmitter.Play();
        }
    }
}
