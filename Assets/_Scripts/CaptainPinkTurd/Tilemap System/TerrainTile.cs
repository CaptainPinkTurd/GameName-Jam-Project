using CaptainPinkTurd.Core.SO;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CaptainPinkTurd.TilemapSystem
{
    [CreateAssetMenu(menuName = "2D/Tiles/Terrain Tile")]
    public class TerrainTile : Tile
    {
        public TerrainType terrain;
    }
}