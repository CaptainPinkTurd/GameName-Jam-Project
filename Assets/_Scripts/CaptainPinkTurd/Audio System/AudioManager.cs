using System;
using CaptainPinkTurd.Core.DesignPattern.Singleton;
using UnityEngine;
using UnityEngine.Audio;

namespace CaptainPinkTurd.AudioSystem
{
    public enum EVolumeType
    {
        Master = 0,
        Music = 1,
        SFX = 2
    }
    public class AudioManager : PersistentSingleton<AudioManager>
    {
        [Header("Mixer Groups References")]
        [SerializeField] private AudioMixerGroup masterMixerGroup;
        [SerializeField] private AudioMixerGroup musicMixerGroup;
        [SerializeField] private AudioMixerGroup sfxMixerGroup;
        
        [Header("Mixer Parameters")]
        [SerializeField] private string masterVolumeParam = "masterVolume";
        [SerializeField] private string musicVolumeParam = "musicVolume";
        [SerializeField] private string sfxVolumeParam = "soundFxVolume";
        
        // Cached values (linear 0–1)
        private float masterVolume = 1f;
        private float musicVolume = 1f;
        private float sfxVolume = 1f;
        
        public AudioMixerGroup MasterMixerGroup => masterMixerGroup;
        public AudioMixerGroup MusicMixerGroup => musicMixerGroup;
        public AudioMixerGroup SfxMixerGroup => sfxMixerGroup;
        public float GetMasterVolume() => masterVolume;
        public float GetMusicVolume() => musicVolume;
        public float GetSFXVolume() => sfxVolume;

        private void Start()
        {
            Load();
            ApplyAllSettings();
        }

        #region Public API

        public void SetVolume(EVolumeType volumeType, float value)
        {
            switch (volumeType)
            {
                case EVolumeType.Master:
                    masterVolume = value;
                    ApplyVolume(masterVolumeParam, value);
                    break;
                case EVolumeType.Music:
                    musicVolume = value;
                    ApplyVolume(musicVolumeParam, value);
                    break;
                case EVolumeType.SFX:
                    sfxVolume = value;
                    ApplyVolume(sfxVolumeParam, value);
                    break;
            }
            Save();
        }
        #endregion 
        
        #region Internal Logic
        private void ApplyAllSettings()
        {
            ApplyVolume(masterVolumeParam, masterVolume);
            ApplyVolume(musicVolumeParam, musicVolume);
            ApplyVolume(sfxVolumeParam, sfxVolume);
        }

        private void ApplyVolume(string parameter, float value)
        {
            // Convert linear (0–1) to logarithmic (dB)
            float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
            masterMixerGroup.audioMixer.SetFloat(parameter, dB);
        }

        private void Save()
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        }

        private void Load()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", .75f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", .75f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", .75f);
        }
        #endregion
    }
}