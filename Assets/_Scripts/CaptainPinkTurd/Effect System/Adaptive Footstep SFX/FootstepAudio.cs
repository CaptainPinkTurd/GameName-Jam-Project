using System;
using System.Collections.Generic;
using UnityEngine;

namespace CaptainPinkTurd.EffectSystem.AdaptiveFootstepSFX
{
    public class FootstepAudio : MonoBehaviour
    {
        public List<Sounds> terrainDatabase = new List<Sounds>();
        
        [Serializable]
        public class Sounds
        {
            [HideInInspector] public string terrainType;
            public AudioClip[] footsteps = Array.Empty<AudioClip>();
            
            public Sounds(string terrainType)
            {
                this.terrainType = terrainType;
            }
        }
    }
}