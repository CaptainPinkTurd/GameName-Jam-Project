using System;
using System.Collections.Generic;
using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.CustomDataStructure;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Variables;
using CaptainPinkTurd.Core.Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CaptainPinkTurd.RPG
{
    public class TilemapFootstep : MonoBehaviour
    {
        [SerializeField] private SerializeKeyValuePair<TileBase, List<AudioClip>>[] footstepDataByTile;
        [SerializeField] private Vector3VariableSO playerPosition;
        [SerializeField] private float footstepInterval = 0.5f;

        private Vector3Int currentGridPosition;
        private SoundData soundData;
        private Tilemap tilemap;
        private float elapsedTime;

        private void Awake()
        {
            soundData = new SoundData();
            tilemap = GetComponent<Tilemap>();
        }

        private void OnEnable()
        {
            playerPosition.OnValueChanged += PlayFootstepAtPosition;
        }

        private void OnDisable()
        {
            playerPosition.OnValueChanged -= PlayFootstepAtPosition;
        }

        private void Update()
        {
            elapsedTime += Time.deltaTime;
        }

        private void PlayFootstepAtPosition(Vector3 position)
        {
            var gridPosition = tilemap.WorldToCell(position);
            
            if (!tilemap.HasTile(gridPosition) || gridPosition == currentGridPosition || elapsedTime < footstepInterval) return;
            
            elapsedTime = 0;
            currentGridPosition = gridPosition;
            
            var tile = tilemap.GetTile<TileBase>(gridPosition);
            if (footstepDataByTile.TryGetValue(tile, out var footstepSounds))
            {
                int randomIndex = UnityEngine.Random.Range(0, footstepSounds.Count);
                soundData.clip = footstepSounds[randomIndex];
                SoundManager.Instance.CreateSoundBuilder()
                    .WithPosition(position).WithRandomPitch().Play(soundData);
            }
            else
            {
                Debug.LogError($"No footstep sound found for tile {tile.name}");
            }
        }
    }
}