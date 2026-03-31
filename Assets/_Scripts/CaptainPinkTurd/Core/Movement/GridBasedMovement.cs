using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CaptainPinkTurd.Core.Movement
{
    public class GridBasedMovement : MonoBehaviour
    {
        [Header("Grid Based Movement Configs")] 
        [SerializeField] private float moveTimeBetweenGrids = 0.2f;
        [SerializeField] private List<Tilemap> groundTiles;
        [SerializeField] private List<Tilemap> collisionTiles;
        
        private Tilemap currentGroundTilemap;
        private Tilemap currentCollisionTilemap;
        private Vector3 targetPos;
            
        protected bool isMoving;

        protected virtual void OnEnable()
        {
            targetPos = transform.position;
            isMoving = false;
        }
        protected virtual void OnDisable()
        {
            transform.position = targetPos;
        }
        
        protected void Move(Vector2 direction)
        {
            if (isMoving) return;
            
            currentGroundTilemap = GetActiveTilemap(groundTiles);
            currentCollisionTilemap = GetActiveTilemap(collisionTiles);
            
            if (CanMove(direction))
            {
                StartCoroutine(MoveToGrid(direction));
            }
        }

        private IEnumerator MoveToGrid(Vector2 direction)
        {
            isMoving = true;
            float elapsedTime = 0;
            
            var originalPos = transform.position;
            var targetGridPos = currentGroundTilemap.WorldToCell(originalPos + (Vector3)direction);
            targetPos = currentCollisionTilemap.GetCellCenterWorld(targetGridPos);
            
            while (elapsedTime < moveTimeBetweenGrids)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(originalPos, targetPos, elapsedTime / moveTimeBetweenGrids);
                yield return null;
            }
            
            transform.position = targetPos;
            isMoving = false;
        }
        private bool CanMove(Vector2 direction)
        {
            Vector3Int gridPosition = currentGroundTilemap.WorldToCell(transform.position + (Vector3)direction);
            
            return currentGroundTilemap.HasTile(gridPosition) &&
                   !currentCollisionTilemap.HasTile(gridPosition);
        }

        private Tilemap GetActiveTilemap(List<Tilemap> tilemaps)
        {
            foreach (var tilemap in tilemaps)
            {
                if(tilemap.isActiveAndEnabled) return tilemap;
            }
            
            Debug.LogError("No tilemap is active");
            return null;
        }
    }
}