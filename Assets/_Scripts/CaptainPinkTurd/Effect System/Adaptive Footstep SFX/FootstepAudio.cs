using System;
using System.Collections.Generic;
using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.Enum;
using CaptainPinkTurd.Core.SO;
using CaptainPinkTurd.TilemapSystem;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CaptainPinkTurd.EffectSystem.AdaptiveFootstepSFX
{
    public class FootstepAudio : MonoBehaviour
    {
        [SerializeField] private LayerMask terrainMask;
        [SerializeField] private Transform footPosition;
        [SerializeField] private EGamePerspective gamePerspective;
        [SerializeField] internal List<Sounds> terrainDatabase = new List<Sounds>();
        
        public void Step()
        {
            TerrainType terrainType = null;
            
            switch (gamePerspective)
            {
                case EGamePerspective.Topdown2D:
                    var hits = Physics2D.RaycastAll(footPosition.position, 
                        transform.forward, Mathf.Infinity, terrainMask);
                    if (hits.Length == 0)
                    {
                        Debug.LogError("No Terrain detected");
                        return;
                    }
            
                    var topObject = hits[0].collider.gameObject;
                    for (int i = 1; i < hits.Length; i++)
                    {
                        var hitColliderRenderer = hits[i].collider.GetComponent<Renderer>();
                        var topObjectRenderer = topObject.GetComponent<Renderer>();

                        if (hitColliderRenderer.sortingOrder > topObjectRenderer.sortingOrder &&
                            hitColliderRenderer.sortingOrder < GetComponent<Renderer>().sortingOrder)
                        {
                            topObject = hits[i].collider.gameObject;
                        }
                    }
            
                    if(topObject.TryGetComponent(out Tilemap tilemap))
                    {
                        var topTile = tilemap.GetTile(tilemap.WorldToCell(footPosition.position));

                        switch (topTile)
                        {
                            case TerrainTile terrainTile:
                                terrainType = terrainTile.terrain;
                                break;
                            case TerrainRuleTile terrainRuleTile:
                                terrainType = terrainRuleTile.terrain;
                                break;
                        }
                    }
                    else if(topObject.TryGetComponent(out TerrainBehaviour terrainBehaviour))
                    {
                        terrainType =  terrainBehaviour.terrain;
                    }

                    break;
                case EGamePerspective.Sidescroller2D:
                    break;
            }
            
            if(terrainType)
            {
                Sounds sounds = null;
                foreach (var tdb in terrainDatabase)
                {
                    if (tdb.terrainType != terrainType.name) return;

                    sounds = tdb;
                    break;
                }

                if (sounds != null)
                {
                    var sfx = sounds.footsteps[UnityEngine.Random.Range(0, sounds.footsteps.Length)];
                    SoundManager.Instance.CreateSoundBuilder().WithPosition(footPosition.position).WithRandomPitch().Play(sfx);
                }
                else
                {
                    Debug.LogError("No footstep sound found for terrain type: " + terrainType.name);
                }
            }
            else
            {
                Debug.LogError("No terrain tile detected");
            }
        }
         
        [Serializable]
        public class Sounds
        {
            [HideInInspector] public string terrainType;
            public SoundData[] footsteps = Array.Empty<SoundData>();
            
            public Sounds(string terrainType)
            {
                this.terrainType = terrainType;
            }
        }
    }
}